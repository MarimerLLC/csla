using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Csla;

namespace Polymorphism.Business
{
  [Serializable]
  // Common base class with common properties 
  public class ChildType<T> : BusinessBase<T>, IChild where T : ChildType<T>, IChild
  {
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(p => p.Id);
    public int Id
    {
      get { return GetProperty(IdProperty); }
      set { SetProperty(IdProperty, value); }
    }

    // example with managed backing field
    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(p => p.Name);
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    protected static void AddObjectAuthorizationRules()
    {
      Debug.Print("ChildType<T> - AddObjectAuthorizationRules");
    }
  }
}
