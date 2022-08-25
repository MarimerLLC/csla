//-----------------------------------------------------------------------
// <copyright file="Root.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Business object type for use in tests</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Csla.Test.Server.Interceptors
{
  [Serializable()]
  public class Root : BusinessBase<Root>
  {
    public static PropertyInfo<string> DataProperty = RegisterProperty<string>(c => c.Data);
    
    [Required]
    public string Data
    {
      get { return GetProperty(DataProperty); }
      set { SetProperty(DataProperty, value); }
    }

    public static PropertyInfo<int> CreatedDomainProperty = RegisterProperty<int>(c => c.CreatedDomain);
    public int CreatedDomain
    {
      get { return GetProperty(CreatedDomainProperty); }
      private set { LoadProperty(CreatedDomainProperty, value); }
    }

    public static readonly PropertyInfo<Children> ChildrenProperty = RegisterProperty<Children>(c => c.Children);
    public Children Children
    {
      get { return GetProperty(ChildrenProperty); }
      private set { LoadProperty(ChildrenProperty, value); }
    }

    [Serializable()]
    public class Criteria
    {
      public string _data;

      public Criteria()
      {
        _data = "<new>";
      }

      public Criteria(string data)
      {
        this._data = data;
      }
    }

    [Create]
    private void DataPortal_Create(Criteria criteria, [Inject] IChildDataPortal<Children> dataPortal)
    {
      using (BypassPropertyChecks)
      {
        Data = criteria._data;
        Children = dataPortal.CreateChild();
      }
      CreatedDomain = AppDomain.CurrentDomain.Id;
    }

    [Fetch]
    protected void DataPortal_Fetch(Criteria criteria, [Inject] IChildDataPortal<Children> dataPortal)
    {
      using (BypassPropertyChecks)
      {
        Data = criteria._data;
        Children = dataPortal.CreateChild();
      }
      MarkOld();
    }

    [Insert]
    protected void DataPortal_Insert()
    {
    }

    [Update]
    protected void DataPortal_Update()
    {
      //we would update here
    }

    [DeleteSelf]
    protected void DataPortal_DeleteSelf()
    {
    }

    [Delete]
		protected void DataPortal_Delete(object criteria)
    {
    }

    protected override void OnDeserialized(System.Runtime.Serialization.StreamingContext context)
    {
    }

    protected override void DataPortal_OnDataPortalInvoke(DataPortalEventArgs e)
    {
    }

    protected override void DataPortal_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    {
    }

    internal int GetEditLevel()
    {
      return EditLevel;
    }
  }
}