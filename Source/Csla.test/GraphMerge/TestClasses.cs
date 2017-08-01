﻿using System;
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
    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
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

    public void AddChild()
    {
      var child = Csla.DataPortal.Create<Foo>();
      child.MarkAsChild();
      LoadProperty(ChildProperty, child);
    }

    public void MockUpdated()
    {
      MarkOld();
    }

    public void MarkForDelete()
    {
      MarkDeleted();
    }

    public void MockDeleted()
    {
      MarkNew();
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRules.AddRule(new NoTwo { PrimaryProperty = NameProperty });
    }

    private class NoTwo : Csla.Rules.BusinessRule
    {
      protected override void Execute(RuleContext context)
      {
        var target = (Foo)context.Target;
        if (target.Name == "2")
          context.AddErrorResult("Name can not be 2");
      }
    }
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
