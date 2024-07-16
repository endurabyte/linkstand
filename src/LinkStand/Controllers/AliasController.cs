using Microsoft.AspNetCore.Mvc;

namespace LinkStand.Controllers;

public record Alias(string Id, string Target, AliasType Type);

[ApiController]
[Route("/")]
public class AliasController(IAliasService urls) : ControllerBase
{
  [HttpPost]
  public IActionResult CreateAlias([FromQuery] string url, [FromQuery] AliasType type = AliasType.Short)
  {
    Alias alias = AliasFactory.Create(url, type);
    
    urls.Add(alias);

    return Ok(new { id = alias.Id });
  }

  [HttpGet("{*id}")]
  public IActionResult GetTargetUrl(string id) => 
    urls.TryGet(id, out Alias? alias) switch
  {
    true when alias is not null => Redirect(alias.Target.EnsureHttpPrefix()),
    _ => NotFound()
  };
}