namespace LinkStand.Controllers;

public interface IUrlMapService
{
  void AddMapping(string shortenedUrl, string originalUrl);
  bool TryGetOriginalUrl(string shortenedUrl, out string? originalUrl);
}

public class UrlMapService : IUrlMapService
{
  private readonly Dictionary<string, string> _mappings = [];

  public void AddMapping(string shortenedUrl, string originalUrl) => _mappings[shortenedUrl] = originalUrl;

  public bool TryGetOriginalUrl(string shortenedUrl, out string? originalUrl) =>
    _mappings.TryGetValue(shortenedUrl, out originalUrl);
}