//-----------------------------------------------------------------------
// <copyright file="SingleItem.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Security;
using Csla.Core;
using Csla.Serialization;

namespace Csla.Testing.Business.EditableRootListTests
{
  [Serializable]
  public class SingleItem : BusinessBase<SingleItem>
  {
    private SingleItem() { }

    public static SingleItem GetSingleItem()
    {
      return new SingleItem();
    }

    private static PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id, "Internal Id", 0);
    public int Id
    {
      get
      {
        return GetProperty<int>(IdProperty);
      }
      set
      {
        SetProperty<int>(IdProperty,value);
      }
    }

    private static PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name, "Item Name", "");
    public string Name
    {
      get
      {
        return GetProperty<string>(NameProperty);
      }
      set
      {
        SetProperty<string>(NameProperty, value);
      }
    }

    private static PropertyInfo<SmartDate> DateCreatedProperty = RegisterProperty<SmartDate>(c => c.DateCreated, "Date Created On");
    public string DateCreated
    {
      get
      {
        return GetProperty<SmartDate>(DateCreatedProperty).Text;
      }
      set
      {
        SmartDate test=new SmartDate();
        if (SmartDate.TryParse(value, ref test) == true)
        {
          SetProperty<SmartDate>(DateCreatedProperty, test);
        }
      }
    }

    private static PropertyInfo<string> MethodCalledProperty = RegisterProperty<string>(c => c.MethodCalled, "MethodCalled", "");
    public string MethodCalled
    {
      get
      {
        return GetProperty<string>(MethodCalledProperty);
      }
      set
      {
        SetProperty<string>(MethodCalledProperty, value);
      }
    }

    internal static SingleItem GetSingleItem(int id, string name, DateTime createdOn)
    {
      SingleItem newItem = new SingleItem();
      newItem.LoadProperty(IdProperty, id);
      newItem.LoadProperty(NameProperty, name);
      newItem.LoadProperty(DateCreatedProperty, new SmartDate(createdOn));
      //newItem.MarkAsChild();  Leave this off to allow deletion
      newItem.MarkOld();
      return newItem;
    }

    protected override void DataPortal_DeleteSelf()
    {
      if (!IsNew)
        MethodCalled = "DataPortal_DeleteSelf";
    }

    protected override void DataPortal_Insert()
    {
      MethodCalled = "DataPortal_Insert";
    }
    protected override void DataPortal_Update()
    {
      MethodCalled = "DataPortal_Update";
    }
    protected void DataPortal_Delete(object criteria)
    {
      if (!IsNew)
        MethodCalled = "DataPortal_Delete";
    }
  }
}