using SaphiraTerror.Application.DTOs;
using SaphiraTerror.Application.Filters;

namespace SaphiraTerror.Application.Services;

/// <summary>
/// Serviço de aplicação para consultar o catálogo com filtros/paginação.
/// </summary>
public interface IFilmeQueryService
{
    Task<PagedResult<FilmeDto>> SearchAsync(FilmeFilter filter, CancellationToken ct = default);
}
