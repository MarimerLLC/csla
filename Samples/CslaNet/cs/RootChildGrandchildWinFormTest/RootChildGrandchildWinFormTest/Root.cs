using System;
using System.Collections.Generic;
using System.Text;
using Csla;

namespace WindowsApplication2
{
  [Serializable]
  public class Root : BusinessBase<Root>
  {
    public static PropertyInfo<int> IdProperty = RegisterProperty<int>(p => p.Id);
    public int Id
    {
      get { return GetProperty<int>(IdProperty); }
      set { SetProperty<int>(IdProperty, value); }
    }

    public static PropertyInfo<string> NameProperty = RegisterProperty<string>(p => p.Name);
    public string Name
    {
      get { return GetProperty<string>(NameProperty); }
      set { SetProperty<string>(NameProperty, value); }
    }

    public static PropertyInfo<ChildList> ChildrenProperty = RegisterProperty<ChildList>(p => p.Children, RelationshipTypes.LazyLoad);
    public ChildList Children
    {
      get
      {
        if (!FieldManager.FieldExists(ChildrenProperty))
          LoadProperty<ChildList>(ChildrenProperty, new ChildList());
        return GetProperty<ChildList>(ChildrenProperty);
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
      var childList = ReadProperty<ChildList>(ChildrenProperty);
      if (childList != null)
        childList.DumpEditLevels(sb);
      else
        sb.AppendFormat("{0} null: - -\r", "ChildList", "-");
      System.Windows.Forms.MessageBox.Show(sb.ToString());
    }
  }
}
