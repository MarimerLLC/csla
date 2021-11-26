//-----------------------------------------------------------------------
// <copyright file="Root.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Test.Basic
{
  [Serializable()]
  public class Root : BusinessBase<Root>
  {
    public static PropertyInfo<string> DataProperty = RegisterProperty<string>(c => c.Data);
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
      TestResults.Add("Root", "Created");
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
      TestResults.Add("Root", "Fetched");
    }

    [Insert]
    protected void DataPortal_Insert()
    {
      TestResults.Add("Root", "Inserted");
    }

    [Update]
    protected void DataPortal_Update()
    {
      //we would update here
      TestResults.Add("Root", "Updated");
    }

    [DeleteSelf]
    protected void DataPortal_DeleteSelf()
    {
      TestResults.Add("Root", "Deleted self");
    }

    [Delete]
		protected void DataPortal_Delete(object criteria)
    {
      TestResults.Add("Root", "Deleted");
    }

    protected override void OnDeserialized(System.Runtime.Serialization.StreamingContext context)
    {
      TestResults.Add("Deserialized", "root Deserialized");
    }

    protected override void DataPortal_OnDataPortalInvoke(DataPortalEventArgs e)
    {
      TestResults.Add("dpinvoke", "Invoked");
    }

    protected override void DataPortal_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    {
      TestResults.Add("dpinvokecomplete", "InvokeCompleted");
    }

    internal int GetEditLevel()
    {
      return EditLevel;
    }
  }
}