using System;
using System.Collections.Generic;
using System.Text;
using Csla;

namespace WindowsApplication2
{
  [Serializable]
  public class Root : BusinessBase<Root>
  {
    private static PropertyInfo<int> IdProperty = RegisterProperty<int>(typeof(Root), new PropertyInfo<int>("Id", "Id"));
    public int Id
    {
      get { return GetProperty<int>(IdProperty); }
      set { SetProperty<int>(IdProperty, value); }
    }

    private static PropertyInfo<string> NameProperty = RegisterProperty<string>(typeof(Root), new PropertyInfo<string>("Name", "Name"));
    public string Name
    {
      get { return GetProperty<string>(NameProperty); }
      set { SetProperty<string>(NameProperty, value); }
    }

    private static PropertyInfo<ChildList> ChildrenProperty = RegisterProperty<ChildList>(typeof(Root), new PropertyInfo<ChildList>("Children", "Children"));
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
