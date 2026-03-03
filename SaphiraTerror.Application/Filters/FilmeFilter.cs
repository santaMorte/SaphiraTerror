using SaphiraTerror.Application.DTOs;
using System.Reflection;

namespace SaphiraTerror.Application.Filters;

/// <summary>
/// Filtros do catálogo + paginação/ordenação.
/// É um 'record' para herdar de PagedRequest (que também é record).
/// </summary>
//public sealed record FilmeFilter : PagedRequest
//{
    //public int? GeneroId { get; init; }
    //public int? ClassificacaoId { get; init; }
    //public int? Ano { get; init; }
    //public string? Q { get; init; }          // busca em Título/Sinopse
    //public string? SortBy { get; init; }     // "Titulo" | "Ano" | "CreatedAt"
    //public bool Desc { get; init; } = true;

    //public FilmeFilter(
    //    int? generoId = null,
    //    int? classificacaoId = null,
    //    int? ano = null,
    //    string? q = null,
    //    string? sortBy = null,
    //    bool desc = true,
    //    int page = 1,
    //    int pageSize = 12
    //) : base(page, pageSize)
    //{
    //    GeneroId = generoId;
    //    ClassificacaoId = classificacaoId;   // <- corrigido (sem typo)
    //    Ano = ano;
    //    Q = q;
    //    SortBy = sortBy;
    //    Desc = desc;
    //}


   
//}

 //refatorado fase 03

// Classe(não record) para o binder preencher pelas props
public sealed class FilmeFilter : PagedRequest
{
    public int? GeneroId { get; set; }
    public int? ClassificacaoId { get; set; }
    public int? Ano { get; set; }
    public string? Q { get; set; }        // busca em Título/Sinopse
    public string? SortBy { get; set; }   // "Titulo" | "Ano" | "CreatedAt"
    public bool Desc { get; set; } = true;
}



