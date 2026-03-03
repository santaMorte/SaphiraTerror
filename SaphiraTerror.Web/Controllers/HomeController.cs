//using Microsoft.AspNetCore.Mvc;

//namespace SaphiraTerror.Web.Controllers;

//public class HomeController : Controller
//{
//    public IActionResult Index() => View();
//    public IActionResult About() => View();
//    public IActionResult Catalog() => View();
//    public IActionResult Genre() => View();
//    public IActionResult Logout() => RedirectToAction("Index"); // placeholder Fase 5
//}


//refatorado fase 4

using Microsoft.AspNetCore.Mvc;

using SaphiraTerror.Web.Models;
using SaphiraTerror.Web.Services;

namespace SaphiraTerror.Web.Controllers;

public class HomeController(IApiClient api, ILogger<HomeController> log) : Controller
{
    private readonly IApiClient _api = api;

    private async Task<CatalogPageVm> BuildVm(CatalogFilterVm filter, CancellationToken ct)
    {
        if (filter.Page < 1) filter.Page = 1;
        if (filter.PageSize < 1 || filter.PageSize > 48) filter.PageSize = 12;

        return new CatalogPageVm
        {
            Filter = filter,
            Generos = await _api.GetGenerosAsync(ct),
            Classificacoes = await _api.GetClassificacoesAsync(ct),
            PageResult = await _api.SearchFilmesAsync(filter, ct)
        };
    }

    // ONE-PAGE: /  (aceita filtros via querystring)
    [HttpGet("")]
    public async Task<IActionResult> Index([FromQuery] CatalogFilterVm filter, string? section, CancellationToken ct)
    {
        var vm = await BuildVm(filter, ct);
        ViewBag.Section = section; // "about" | "genres" | "catalog"
        return View(vm);
    }

    // Compat: /Home/Catalog -> Index em #catalog
    [HttpGet]
    public Task<IActionResult> Catalog([FromQuery] CatalogFilterVm filter, CancellationToken ct)
        => Index(filter, "catalog", ct);

    // Compat: /Home/Genre -> Index em #genres
    [HttpGet]
    public Task<IActionResult> Genre(CancellationToken ct)
        => Index(new CatalogFilterVm(), "genres", ct);
}

