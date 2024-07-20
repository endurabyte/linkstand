namespace LinkStand.Controllers;

public static class RequestExtensions
{
  public static string GetIp(this HttpRequest req) =>
    // Get ip from Fly-Client-IP header, or fallback to connection remote ip
    req.HttpContext.Request.Headers.TryGetValue("Fly-Client-IP", out var ip) switch
    {
      true => $"{ip}",
      _ => $"{req.HttpContext.Connection.RemoteIpAddress}",
    };
}