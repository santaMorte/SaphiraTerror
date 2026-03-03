namespace SaphiraTerror.Application.DTOs;

//criado fase 01
/// <summary>
/// Pedido de paginação padrão (valores serão saneados no serviço).
/// </summary>
//public record PagedRequest(int Page = 1, int PageSize = 12);

/// <summary>
/// Resultado de paginação com metadados.
/// </summary>
//public record PagedResult<T>(
//    int Page,
//    int PageSize,
//    int Total,
//    IReadOnlyList<T> Items);




//refatorado fase 03
// Classe simples para o binder setar via query (?page=&pageSize=)
public class PagedRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 12;
}


/// <summary>
/// Resultado de paginação com metadados.
/// </summary>
public record PagedResult<T>(
    int Page,
    int PageSize,
    int Total,
    IReadOnlyList<T> Items);
