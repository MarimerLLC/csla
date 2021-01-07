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
    private class Criteria
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

    public static Root NewRoot()
    {
      return Csla.DataPortal.Create<Root>(new Criteria());
    }

    public static Root GetRoot(string data)
    {
      return Csla.DataPortal.Fetch<Root>(new Criteria(data));
    }

    public static void DeleteRoot(string data)
    {
      Csla.DataPortal.Delete<Root>(new Criteria(data));
    }

    private void DataPortal_Create(object criteria)
    {
      Criteria crit = (Criteria)(criteria);
      using (BypassPropertyChecks)
        Data = crit._data;
      CreatedDomain = AppDomain.CurrentDomain.Id;
      Csla.ApplicationContext.GlobalContext.Add("Root", "Created");
    }

    protected void DataPortal_Fetch(object criteria)
    {
      Criteria crit = (Criteria)(criteria);
      using (BypassPropertyChecks)
        Data = crit._data;
      MarkOld();
      Csla.ApplicationContext.GlobalContext.Add("Root", "Fetched");
    }

    protected override void DataPortal_Insert()
    {
      Csla.ApplicationContext.GlobalContext.Add("clientcontext",
          ApplicationContext.ClientContext["clientcontext"]);

      Csla.ApplicationContext.GlobalContext.Add("globalcontext",
      ApplicationContext.GlobalContext["globalcontext"]);

      ApplicationContext.GlobalContext.Remove("globalcontext");
      ApplicationContext.GlobalContext["globalcontext"] = "new global value";

      Csla.ApplicationContext.GlobalContext.Add("Root", "Inserted");
    }

    protected override void DataPortal_Update()
    {
      //we would update here
      Csla.ApplicationContext.GlobalContext.Add("Root", "Updated");
    }

    protected override void DataPortal_DeleteSelf()
    {
      Csla.ApplicationContext.GlobalContext.Add("Root", "Deleted self");
    }

    protected void DataPortal_Delete(object criteria)
    {
      Csla.ApplicationContext.GlobalContext.Add("Root", "Deleted");
    }

    protected override void OnDeserialized(System.Runtime.Serialization.StreamingContext context)
    {
      Csla.ApplicationContext.GlobalContext.Add("Deserialized", "root Deserialized");
    }

    protected override void DataPortal_OnDataPortalInvoke(DataPortalEventArgs e)
    {
      Csla.ApplicationContext.GlobalContext["dpinvoke"] = ApplicationContext.GlobalContext["global"];
    }

    protected override void DataPortal_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    {
      Csla.ApplicationContext.GlobalContext["dpinvokecomplete"] = ApplicationContext.GlobalContext["global"];
    }

  }
}