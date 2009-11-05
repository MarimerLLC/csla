using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Test.LazyLoad
{
  [Serializable]
  public class AChild : Csla.BusinessBase<AChild>
  {
    private Guid _id;
    public Guid Id
    {
      get { return _id; }
      set
      {
        _id = value;
        PropertyHasChanged();
      }
    }

    public int EditLevel
    {
      get { return base.EditLevel; }
    }

    protected override object GetIdValue()
    {
      return _id;
    }

    public AChild()
    {
      MarkAsChild();
      _id = Guid.NewGuid();
    }
  }
}
