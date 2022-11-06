using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csla.Test.DataPortal
{
  internal class PrincipalInfo : ReadOnlyBase<PrincipalInfo>
  {
    public static PropertyInfo<bool> IsAuthenticatedProperty = RegisterProperty<bool>(nameof(IsAuthenticated));
    public static PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));

    public bool IsAuthenticated
    {
      get { return GetProperty(IsAuthenticatedProperty); }
      private set { LoadProperty(IsAuthenticatedProperty, value); }
    }

    public string Name 
    { 
      get { return GetProperty(NameProperty); }
      private set { LoadProperty(NameProperty, value); } 
    }

    [Fetch]
    private void Fetch()
    {
      IsAuthenticated = ApplicationContext.Principal.Identity?.IsAuthenticated ?? false;
      Name = ApplicationContext.Principal.Identity?.Name ?? string.Empty;
    }
  }
}
