using Microsoft.AspNetCore.Mvc;
using SaphiraTerror.Application.Services;

namespace SaphiraTerror.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GenerosController : ControllerBase
{
    private readonly ICatalogLookupService _lookup;

    public GenerosController(ICatalogLookupService lookup) => _lookup = lookup;

    [HttpGet]
    public async Task<ActionResult<object>> Get(CancellationToken ct)
    {
        var items = await _lookup.GetGenerosAsync(ct);
        // Tupla -> objeto forte (Id, Nome)
        return Ok(items.Select(x => new { x.Id, x.Nome }));
    }
}
