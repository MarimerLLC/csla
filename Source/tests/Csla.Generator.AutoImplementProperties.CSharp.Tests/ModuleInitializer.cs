using System.Runtime.CompilerServices;

namespace Csla.Generator.Tests
{
  public static class ModuleInitializer
  {
    [ModuleInitializer]
    public static void Init()
    {
      VerifySourceGenerators.Initialize();
    }
  }
}