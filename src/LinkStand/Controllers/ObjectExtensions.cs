namespace LinkStand.Controllers;

public static class ObjectExtensions
{
  public static T AndDo<T>(this T t, Action<T> a)
  {
    a(t);
    return t;
  }
}