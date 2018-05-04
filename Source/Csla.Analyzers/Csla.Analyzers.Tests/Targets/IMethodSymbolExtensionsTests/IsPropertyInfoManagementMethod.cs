namespace Csla.Analyzers.Tests.Targets.IMethodSymbolExtensionsTests
{
  public class PropertyInfoManagementMethods
    : BusinessBase<PropertyInfoManagementMethods>
  {
    public void AMethod()
    {
      this.GetProperty(null);
      this.GetPropertyConvert<string, string>(null, null);
      this.SetProperty(null, null);
      this.SetPropertyConvert<string, string>(null, null);
      this.LoadProperty(null, null);
      this.LoadPropertyAsync<string>(null, null);
      this.LoadPropertyConvert<string, string>(null, null);
      this.LoadPropertyMarkDirty(null, null);
      this.ReadProperty(null);
      this.ReadPropertyConvert<string, string>(null);
      this.LazyGetProperty<string>(null, null);
      this.LazyGetPropertyAsync<string>(null, null);
      this.LazyReadProperty<string>(null, null);
      this.LazyReadPropertyAsync<string>(null, null);
      this.Something();
    }

    private void Something() { }
  }
}  
