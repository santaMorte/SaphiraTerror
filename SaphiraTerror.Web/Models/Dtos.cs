namespace SaphiraTerror.Web.Models;

// espelha a resposta da API
public record PagedResult<T>(int Page, int PageSize, int Total, IReadOnlyList<T> Items);

public record FilmeDto(
    int Id,
    string Titulo,
    string? Sinopse,
    int Ano,
    string? ImagemCapaUrl,
    int GeneroId,
    string GeneroNome,
    int ClassificacaoId,
    string ClassificacaoNome);
