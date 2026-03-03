using System.Net.Http.Json;
using System.Text;
using Microsoft.Extensions.Caching.Memory;
using SaphiraTerror.Web.Models;

namespace SaphiraTerror.Web.Services;

public sealed class ApiClient(HttpClient http, IMemoryCache cache) : IApiClient
{
    private readonly HttpClient _http = http;
    private readonly IMemoryCache _cache = cache;

    public async Task<IReadOnlyList<(int Id, string Nome)>> GetGenerosAsync(CancellationToken ct = default)
    {
        // cache de 10 minutos
        if (_cache.TryGetValue("generos", out IReadOnlyList<(int, string)>? cached) && cached is not null)
            return cached;

        var data = await _http.GetFromJsonAsync<List<Item>>("api/generos", cancellationToken: ct)
                   ?? new List<Item>();
        var list = data.Select(x => (x.Id, x.Nome)).ToList().AsReadOnly();
        _cache.Set("generos", list, TimeSpan.FromMinutes(10));
        return list;
    }

    public async Task<IReadOnlyList<(int Id, string Nome)>> GetClassificacoesAsync(CancellationToken ct = default)
    {
        if (_cache.TryGetValue("classificacoes", out IReadOnlyList<(int, string)>? cached) && cached is not null)
            return cached;

        var data = await _http.GetFromJsonAsync<List<Item>>("api/classificacoes", cancellationToken: ct)
                   ?? new List<Item>();
        var list = data.Select(x => (x.Id, x.Nome)).ToList().AsReadOnly();
        _cache.Set("classificacoes", list, TimeSpan.FromMinutes(10));
        return list;
    }

    public async Task<PagedResult<FilmeDto>> SearchFilmesAsync(CatalogFilterVm f, CancellationToken ct = default)
    {
        var qs = BuildQuery(f);
        var res = await _http.GetFromJsonAsync<PagedResult<FilmeDto>>($"api/filmes{qs}", cancellationToken: ct)
                  ?? new PagedResult<FilmeDto>(1, 12, 0, Array.Empty<FilmeDto>());
        return res;
    }

    public Task<FilmeDto?> GetFilmeAsync(int id, CancellationToken ct = default)
        => _http.GetFromJsonAsync<FilmeDto>($"api/filmes/{id}", cancellationToken: ct);

    private static string BuildQuery(CatalogFilterVm f)
    {
        var sb = new StringBuilder($"?page={f.Page}&pageSize={f.PageSize}");
        if (f.GeneroId is int gid) sb.Append($"&generoId={gid}");
        if (f.ClassificacaoId is int cid) sb.Append($"&classificacaoId={cid}");
        if (f.Ano is int ano) sb.Append($"&ano={ano}");
        if (!string.IsNullOrWhiteSpace(f.Q)) sb.Append($"&q={Uri.EscapeDataString(f.Q)}");
        if (!string.IsNullOrWhiteSpace(f.SortBy)) sb.Append($"&sortBy={f.SortBy}");
        if (!f.Desc) sb.Append("&desc=false");
        return sb.ToString();
    }

    private record Item(int Id, string Nome);
}
