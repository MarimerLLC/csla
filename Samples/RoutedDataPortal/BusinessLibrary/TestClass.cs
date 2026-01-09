using Csla;

namespace BusinessLibrary
{
  [CslaImplementProperties]
  public partial class TestClass : ReadOnlyBase<TestClass>
  {
    public partial string? CreatedFrom { get; private set; }

    [Fetch]
    private void Fetch()
    {
      CreatedFrom = $"{Environment.MachineName} - {ApplicationContext.LocalContext["dpv"]?.ToString()}";
    }
  }
}
