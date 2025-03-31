namespace System;

/// <summary>
/// Provides extension methods for <see cref="TimeSpan"/>.
/// </summary>
[Obsolete("The extensions type will become internal with CSLA 10.")]
public static class TimeSpanExtensions
{
  /// <summary>
  /// Converts the specified <see cref="TimeSpan"/> to a <see cref="CancellationToken"/> with the specified timeout.
  /// </summary>
  /// <param name="timeout">The timeout duration.</param>
  /// <returns>The <see cref="CancellationToken"/> with the specified timeout.</returns>
  [Obsolete($"This extension method will be removed with CSLA 10. You should write your own extension with proper disposal handling for the underlying {nameof(CancellationTokenSource)}.")]
  public static CancellationToken ToCancellationToken(this TimeSpan timeout)
  {
    var cts = new CancellationTokenSource(timeout);
    return cts.Token;
  }

  /// <summary>
  /// Creates a <see cref="CancellationTokenSource"/> with the given <paramref name="timeout"/>.
  /// </summary>
  /// <param name="timeout">The timeout duration.</param>
  /// <returns>The <see cref="CancellationTokenSource"/> with the specified timeout.</returns>
  public static CancellationTokenSource ToCancellationTokenSource(this TimeSpan timeout) => new(timeout);
}
