namespace SaphiraTerror.Application.Services;

/// <summary>
/// Serviço de listas auxiliares (gêneros e classificações).
/// </summary>
public interface ICatalogLookupService
{
    Task<IReadOnlyList<(int Id, string Nome)>> GetGenerosAsync(CancellationToken ct = default);
    Task<IReadOnlyList<(int Id, string Nome)>> GetClassificacoesAsync(CancellationToken ct = default);
}
