using SaphiraTerror.Application.Abstractions.Repositories;
using SaphiraTerror.Application.DTOs;
using SaphiraTerror.Application.Filters;
using SaphiraTerror.Application.Services;

namespace SaphiraTerror.Infrastructure.Services;

/// <summary>
/// Conecta repositório (Infra) com DTOs (Application).
/// </summary>
public sealed class FilmeQueryService : IFilmeQueryService
{
    private readonly IFilmeRepository _repo;

    public FilmeQueryService(IFilmeRepository repo) => _repo = repo;

    public async Task<PagedResult<FilmeDto>> SearchAsync(FilmeFilter filter, CancellationToken ct = default)
    {
        var (items, total) = await _repo.SearchAsync(filter, ct);

        var dtos = items.Select(f => new FilmeDto(
            f.Id,
            f.Titulo,
            f.Sinopse,
            f.Ano,
            f.ImagemCapaUrl,
            f.GeneroId,
            f.Genero.Nome,
            f.ClassificacaoId,
            f.Classificacao.Nome
        )).ToList();

        // Saneamento de paginação igual ao repositório
        var page = filter.Page < 1 ? 1 : filter.Page;
        var pageSize = filter.PageSize < 1 ? 12 : filter.PageSize;
        if (pageSize > 48) pageSize = 48;

        return new PagedResult<FilmeDto>(page, pageSize, total, dtos);
    }
}
