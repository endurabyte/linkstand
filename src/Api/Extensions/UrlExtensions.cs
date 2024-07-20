namespace Api.Extensions;

public static class UrlExtensions
{
  public static string EnsureHttpPrefix(this string url, string prefix = "https") => url.StartsWith(prefix)
    ? url
    : $"{prefix}://{url}";
}

public static class TaskExtensions
{
  /// <summary>
  /// Shorthand for ConfigureAwait(false)
  /// </summary>
  public static Task OnAnyThread(this Task t) => t.WithContext(AsyncContext.Any);

  /// <summary>
  /// Shorthand for ConfigureAwait(false)
  /// </summary>
  public static Task<T> OnAnyThread<T>(this Task<T> t) => t.WithContext(AsyncContext.Any);

  private static Task WithContext(this Task t, AsyncContext context)
  {
    t.ConfigureAwait(continueOnCapturedContext: context == AsyncContext.Captured);
    return t;
  }

  private static Task<T> WithContext<T>(this Task<T> t, AsyncContext context)
  {
    t.ConfigureAwait(continueOnCapturedContext: context == AsyncContext.Captured);
    return t;
  }

  private enum AsyncContext
  {
    Captured,
    Any
  }
}
