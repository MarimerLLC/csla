using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;

namespace Csla.Test.PropertyGetSet
{
  [Serializable]
  public class EditableGetSetBase<T> : EditableGetSetTopBase<T> 
    where T: EditableGetSetBase<T>
  {
    private static int _dummy;

    public EditableGetSetBase()
    {
      _dummy = 0;
    }

    private static PropertyInfo<string> BaseProperty = RegisterProperty<string>(typeof(EditableGetSetBase<T>), new PropertyInfo<string>("Base", "Base"));
    public string Base
    {
      get { return GetProperty<string>(BaseProperty); }
      set { SetProperty<string>(BaseProperty, value); }
    }
  }

  [Serializable]
  public class EditableGetSetNFIBase<T> : EditableGetSetTopNFIBase<T>
    where T : EditableGetSetNFIBase<T>
  {
    public EditableGetSetNFIBase()
    {
    }

    public static PropertyInfo<string> BaseProperty = RegisterProperty<string>(new PropertyInfo<string>("Base", "Base"));
    public string Base
    {
      get { return GetProperty<string>(BaseProperty); }
      set { SetProperty<string>(BaseProperty, value); }
    }
  }
}
