using LinkStand.Extensions;

namespace LinkStand.Model;

public static class AliasFactory
{
  private static readonly string[] Nouns = ["car", "grave", "tree", "sky", "ocean"];
  private static readonly string[] Verbs = ["paints", "sings", "runs", "flies", "writes"];
  private static readonly string[] Adjectives = ["beautiful", "strong", "bright", "dark", "quiet"];
  private static readonly string[] Adverbs = ["quickly", "silently", "intentionally", "happily", "sadly"];

  public static Alias Create(string url, AliasType type) => new(GetAliasId(url, type), url, type, []);

  private static AliasId GetAliasId(string url, AliasType type) => type switch
  {
    AliasType.None => AliasId.From(url),
    AliasType.Memorable => GetMemorableUrl(),
    _ => GetShortUrl(),
  };

  private static AliasId GetShortUrl() => AliasId.From($"{Guid.NewGuid()}".Sha1()[..7]);

  private static AliasId GetMemorableUrl() => AliasId.From(
      $"{Nouns[Random.Shared.Next(Nouns.Length)]}-"
    + $"{Verbs[Random.Shared.Next(Verbs.Length)]}-"
    + $"{Adjectives[Random.Shared.Next(Adjectives.Length)]}-"
    + $"{Nouns[Random.Shared.Next(Nouns.Length)]}-"
    + $"{Adverbs[Random.Shared.Next(Adverbs.Length)]}");
}