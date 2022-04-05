using Csla;
using System;

namespace BusinessLibrary
{
  [Serializable]
  public class TestClass : ReadOnlyBase<TestClass>
  {
    public static readonly PropertyInfo<string> CreatedFromProperty = RegisterProperty<string>(c => c.CreatedFrom);
    public string CreatedFrom
    {
      get { return GetProperty(CreatedFromProperty); }
      private set { LoadProperty(CreatedFromProperty, value); }
    }

    [Fetch]
    private void Fetch()
    {
      CreatedFrom = $"{Environment.MachineName} - {ApplicationContext.LocalContext["dpv"].ToString()}";
    }
  }
}
