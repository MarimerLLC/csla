using System;
using System.Collections.Generic;
using System.Text;
using Csla;

namespace WindowsApplication2
{
  public class Root : BusinessBase<Root>
  {
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(nameof(Id));
    public int Id
    {
      get => GetProperty(IdProperty);
      set => SetProperty(IdProperty, value);
    }

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
    public string Name
    {
      get => GetProperty(NameProperty);
      set => SetProperty(NameProperty, value);
    }

    public static readonly PropertyInfo<ChildList> RealChildrenProperty = RegisterProperty<ChildList>(nameof(RealChildren), RelationshipTypes.LazyLoad);
#nullable disable
    public ChildList RealChildren
    {
      get => LazyGetProperty(RealChildrenProperty, 
        () => ApplicationContext.GetRequiredService<IChildDataPortal<ChildList>>().CreateChild());
    }
#nullable enable

    public SortedBindingList<Child> Children
    {
      get { return new SortedBindingList<Child>(RealChildren); }
    }

    private static int _lastId;

    [Create]
    private void Create()
    {
      LoadProperty<int>(IdProperty, System.Threading.Interlocked.Increment(ref _lastId));
    }

    public void DumpEditLevels()
    {
      StringBuilder sb = new StringBuilder();
      sb.AppendFormat("{0} {1}: {2} {3}\r", this.GetType().Name, this.Id, this.EditLevel, this.BindingEdit);
      var childList = ReadProperty<ChildList>(RealChildrenProperty);
      if (childList != null)
        childList.DumpEditLevels(sb);
      else
        sb.AppendFormat("{0} null: - -\r", "ChildList", "-");
      System.Windows.Forms.MessageBox.Show(sb.ToString());
    }
  }
}
