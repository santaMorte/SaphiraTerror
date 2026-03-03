using SaphiraTerror.Application.Abstractions.Repositories;
using SaphiraTerror.Application.Services;

namespace SaphiraTerror.Infrastructure.Services;

/// <summary>
/// Fornece listas auxiliares (gêneros/classificações) em forma simples.
/// </summary>
public sealed class CatalogLookupService : ICatalogLookupService
{
    private readonly IGeneroRepository _generos;
    private readonly IClassificacaoRepository _classificacoes;

    public CatalogLookupService(
        IGeneroRepository generos,
        IClassificacaoRepository classificacoes)
    {
        _generos = generos;
        _classificacoes = classificacoes;
    }

    public async Task<IReadOnlyList<(int Id, string Nome)>> GetGenerosAsync(CancellationToken ct = default)
    {
        var list = await _generos.GetAllAsync(ct);
        return list.Select(g => (g.Id, g.Nome)).ToList();
    }

    public async Task<IReadOnlyList<(int Id, string Nome)>> GetClassificacoesAsync(CancellationToken ct = default)
    {
        var list = await _classificacoes.GetAllAsync(ct);
        return list.Select(c => (c.Id, c.Nome)).ToList();
    }
}
