//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using SaphiraTerror.Infrastructure.Persistence;
//using SaphiraTerror.Web.Areas.Admin.Models;

//namespace SaphiraTerror.Web.Areas.Admin.Controllers;

//[Area("Admin")]
//[Authorize(Policy = "ManagerOrAdmin")]
//public class FilmesController : Controller
//{
//    private readonly AppDbContext _db;

//    public FilmesController(AppDbContext db) => _db = db;

//    // GET: /Admin/Filmes
//    [HttpGet]
//    public async Task<IActionResult> Index([FromQuery] FilmeListFilter f, CancellationToken ct)
//    {
//        f.Page = Math.Max(1, f.Page);
//        f.PageSize = Math.Clamp(f.PageSize, 5, 48);

//        var q = _db.Filmes.AsNoTracking()
//            .Include(x => x.Genero)
//            .Include(x => x.Classificacao)
//            .AsQueryable();

//        if (!string.IsNullOrWhiteSpace(f.Q))
//        {
//            var term = f.Q.Trim();
//            q = q.Where(x => x.Titulo.Contains(term) ||
//                             (x.Sinopse != null && x.Sinopse.Contains(term)));
//        }
//        if (f.GeneroId is int gid) q = q.Where(x => x.GeneroId == gid);
//        if (f.ClassificacaoId is int cid) q = q.Where(x => x.ClassificacaoId == cid);
//        if (f.Ano is int ano) q = q.Where(x => x.Ano == ano);

//        q = (f.SortBy?.ToLowerInvariant()) switch
//        {
//            "titulo" => f.Desc ? q.OrderByDescending(x => x.Titulo) : q.OrderBy(x => x.Titulo),
//            "ano" => f.Desc ? q.OrderByDescending(x => x.Ano) : q.OrderBy(x => x.Ano),
//            _ => f.Desc ? q.OrderByDescending(x => x.CreatedAt) : q.OrderBy(x => x.CreatedAt),
//        };

//        var total = await q.CountAsync(ct);
//        var items = await q.Skip((f.Page - 1) * f.PageSize).Take(f.PageSize)
//            .Select(x => new FilmeRowVm(
//                x.Id, x.Titulo, x.Ano, x.Genero.Nome, x.Classificacao.Nome))
//            .ToListAsync(ct);

//        var result = new PagedResult<FilmeRowVm>(f.Page, f.PageSize, total, items);

//        ViewBag.Filter = f;
//        ViewBag.Generos = await _db.Generos.AsNoTracking().OrderBy(g => g.Nome).ToListAsync(ct);
//        ViewBag.Classificacoes = await _db.Classificacoes.AsNoTracking().OrderBy(c => c.Nome).ToListAsync(ct);

//        return View("~/Areas/Admin/Views/Filmes/Index.cshtml", result);
//    }

//    // GET: /Admin/Filmes/Create
//    [HttpGet]
//    public async Task<IActionResult> Create(CancellationToken ct)
//    {
//        await CarregarTaxonomiasAsync(ct);
//        return View("~/Areas/Admin/Views/Filmes/Create.cshtml", new FilmeEditVm());
//    }

//    // POST: /Admin/Filmes/Create
//    [HttpPost]
//    [ValidateAntiForgeryToken]
//    public async Task<IActionResult> Create(FilmeEditVm vm, CancellationToken ct)
//    {
//        if (!await ValidarTaxonomiasAsync(vm, ct))
//            ModelState.AddModelError("", "Gênero ou Classificação inválidos.");

//        if (!ModelState.IsValid)
//        {
//            await CarregarTaxonomiasAsync(ct);
//            return View("~/Areas/Admin/Views/Filmes/Create.cshtml", vm);
//        }

//        var entity = new SaphiraTerror.Domain.Entities.Filme
//        {
//            Titulo = vm.Titulo.Trim(),
//            Sinopse = vm.Sinopse?.Trim(),
//            Ano = vm.Ano,
//            ImagemCapaUrl = vm.ImagemCapaUrl?.Trim(),
//            GeneroId = vm.GeneroId,
//            ClassificacaoId = vm.ClassificacaoId,
//            CreatedAt = DateTime.UtcNow
//        };

//        _db.Filmes.Add(entity);
//        await _db.SaveChangesAsync(ct);
//        TempData["ok"] = "Filme criado com sucesso.";
//        return RedirectToAction(nameof(Index));
//    }

//    // GET: /Admin/Filmes/Edit/5
//    [HttpGet]
//    public async Task<IActionResult> Edit(int id, CancellationToken ct)
//    {
//        var f = await _db.Filmes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);
//        if (f == null) return NotFound();

//        await CarregarTaxonomiasAsync(ct);

//        var vm = new FilmeEditVm
//        {
//            Id = f.Id,
//            Titulo = f.Titulo,
//            Sinopse = f.Sinopse,
//            Ano = f.Ano,
//            ImagemCapaUrl = f.ImagemCapaUrl,
//            GeneroId = f.GeneroId,
//            ClassificacaoId = f.ClassificacaoId
//        };
//        return View("~/Areas/Admin/Views/Filmes/Edit.cshtml", vm);
//    }

//    // POST: /Admin/Filmes/Edit/5
//    [HttpPost]
//    [ValidateAntiForgeryToken]
//    public async Task<IActionResult> Edit(int id, FilmeEditVm vm, CancellationToken ct)
//    {
//        if (id != vm.Id) return BadRequest();

//        if (!await ValidarTaxonomiasAsync(vm, ct))
//            ModelState.AddModelError("", "Gênero ou Classificação inválidos.");

//        if (!ModelState.IsValid)
//        {
//            await CarregarTaxonomiasAsync(ct);
//            return View("~/Areas/Admin/Views/Filmes/Edit.cshtml", vm);
//        }

//        var f = await _db.Filmes.FirstOrDefaultAsync(x => x.Id == id, ct);
//        if (f == null) return NotFound();

//        f.Titulo = vm.Titulo.Trim();
//        f.Sinopse = vm.Sinopse?.Trim();
//        f.Ano = vm.Ano;
//        f.ImagemCapaUrl = vm.ImagemCapaUrl?.Trim();
//        f.GeneroId = vm.GeneroId;
//        f.ClassificacaoId = vm.ClassificacaoId;
//        // f.UpdatedAt = DateTime.UtcNow; // <-- removido (não existe na entidade)

//        await _db.SaveChangesAsync(ct);
//        TempData["ok"] = "Filme atualizado com sucesso.";
//        return RedirectToAction(nameof(Index));
//    }

//    // GET: /Admin/Filmes/Delete/5  (confirmação)
//    [HttpGet]
//    public async Task<IActionResult> Delete(int id, CancellationToken ct)
//    {
//        var f = await _db.Filmes
//            .Include(x => x.Genero).Include(x => x.Classificacao)
//            .AsNoTracking()
//            .FirstOrDefaultAsync(x => x.Id == id, ct);
//        if (f == null) return NotFound();
//        return View("~/Areas/Admin/Views/Filmes/Delete.cshtml", f);
//    }

//    // POST: /Admin/Filmes/Delete/5  (execução)
//    [HttpPost, ActionName("Delete")]
//    [ValidateAntiForgeryToken]
//    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
//    {
//        bool isAdmin = User.IsInRole("Admin") || User.IsInRole("ADMIN") || User.IsInRole("Administrador");
//        if (!isAdmin) return Forbid();

//        var f = await _db.Filmes.FirstOrDefaultAsync(x => x.Id == id, ct);
//        if (f == null) return NotFound();

//        _db.Filmes.Remove(f);
//        await _db.SaveChangesAsync(ct);
//        TempData["ok"] = "Filme excluído.";
//        return RedirectToAction(nameof(Index));
//    }

//    private async Task CarregarTaxonomiasAsync(CancellationToken ct)
//    {
//        ViewBag.Generos = await _db.Generos.AsNoTracking().OrderBy(g => g.Nome).ToListAsync(ct);
//        ViewBag.Classificacoes = await _db.Classificacoes.AsNoTracking().OrderBy(c => c.Nome).ToListAsync(ct);
//    }

//    private async Task<bool> ValidarTaxonomiasAsync(FilmeEditVm vm, CancellationToken ct)
//    {
//        var gOk = await _db.Generos.AnyAsync(g => g.Id == vm.GeneroId, ct);
//        var cOk = await _db.Classificacoes.AnyAsync(c => c.Id == vm.ClassificacaoId, ct);
//        return gOk && cOk;
//    }
//}


//refatorado fase 08

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SaphiraTerror.Domain.Entities;
using SaphiraTerror.Infrastructure.Entities;            // Filme, Genero, Classificacao
using SaphiraTerror.Infrastructure.Persistence;         // AppDbContext
using SaphiraTerror.Web.Areas.Admin.Models;            // FilmeEditVm
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SaphiraTerror.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = "ManagerOrAdmin")]
public sealed class FilmesController : Controller
{
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;

    public FilmesController(AppDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    // ============================================================
    // Helpers
    // ============================================================
    private void PopularCombos()
    {
        ViewBag.Generos = _db.Generos
            .OrderBy(g => g.Nome)
            .Select(g => new { g.Id, g.Nome })
            .ToList();

        ViewBag.Classificacoes = _db.Classificacoes
            .OrderBy(c => c.Nome)
            .Select(c => new { c.Id, c.Nome })
            .ToList();
    }

    private static FilmeEditVm MapToVm(Filme f) => new FilmeEditVm
    {
        Id = f.Id,
        Titulo = f.Titulo,
        Ano = f.Ano,
        GeneroId = f.GeneroId,
        ClassificacaoId = f.ClassificacaoId,
        Sinopse = f.Sinopse,
        ImagemCapaUrl = f.ImagemCapaUrl
    };

    private static void MapToEntity(FilmeEditVm vm, Filme f)
    {
        f.Titulo = vm.Titulo;
        f.Ano = vm.Ano;
        f.GeneroId = vm.GeneroId;
        f.ClassificacaoId = vm.ClassificacaoId;
        f.Sinopse = vm.Sinopse;
        f.ImagemCapaUrl = vm.ImagemCapaUrl;
    }

    private async Task<string?> SalvarCapaAsync(IFormFile file, CancellationToken ct)
    {
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        var permitido = new[] { ".jpg", ".jpeg", ".png", ".webp" };

        if (!permitido.Contains(ext))
        {
            ModelState.AddModelError("ImagemCapaUrl", "Formato inválido. Use JPG, PNG ou WEBP.");
            return null;
        }

        if (file.Length > 2 * 1024 * 1024) // 2 MB
        {
            ModelState.AddModelError("ImagemCapaUrl", "Arquivo muito grande (máx. 2 MB).");
            return null;
        }

        var dir = Path.Combine(_env.WebRootPath, "uploads", "filmes");
        Directory.CreateDirectory(dir);

        var fileName = $"{Guid.NewGuid():N}{ext}";
        var savePath = Path.Combine(dir, fileName);

        await using var fs = System.IO.File.Create(savePath);
        await file.CopyToAsync(fs, ct);

        // caminho público
        return $"/uploads/filmes/{fileName}";
    }

    // ============================================================
    // CRUD
    // ============================================================

    public async Task<IActionResult> Index()
    {
        var list = await _db.Filmes
            .AsNoTracking()
            .Include(f => f.Genero)
            .Include(f => f.Classificacao)
            .OrderByDescending(f => f.Id)
            .Select(f => new
            {
                f.Id,
                f.Titulo,
                f.Ano,
                Genero = f.Genero!.Nome,
                Classificacao = f.Classificacao!.Nome,
                f.ImagemCapaUrl
            }).ToListAsync();

        return View(list);
    }

    public async Task<IActionResult> Details(int id)
    {
        var f = await _db.Filmes
            .AsNoTracking()
            .Include(x => x.Genero)
            .Include(x => x.Classificacao)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (f is null) return NotFound();

        var vm = MapToVm(f);
        ViewBag.GeneroNome = f.Genero?.Nome;
        ViewBag.ClassificacaoNome = f.Classificacao?.Nome;
        return View(vm);
    }

    [HttpGet]
    public IActionResult Create()
    {
        PopularCombos();
        return View(new FilmeEditVm());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FilmeEditVm model, IFormFile? capaArquivo, CancellationToken ct)
    {
        if (capaArquivo is not null && capaArquivo.Length > 0)
        {
            var saved = await SalvarCapaAsync(capaArquivo, ct);
            if (ModelState.IsValid && saved is not null)
                model.ImagemCapaUrl = saved;
        }

        if (!ModelState.IsValid)
        {
            PopularCombos();
            return View(model);
        }

        var entity = new Filme();
        MapToEntity(model, entity);

        _db.Filmes.Add(entity);
        await _db.SaveChangesAsync(ct);

        TempData["ok"] = "Filme criado com sucesso.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var f = await _db.Filmes.FindAsync(id);
        if (f is null) return NotFound();

        PopularCombos();
        return View(MapToVm(f));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, FilmeEditVm model, IFormFile? capaArquivo, CancellationToken ct)
    {
        if (id != model.Id) return BadRequest();

        if (capaArquivo is not null && capaArquivo.Length > 0)
        {
            var saved = await SalvarCapaAsync(capaArquivo, ct);
            if (ModelState.IsValid && saved is not null)
                model.ImagemCapaUrl = saved;
        }

        if (!ModelState.IsValid)
        {
            PopularCombos();
            return View(model);
        }

        var f = await _db.Filmes.FirstOrDefaultAsync(x => x.Id == id);
        if (f is null) return NotFound();

        MapToEntity(model, f);
        _db.Filmes.Update(f);
        await _db.SaveChangesAsync(ct);

        TempData["ok"] = "Filme atualizado.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var f = await _db.Filmes
            .AsNoTracking()
            .Include(x => x.Genero)
            .Include(x => x.Classificacao)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (f is null) return NotFound();
        return View(f);
    }

    [Authorize(Policy = "AdminOnly")]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
    {
        var f = await _db.Filmes.FindAsync(id);
        if (f is null) return NotFound();

        _db.Filmes.Remove(f);
        await _db.SaveChangesAsync(ct);
        TempData["ok"] = "Filme removido.";
        return RedirectToAction(nameof(Index));
    }
}
