using System;

namespace Csla.Analyzers.IntegrationTests
{
  [Serializable]
  public partial class PartialClass
    : BusinessBase<PartialClass>
  {
    public static readonly PropertyInfo<string> DataProperty = RegisterProperty<string>(p => p.Data);
    public string Data => GetProperty(DataProperty);
  }
}
