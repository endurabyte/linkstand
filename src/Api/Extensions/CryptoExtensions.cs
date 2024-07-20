using System.Security.Cryptography;

namespace Api.Extensions;

public static class CryptoExtensions
{
  private static readonly SHA1 _sha1 = SHA1.Create();

  public static string Sha1(this string text)
  {
    byte[] data = System.Text.Encoding.ASCII.GetBytes(text);
    byte[] hash = data.Sha1();

    return string.Join($"", hash.Select(b => $"{b:x2}"));
  }

  public static byte[] Sha1(this byte[] data) => _sha1.ComputeHash(data);
}