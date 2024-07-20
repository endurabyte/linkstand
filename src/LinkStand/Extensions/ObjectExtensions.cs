namespace LinkStand.Extensions;

public static class ObjectExtensions
{
  public static T Chain<T>(this T t, Action<T> a)
  {
    a(t);
    return t;
  }

  public static async Task<T> Chain<T>(this T t, Func<Task> f)
  {
    await f();
    return t;
  }
}