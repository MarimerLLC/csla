using System;

namespace Csla.Analyzers.IntegrationTests
{
  [Serializable]
  public sealed class RefAndOutOperationParameters
    : BusinessBase<RefAndOutOperationParameters>
  {
    public RefAndOutOperationParameters() { }

    // The "b" and "c" parameters should have errors because they are
    // out and ref, respectively.
    [Fetch]
    private void Fetch(string a, out string b, ref string c) 
    {
      b = string.Empty;
    }
  }
}
