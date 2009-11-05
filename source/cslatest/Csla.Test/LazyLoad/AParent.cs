using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Test.LazyLoad
{
  [Serializable]
  public class AParent : Csla.BusinessBase<AParent>
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

    private static PropertyInfo<AChildList> ChildListProperty = 
      RegisterProperty<AChildList>(typeof(AParent), new PropertyInfo<AChildList>("ChildList", "Child list"));
    public AChildList ChildList
    {
      get 
      {
        if (!FieldManager.FieldExists(ChildListProperty))
          LoadProperty<AChildList>(ChildListProperty, new AChildList());
        return GetProperty<AChildList>(ChildListProperty); 
      }
    }

    //private AChildList _children;
    //public AChildList ChildList
    //{
    //  get 
    //  {
    //    if (_children == null)
    //    {
    //      _children = new AChildList();
    //      for (int count = 0; count < EditLevel; count++)
    //        ((Csla.Core.IUndoableObject)_children).CopyState(EditLevel);
    //    }
    //    return _children; 
    //  }
    //}

    public AChildList GetChildList()
    {
      if (FieldManager.FieldExists(ChildListProperty))
        return ReadProperty<AChildList>(ChildListProperty);
      else
        return null;
      //return _children;
    }

    public int EditLevel
    {
      get { return base.EditLevel; }
    }

    protected override object GetIdValue()
    {
      return _id;
    }

    public AParent()
    {
      _id = Guid.NewGuid();
    }
  }
}
