namespace System;

/// <summary>
/// Provides extension methods for <see cref="TimeSpan"/>.
/// </summary>
internal static class TimeSpanExtensions
{
  /// <summary>
  /// Creates a <see cref="CancellationTokenSource"/> with the given <paramref name="timeout"/>.
  /// </summary>
  /// <param name="timeout">The timeout duration.</param>
  /// <returns>The <see cref="CancellationTokenSource"/> with the specified timeout.</returns>
  public static CancellationTokenSource ToCancellationTokenSource(this TimeSpan timeout) => new(timeout);
}
