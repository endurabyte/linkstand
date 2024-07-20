using Microsoft.AspNetCore.Mvc;

namespace LinkStand.Controllers;

public record Alias(AliasId Id, string Target, AliasType Type);
public record AliasEvent(AliasId AliasId, string Ip, DateTime Timestamp);

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
  public IActionResult GetAlias(AliasId id) =>
    aliases.TryGetAlias(id, out Alias? alias) switch
    {
      true when alias is not null => Redirect(alias.Target.EnsureHttpPrefix().Chain(_ => AddEvent(id))),
      _ => NotFound()
    };

  [HttpGet("clicks")]
  public IActionResult GetClicks([FromQuery] AliasId id) =>
    aliases.TryGetEvents(id, out List<AliasEvent>? events) switch
    {
      true when events is not null => Ok(new { clicks = events.Count }),
      _ => NotFound(),
    };

  private void AddEvent(AliasId id)
  {
    aliases.Add(new AliasEvent(id, Request.GetIp(), DateTime.Now));
  }
}