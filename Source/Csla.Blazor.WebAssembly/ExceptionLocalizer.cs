using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Csla.Blazor.State;
using Csla.Properties;
using Csla.State;

namespace Csla.Blazor.WebAssembly;

internal static class ExceptionLocalizer
{
  [StackTraceHidden]
  public static void ThrowIfNullSessionNotRetrieved([NotNull] Session? session)
  {
    if (session is not null)
    {
      return;
    }

    Throw();

    [DoesNotReturn, StackTraceHidden]
    static void Throw()
    {
      const string SessionRetrievalHint = $"await {nameof(StateManager)}.{nameof(StateManager.InitializeAsync)}()";
      throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.WasmApplicationContextManagerSessionNotRetrieved, SessionRetrievalHint));
    }
  }
}
