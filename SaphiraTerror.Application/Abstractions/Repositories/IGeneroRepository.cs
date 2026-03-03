using SaphiraTerror.Domain.Entities;

namespace SaphiraTerror.Application.Abstractions.Repositories;

public interface IGeneroRepository
{
    Task<IReadOnlyList<Genero>> GetAllAsync(CancellationToken ct = default);
}
