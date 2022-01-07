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
    private Children _children = Csla.Test.Basic.Children.NewChildren();

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

    public Children Children
    {
      get { return _children; }
    }

    ///start editing
    ///
    public override bool IsDirty
    {
      get
      {
        return base.IsDirty || _children.IsDirty;
      }
    }

    [Serializable()]
    internal class Criteria
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

    private void DataPortal_Create(object criteria)
    {
      Criteria crit = (Criteria)(criteria);
      using (BypassPropertyChecks)
        Data = crit._data;
      CreatedDomain = AppDomain.CurrentDomain.Id;
      TestResults.Add("Root", "Created");
    }

    protected void DataPortal_Fetch(object criteria)
    {
      Criteria crit = (Criteria)(criteria);
      using (BypassPropertyChecks)
        Data = crit._data;
      MarkOld();
      TestResults.Add("Root", "Fetched");
    }

    [Insert]
    protected void DataPortal_Insert()
    {
      TestResults.Add("clientcontext",
          ApplicationContext.ClientContext["clientcontext"]?.ToString());

      TestResults.Overwrite("globalcontext",
        TestResults.GetResult("globalcontext"));

      TestResults.Overwrite("globalcontext", "new global value");

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
      TestResults.Add("dpinvoke", TestResults.GetResult("global"));
    }

    protected override void DataPortal_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    {
      TestResults.Add("dpinvokecomplete", TestResults.GetResult("global"));
    }

  }
}