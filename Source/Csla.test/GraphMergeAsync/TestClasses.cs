using Csla.Rules;

namespace Csla.Test.GraphMergeAsync
{
  [Serializable]
  public class Foo : BusinessBase<Foo>
  {
    public Foo()
    {
      ChildList = new FooList();
    }

    public static readonly PropertyInfo<string> NameProperty =
      RegisterProperty<string>(c => c.Name);
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    public static readonly PropertyInfo<Foo> ChildProperty = RegisterProperty<Foo>(c => c.Child);
    public Foo Child
    {
      get { return GetProperty(ChildProperty); }
      set { SetProperty(ChildProperty, value); }
    }

    public static readonly PropertyInfo<FooList> ChildListProperty = RegisterProperty<FooList>(c => c.ChildList);
    public FooList ChildList
    {
      get { return GetProperty(ChildListProperty); }
      private set { LoadProperty(ChildListProperty, value); }
    }

    public void AddChild(IDataPortal<Foo> dataPortal)
    {
      var child = dataPortal.Create();
      child.MarkAsChild();
      LoadProperty(ChildProperty, child);
    }

    public void MockUpdated()
    {
      MarkOld();
      Child?.MockUpdated();
      ChildList.MockUpdated();
    }

    public void MarkForDelete()
    {
      MarkDeleted();
      Child?.MarkForDelete();
      ChildList.Clear();
    }

    public void MockDeleted()
    {
      MarkNew();
      Child?.MockDeleted();
      ChildList.MockDeleted();
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRules.AddRule(new NoTwo { PrimaryProperty = NameProperty });
    }

    private class NoTwo : BusinessRule
    {
      protected override void Execute(IRuleContext context)
      {
        var target = (Foo)context.Target;
        if (target.Name == "2")
          context.AddErrorResult("Name can not be 2");
      }
    }

    [Create]
    private void Create([Inject] IChildDataPortal<FooList> childDataPortal)
    {
      LoadProperty(ChildListProperty, childDataPortal.CreateChild());
      BusinessRules.CheckRulesAsync();
    }

    [InsertChild]
    private void Child_Insert()
    { }

    [UpdateChild]
    private void Child_Update()
    { }

    [DeleteSelfChild]
    private void Child_DeleteSelf()
    { }
  }

  [Serializable]
  public class FooList : BusinessListBase<FooList, Foo>
  {
    public void MockUpdated()
    {
      foreach (var item in this)
        item.MockUpdated();
      DeletedList.Clear();
    }

    internal void MockDeleted()
    {
      Clear();
      DeletedList.Clear();
    }

    [Create]
    private void Create()
    {
    }

    [Update]
    protected void DataPortal_Update()
    {
      base.Child_Update();
    }
  }

  [Serializable]
  public class FooBindingList : BusinessBindingListBase<FooBindingList, Foo>
  {
    [Create]
    private void DataPortal_Create()
    {

    }
  }

  [Serializable]
  public class FooDynamicList : DynamicListBase<Foo>
  {
    private void DataPortal_Create()
    { }
  }

  [Serializable]
  public class FooDynamicBindingList : DynamicBindingListBase<Foo>
  {
    private void DataPortal_Create()
    { }
  }
}
