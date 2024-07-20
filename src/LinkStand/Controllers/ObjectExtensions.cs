namespace LinkStand.Controllers;

public static class ObjectExtensions
{
  public static T Chain<T>(this T t, Action<T> a)
  {
    a(t);
    return t;
  }
}