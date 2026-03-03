using Microsoft.EntityFrameworkCore;
using SaphiraTerror.Application.Abstractions.Repositories;
using SaphiraTerror.Domain.Entities;
using SaphiraTerror.Infrastructure.Persistence;

namespace SaphiraTerror.Infrastructure.Repositories;

public sealed class EfClassificacaoRepository : IClassificacaoRepository
{
    private readonly AppDbContext _ctx;
    public EfClassificacaoRepository(AppDbContext ctx) => _ctx = ctx;

    public async Task<IReadOnlyList<Classificacao>> GetAllAsync(CancellationToken ct = default) =>
        await _ctx.Classificacoes.AsNoTracking()
            .OrderBy(c => c.Nome)
            .ToListAsync(ct);
}
