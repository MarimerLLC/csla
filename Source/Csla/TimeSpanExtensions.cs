namespace System;

/// <summary>
/// Provides extension methods for <see cref="TimeSpan"/>.
/// </summary>
public static class TimeSpanExtensions
{
    /// <summary>
    /// Converts the specified <see cref="TimeSpan"/> to a <see cref="CancellationToken"/> with the specified timeout.
    /// </summary>
    /// <param name="timeout">The timeout duration.</param>
    /// <returns>The <see cref="CancellationToken"/> with the specified timeout.</returns>
    public static CancellationToken ToCancellationToken(this TimeSpan timeout)
    {
        var cts = new CancellationTokenSource(timeout);
        return cts.Token;
    }
}
