using Microsoft.EntityFrameworkCore;
using SaphiraTerror.Application.Abstractions.Repositories;
using SaphiraTerror.Domain.Entities;
using SaphiraTerror.Infrastructure.Persistence;

namespace SaphiraTerror.Infrastructure.Repositories;

public sealed class EfGeneroRepository : IGeneroRepository
{
    private readonly AppDbContext _ctx;
    public EfGeneroRepository(AppDbContext ctx) => _ctx = ctx;

    public async Task<IReadOnlyList<Genero>> GetAllAsync(CancellationToken ct = default) =>
        await _ctx.Generos.AsNoTracking()
            .OrderBy(g => g.Nome)
            .ToListAsync(ct);
}
