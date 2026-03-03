using Microsoft.AspNetCore.Mvc;
using SaphiraTerror.Application.Services;

namespace SaphiraTerror.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClassificacoesController : ControllerBase
{
    private readonly ICatalogLookupService _lookup;

    public ClassificacoesController(ICatalogLookupService lookup) => _lookup = lookup;

    [HttpGet]
    public async Task<ActionResult<object>> Get(CancellationToken ct)
    {
        var items = await _lookup.GetClassificacoesAsync(ct);
        return Ok(items.Select(x => new { x.Id, x.Nome }));
    }
}
