using System;
using System.Collections.Generic;
using System.Text;
using Csla;

namespace WindowsApplication2
{
  [Serializable]
  public class Root : BusinessBase<Root>
  {
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(p => p.Id);
    public int Id
    {
      get { return GetProperty<int>(IdProperty); }
      set { SetProperty<int>(IdProperty, value); }
    }

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(p => p.Name);
    public string Name
    {
      get { return GetProperty<string>(NameProperty); }
      set { SetProperty<string>(NameProperty, value); }
    }

    public SortedBindingList<Child> Children
    {
      get { return new SortedBindingList<Child>(RealChildren); }
    }

    public static readonly PropertyInfo<ChildList> RealChildrenProperty = RegisterProperty<ChildList>(p => p.RealChildren, RelationshipTypes.LazyLoad);
    public ChildList RealChildren
    {
      get
      {
        if (!FieldManager.FieldExists(RealChildrenProperty))
          LoadProperty<ChildList>(RealChildrenProperty, new ChildList());
        return GetProperty<ChildList>(RealChildrenProperty);
      }
    }

    protected override object GetIdValue()
    {
      return ReadProperty<int>(IdProperty);
    }

    public void DumpEditLevels()
    {
      StringBuilder sb = new StringBuilder();
      sb.AppendFormat("{0} {1}: {2} {3}\r", this.GetType().Name, this.GetIdValue().ToString(), this.EditLevel, this.BindingEdit);
      var childList = ReadProperty<ChildList>(RealChildrenProperty);
      if (childList != null)
        childList.DumpEditLevels(sb);
      else
        sb.AppendFormat("{0} null: - -\r", "ChildList", "-");
      System.Windows.Forms.MessageBox.Show(sb.ToString());
    }
  }
}
