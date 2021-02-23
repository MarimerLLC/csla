using System;

namespace Csla.Analyzers.IntegrationTests
{
  [Serializable]
  public sealed class PropertiesAndRegions
    : BusinessBase<PropertiesAndRegions>
  {
    // This should fail because it isn't public (even if it is hidden in a region).
    #region Properties
    private static readonly PropertyInfo<Guid> MyGuidProperty = RegisterProperty<Guid>(c => c.MyGuid);
    public Guid MyGuid => this.GetProperty(PropertiesAndRegions.MyGuidProperty);
    #endregion
  }

  [Serializable]
  public class PropertyDefinitionTests
    : CommandBase<PropertyDefinitionTests>
  {
    // This should fail because it isn't readonly
    public static PropertyInfo<int> ResourceNotReadOnlyProperty = RegisterProperty<int>(c => c.ResourceNotReadOnly);
    public int ResourceNotReadOnly
    {
      get { return ReadProperty(ResourceNotReadOnlyProperty); }
      private set { LoadProperty(ResourceNotReadOnlyProperty, value); }
    }

    // This should fail because it isn't static
    public readonly PropertyInfo<bool> ResourceNotStaticProperty = RegisterProperty<bool>(c => c.ResourceNotStatic);
    public bool ResourceNotStatic
    {
      get { return ReadProperty(ResourceNotStaticProperty); }
      private set { LoadProperty(ResourceNotStaticProperty, value); }
    }

    // This should fail because it isn't public
    private static readonly PropertyInfo<bool> ResourceNotPublicProperty = RegisterProperty<bool>(c => c.ResourceNotPublic);
    public bool ResourceNotPublic
    {
      get { return ReadProperty(ResourceNotPublicProperty); }
      private set { LoadProperty(ResourceNotPublicProperty, value); }
    }

    // This should fail because it isn't anything
    PropertyInfo<int> ResourceNotAnythingProperty = RegisterProperty<int>(c => c.ResourceNotAnything);
    public int ResourceNotAnything
    {
      get { return ReadProperty(ResourceNotAnythingProperty); }
      private set { LoadProperty(ResourceNotAnythingProperty, value); }
    }

    [Execute]
    protected override void DataPortal_Execute() { }
  }
}