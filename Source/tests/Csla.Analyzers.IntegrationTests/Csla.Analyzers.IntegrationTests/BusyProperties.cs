using System;

namespace Csla.Analyzers.IntegrationTests
{
  [Serializable]
  public sealed class BusyProperties
    : BusinessBase<BusyProperties>
  {
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id);
    public int Id
    {
      get
      {
        var x = 42;
        return ReadProperty(IdProperty) + x;
      }
      private set { LoadProperty(IdProperty, value); }
    }
  }
}
