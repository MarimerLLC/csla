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

    private void DataPortal_Fetch()
    {
      CreatedFrom = $"{Environment.MachineName} - {Csla.ApplicationContext.LocalContext["dpv"].ToString()}";
    }
  }
}
