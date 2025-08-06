//-----------------------------------------------------------------------
// <copyright file="RootWithChildren.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Web.Mvc.Test.ModelBinderTest
{
  public class RootWithChildren:Csla.BusinessBase<RootWithChildren>
  {
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(p => p.ID);
    public int ID
    {
      get { return GetProperty<int>(IdProperty); }
      private set { SetProperty<int>(IdProperty, value); }
    }

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(p => p.Name);
    public string Name
    {
      get { return GetProperty<string>(NameProperty); }
      set { SetProperty<string>(NameProperty, value); }
    }
    public static readonly PropertyInfo<ChildList> ChildrenProperty = RegisterProperty<ChildList>(p => p.Children);
    public ChildList Children
    {
      get { return GetProperty<ChildList>(ChildrenProperty); }
      private set { LoadProperty<ChildList>(ChildrenProperty, value); }
    }
    public static RootWithChildren Get(int childCount)
    {
      return DataPortal.Fetch<RootWithChildren>(new SingleCriteria<RootList, int>(childCount));
    }
    private void DataPortal_Fetch(SingleCriteria<RootList, int> criteria)
    {
      using (BypassPropertyChecks)
      {
        ID = criteria.Value * 100;
        Name = "root with " + criteria.Value.ToString() + " children";
      }
      LoadProperty(ChildrenProperty, DataPortal.FetchChild<ChildList>(criteria.Value));
    }

  }
  public class ChildList : Csla.BusinessListBase<ChildList, Child>
  {
    private ChildList()
    {}

    private void Child_Fetch(int childCount)
    {
      for (int i = 0; i < childCount; i++)
      {
        this.Add(DataPortal.FetchChild<Child>(i));
      }
    }
  }
}