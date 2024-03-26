using Microsoft.AspNetCore.Components;

namespace ProjectTracker.Blazor
{
  public class RenderModeProvider(ActiveCircuitState activeCircuitState)
  {
    public RenderModes GetRenderMode(ComponentBase page)
    {
      RenderModes result;
      var isBrowser = OperatingSystem.IsBrowser();
      if (isBrowser)
        result = RenderModes.WasmInteractive;
      else if (activeCircuitState.CircuitExists)
        result = RenderModes.ServerInteractive;
      else if (page.GetType().GetCustomAttributes(typeof(StreamRenderingAttribute), true).Length > 0)
        result = RenderModes.ServerStaticStreaming;
      else
        result = RenderModes.ServerStatic;
      return result;
    }
  }
}
