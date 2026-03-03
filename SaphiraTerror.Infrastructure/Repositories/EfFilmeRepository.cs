using Microsoft.EntityFrameworkCore;
using SaphiraTerror.Application.Abstractions.Repositories;
using SaphiraTerror.Application.Filters;
using SaphiraTerror.Domain.Entities;
using SaphiraTerror.Infrastructure.Persistence;

namespace SaphiraTerror.Infrastructure.Repositories;

/// <summary>
/// Implementação EF Core do repositório de filmes.
/// </summary>
public sealed class EfFilmeRepository : IFilmeRepository
{
    private readonly AppDbContext _ctx;

    public EfFilmeRepository(AppDbContext ctx) => _ctx = ctx;

    public async Task<(IReadOnlyList<Filme> Items, int Total)> SearchAsync(
        FilmeFilter filter, CancellationToken ct = default)
    {
        // Saneamento de paginação
        var page = filter.Page < 1 ? 1 : filter.Page;
        var pageSize = filter.PageSize < 1 ? 12 : filter.PageSize;
        if (pageSize > 48) pageSize = 48;

        var q = _ctx.Filmes
            .AsNoTracking()
            .Include(f => f.Genero)
            .Include(f => f.Classificacao)
            .AsQueryable();

        // Filtros
        if (filter.GeneroId is int gid)
            q = q.Where(f => f.GeneroId == gid);

        if (filter.ClassificacaoId is int cid)
            q = q.Where(f => f.ClassificacaoId == cid);

        if (filter.Ano is int ano)
            q = q.Where(f => f.Ano == ano);

        if (!string.IsNullOrWhiteSpace(filter.Q))
        {
            var like = $"%{filter.Q.Trim()}%";
            q = q.Where(f =>
                EF.Functions.Like(f.Titulo, like) ||
                (f.Sinopse != null && EF.Functions.Like(f.Sinopse, like)));
        }

        // Ordenação
        q = (filter.SortBy?.ToLowerInvariant()) switch
        {
            "titulo" => filter.Desc ? q.OrderByDescending(f => f.Titulo) : q.OrderBy(f => f.Titulo),
            "ano" => filter.Desc ? q.OrderByDescending(f => f.Ano) : q.OrderBy(f => f.Ano),
            "createdat" => filter.Desc ? q.OrderByDescending(f => f.CreatedAt) : q.OrderBy(f => f.CreatedAt),
            _ => q.OrderByDescending(f => f.CreatedAt) // padrão
        };

        var total = await q.CountAsync(ct);

        var items = await q
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public Task<Filme?> GetByIdAsync(int id, CancellationToken ct = default) =>
        _ctx.Filmes
            .AsNoTracking()
            .Include(f => f.Genero)
            .Include(f => f.Classificacao)
            .FirstOrDefaultAsync(f => f.Id == id, ct);
}
