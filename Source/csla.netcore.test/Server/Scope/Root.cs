using System;
using System.Collections.Generic;
using System.Text;
using Csla;
using Csla.Runtime;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace Csla.Test.Server.Scope
{
  [Serializable]
  public class Root : BusinessBase<Root>
  {
    public static readonly PropertyInfo<Guid> GuidProperty = RegisterProperty<Guid>(nameof(Guid));
    public Guid Guid
    {
      get => GetProperty(GuidProperty);
      set => SetProperty(GuidProperty, value);
    }

    public GuidProvider GuidProvider { get; set; }

    [Create]
    private void Create()
    {
      var gp = ApplicationContext.GetRequiredService<GuidProvider>();
      GuidProvider = gp;
      Guid = gp.Guid;
    }
  }
}
