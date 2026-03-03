using Microsoft.AspNetCore.Mvc;
using SaphiraTerror.Application.DTOs;
using SaphiraTerror.Application.Filters;
using SaphiraTerror.Application.Services;
using SaphiraTerror.Application.Abstractions.Repositories;

namespace SaphiraTerror.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FilmesController : ControllerBase
{
    private readonly IFilmeQueryService _service;
    private readonly IFilmeRepository _repo; // para GetById

    public FilmesController(IFilmeQueryService service, IFilmeRepository repo)
    {
        _service = service;
        _repo = repo;
    }

    /// <summary>Consulta paginada com filtros/ordenação.</summary>
    [HttpGet]
    public async Task<ActionResult<PagedResult<FilmeDto>>> Get([FromQuery] FilmeFilter filter, CancellationToken ct)
    {
        var result = await _service.SearchAsync(filter, ct);
        Response.Headers["X-Total-Count"] = result.Total.ToString();
        return Ok(result);
    }

    /// <summary>Detalhe por Id.</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<FilmeDto>> GetById(int id, CancellationToken ct)
    {
        var f = await _repo.GetByIdAsync(id, ct);
        if (f is null) return NotFound();

        var dto = new FilmeDto(
            f.Id, f.Titulo, f.Sinopse, f.Ano, f.ImagemCapaUrl,
            f.GeneroId, f.Genero.Nome, f.ClassificacaoId, f.Classificacao.Nome);

        return Ok(dto);
    }
}
