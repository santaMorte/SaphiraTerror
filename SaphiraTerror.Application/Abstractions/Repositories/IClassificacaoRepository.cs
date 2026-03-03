using SaphiraTerror.Domain.Entities;

namespace SaphiraTerror.Application.Abstractions.Repositories;

public interface IClassificacaoRepository
{
    Task<IReadOnlyList<Classificacao>> GetAllAsync(CancellationToken ct = default);
}
