using SaphiraTerror.Application.Filters;
using SaphiraTerror.Domain.Entities;

namespace SaphiraTerror.Application.Abstractions.Repositories;

/// <summary>
/// Repositório de leitura/escrita de filmes.
/// Nesta fase usamos só leitura com filtros/paginação.
/// </summary>
public interface IFilmeRepository
{
    Task<(IReadOnlyList<Filme> Items, int Total)> SearchAsync(
        FilmeFilter filter,
        CancellationToken ct = default);

    Task<Filme?> GetByIdAsync(int id, CancellationToken ct = default);
}
