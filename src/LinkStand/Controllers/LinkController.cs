using Microsoft.AspNetCore.Mvc;

namespace LinkStand.Controllers;

[ApiController]
[Route("/")]
public class LinkController(IUrlMapService urls) : ControllerBase
{
  private static readonly string[] Nouns = ["car", "grave", "tree", "sky", "ocean"];
  private static readonly string[] Verbs = ["paints", "sings", "runs", "flies", "writes"];
  private static readonly string[] Adjectives = ["beautiful", "strong", "bright", "dark", "quiet"];
  private static readonly string[] Adverbs = ["quickly", "silently", "intentionally", "happily", "sadly"];

  [HttpPost]
  public IActionResult CreateShortUrl([FromQuery] string url)
  {
    string urlAlias = GetRandomUrl();

    urls.AddMapping(urlAlias, url);

    return Ok(new { alias = urlAlias });
  }

  private static string GetRandomUrl() => $"{Nouns[Random.Shared.Next(Nouns.Length)]}-"
      + $"{Verbs[Random.Shared.Next(Verbs.Length)]}-"
      + $"{Adjectives[Random.Shared.Next(Adjectives.Length)]}-"
      + $"{Nouns[Random.Shared.Next(Nouns.Length)]}-"
      + $"{Adverbs[Random.Shared.Next(Adverbs.Length)]}";

  [HttpGet("{*alias}")]
  public IActionResult GetOriginalUrl(string alias) => 
    urls.TryGetOriginalUrl(alias, out string? originalUrl) switch
  {
    true when originalUrl is not null => Redirect(originalUrl),
    _ => NotFound()
  };
}
