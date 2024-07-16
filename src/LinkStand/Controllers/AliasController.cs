using Microsoft.AspNetCore.Mvc;

namespace LinkStand.Controllers;

public record Alias(string Id, string Target, AliasType Type);
public record AliasEvent(string AliasId, string Ip, DateTime Timestamp);

[ApiController]
[Route("/")]
public class AliasController(IAliasService aliases) : ControllerBase
{
  [HttpPost]
  public IActionResult CreateAlias([FromQuery] string url, [FromQuery] AliasType type = AliasType.Short)
  {
    Alias alias = AliasFactory.Create(url, type);
    
    return aliases.TryAdd(alias) switch
    {
      false => Conflict(),
      _ => Created($"/{alias.Id}", new { id = alias.Id }),
    };
  }

  [HttpGet("{*id}")]
  public IActionResult GetAlias(string id) => 
    aliases.TryGetAlias(id, out Alias? alias) switch
  {
    true when alias is not null => Redirect(alias.Target.EnsureHttpPrefix().AndDo(_ => AddEvent(id))),
    _ => NotFound()
  };

  [HttpGet("clicks")]
  public IActionResult GetClicks([FromQuery] string id) =>
    aliases.TryGetEvents(id, out List<AliasEvent>? events) switch
    {
      true when events is not null => Ok(new { clicks = events.Count}),
      _ => NotFound(),
    };

  private void AddEvent(string id)
  {
    // Get ip from Fly-Client-IP header
    string ipAddr = Request.HttpContext.Request.Headers.TryGetValue("Fly-Client-IP", out var ip) switch
    {
      true => $"{ip}",
      _ => $"{Request.HttpContext.Connection.RemoteIpAddress}",
    };
    
    aliases.Add(new AliasEvent(id, $"{ipAddr}", DateTime.Now));
  }
}