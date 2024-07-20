using Api.Contracts;
using Api.Model;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("/")]
public class AliasController(IAliasService aliases) : ControllerBase
{
  [HttpPost]
  public async Task<IActionResult> CreateAlias([FromQuery] string url, [FromQuery] AliasType type = AliasType.Short)
  {
    Alias alias = AliasFactory.Create(url, type);
    Alias? existing = await aliases.GetAsync(alias.Id).OnAnyThread();

    return existing switch
    {
      not null => Conflict(),
      _ => await Created($"/{alias.Id}", new { id = alias.Id })
        .Chain(async () => await aliases.AddOrUpdateAsync(alias).OnAnyThread())
        .OnAnyThread(),
    };
  }

  [HttpGet("{*id}")]
  public async Task<IActionResult> GetAliasAsync(AliasId id)
  {
    Alias? alias = await aliases.GetAsync(id).OnAnyThread();

    return alias switch
    {
      null => NotFound(),
      _ => await Redirect(alias.Target.EnsureHttpPrefix())
        .Chain(async () => await AddEventAsync(alias).OnAnyThread())
        .OnAnyThread(),
    };
  }

  [HttpGet("clicks")]
  public async Task<IActionResult> GetClicks([FromQuery] AliasId id)
  {
    Alias? alias = await aliases.GetAsync(id).OnAnyThread();
    List<AliasEvent> events = await aliases.GetAllAliasEventsAsync(id).OnAnyThread();

    return alias switch
    {
      null => NotFound(),
      _ => Ok(new { clicks = events.Count }),
    };
  }

  private async Task<DataAction> AddEventAsync(Alias alias) => 
    await aliases.AddOrUpdateAsync(new AliasEvent(AliasEventId.From($"{Guid.NewGuid()}"), alias.Id, alias, Request.GetIp(), DateTime.UtcNow));
}