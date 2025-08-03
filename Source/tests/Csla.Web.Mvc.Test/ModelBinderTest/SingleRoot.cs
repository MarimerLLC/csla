//-----------------------------------------------------------------------
// <copyright file="SingleRoot.cs" company="Marimer LLC">
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
  [Serializable()]
  public class SingleRoot : Csla.BusinessBase<SingleRoot>
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
    public static readonly PropertyInfo<SmartDate> DOBProperty = RegisterProperty<SmartDate>(p => p.DOB);
    public SmartDate DOB
    {
      get { return GetProperty<SmartDate>(DOBProperty); }
      set { SetProperty<SmartDate>(DOBProperty, value); }
    }
    private SingleRoot()
    {}

    public static SingleRoot GetSingleRoot(int id)
    {
      return DataPortal.Fetch<SingleRoot>(new SingleCriteria<SingleRoot, int>(id));
    }
    protected override void DataPortal_Create()
    {
      ID = 1;
    }
    private void DataPortal_Fetch(SingleCriteria<SingleRoot, int> criteria)
    {
      using (BypassPropertyChecks)
      {
        ID = criteria.Value;
        Name = "Name" + criteria.Value.ToString();
        DOB = new SmartDate(DateTime.Today);
      }
    }
  }
}