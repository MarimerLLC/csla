using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Test.LazyLoad
{
  [Serializable]
  public class AChild : Csla.BusinessBase<AChild>
  {
    public static PropertyInfo<Guid> IdProperty = RegisterProperty<Guid>(c => c.Id);
    public Guid Id
    {
      get { return GetProperty(IdProperty); }
      set { SetProperty(IdProperty, value); }
    }

    public int EditLevel
    {
      get { return base.EditLevel; }
    }

    public AChild()
    {
      MarkAsChild();
      using (BypassPropertyChecks)
        Id = Guid.NewGuid();
    }
  }
}
