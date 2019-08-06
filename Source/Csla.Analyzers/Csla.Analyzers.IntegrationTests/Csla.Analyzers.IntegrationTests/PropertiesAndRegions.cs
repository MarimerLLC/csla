using System;

namespace Csla.Analyzers.IntegrationTests
{
  [Serializable]
  public sealed class PropertiesAndRegions
    : BusinessBase<PropertiesAndRegions>
  {
    #region Properties
    private static readonly PropertyInfo<Guid> MyGuidProperty = RegisterProperty<Guid>(c => c.MyGuid);
    public Guid MyGuid => this.GetProperty(PropertiesAndRegions.MyGuidProperty);
    #endregion
  }
}
