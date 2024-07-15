namespace LinkStand.Controllers;

public static class UrlExtensions
{
  public static string EnsureHttpPrefix(this string url, string prefix = "https") => url.StartsWith(prefix) 
    ? url 
    : $"{prefix}://{url}";
}
