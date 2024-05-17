namespace Csla.Test.DataPortal
{
  internal class PrincipalInfo : ReadOnlyBase<PrincipalInfo>
  {
    public static PropertyInfo<bool> IsAuthenticatedProperty = RegisterProperty<bool>(nameof(IsAuthenticated));
    public static PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));

    public bool IsAuthenticated
    {
      get => GetProperty(IsAuthenticatedProperty);
      private set => LoadProperty(IsAuthenticatedProperty, value);
    }

    public string Name 
    { 
      get => GetProperty(NameProperty);
      private set => LoadProperty(NameProperty, value);
    }

    [Fetch]
    private void Fetch()
    {
      IsAuthenticated = ApplicationContext.Principal.Identity?.IsAuthenticated ?? false;
      Name = ApplicationContext.Principal.Identity?.Name ?? string.Empty;
    }
  }
}
