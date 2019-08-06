using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csla;
using Csla.Rules;

namespace Csla.Test.GraphMerge
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

    public void AddChild()
    {
      var child = Csla.DataPortal.Create<Foo>();
      child.MarkAsChild();
      LoadProperty(ChildProperty, child);
    }

    public void MockUpdated()
    {
      MarkOld();
      if (Child != null)
        Child.MockUpdated();
      ChildList.MockUpdated();
    }

    public void MarkForDelete()
    {
      MarkDeleted();
      if (Child != null)
        Child.MarkForDelete();
      ChildList.Clear();
    }

    public void MockDeleted()
    {
      MarkNew();
      if (Child != null)
        Child.MockDeleted();
      ChildList.MockDeleted();
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRules.AddRule(new NoTwo { PrimaryProperty = NameProperty });
    }

    private class NoTwo : Csla.Rules.BusinessRule
    {
      protected override void Execute(IRuleContext context)
      {
        var target = (Foo)context.Target;
        if (target.Name == "2")
          context.AddErrorResult("Name can not be 2");
      }
    }

    private void Child_Insert()
    { }

    private void Child_Update()
    { }

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

    protected override void DataPortal_Update()
    {
      base.Child_Update();
    }
  }

  [Serializable]
  public class FooBindingList : BusinessBindingListBase<FooBindingList, Foo>
  { }

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
