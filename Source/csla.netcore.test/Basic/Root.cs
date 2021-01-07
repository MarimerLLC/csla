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

    [Create]
    private void DataPortal_Create(object criteria)
    {
      Criteria crit = (Criteria)(criteria);
      using (BypassPropertyChecks)
      {
        Data = crit._data;
        Children = Csla.Test.Basic.Children.NewChildren();
      }
      CreatedDomain = AppDomain.CurrentDomain.Id;
#pragma warning disable CS0618 // Type or member is obsolete
      Csla.ApplicationContext.GlobalContext.Add("Root", "Created");
#pragma warning restore CS0618 // Type or member is obsolete
    }

    [Fetch]
    protected void DataPortal_Fetch(object criteria)
    {
      Criteria crit = (Criteria)(criteria);
      using (BypassPropertyChecks)
      {
        Data = crit._data;
        Children = Csla.Test.Basic.Children.NewChildren();
      }
      MarkOld();
#pragma warning disable CS0618 // Type or member is obsolete
      Csla.ApplicationContext.GlobalContext.Add("Root", "Fetched");
#pragma warning restore CS0618 // Type or member is obsolete
    }

    [Insert]
    protected override void DataPortal_Insert()
    {
#pragma warning disable CS0618 // Type or member is obsolete
      Csla.ApplicationContext.GlobalContext.Add("clientcontext",
          ApplicationContext.ClientContext["clientcontext"]);

      Csla.ApplicationContext.GlobalContext.Add("globalcontext",
      ApplicationContext.GlobalContext["globalcontext"]);

      ApplicationContext.GlobalContext.Remove("globalcontext");
      ApplicationContext.GlobalContext["globalcontext"] = "new global value";

      Csla.ApplicationContext.GlobalContext.Add("Root", "Inserted");
#pragma warning restore CS0618 // Type or member is obsolete
    }

    [Update]
    protected override void DataPortal_Update()
    {
      //we would update here
#pragma warning disable CS0618 // Type or member is obsolete
      Csla.ApplicationContext.GlobalContext.Add("Root", "Updated");
#pragma warning restore CS0618 // Type or member is obsolete
    }

    [DeleteSelf]
    protected override void DataPortal_DeleteSelf()
    {
#pragma warning disable CS0618 // Type or member is obsolete
      Csla.ApplicationContext.GlobalContext.Add("Root", "Deleted self");
#pragma warning restore CS0618 // Type or member is obsolete
    }

    [Delete]
    protected void DataPortal_Delete(object criteria)
    {
#pragma warning disable CS0618 // Type or member is obsolete
      Csla.ApplicationContext.GlobalContext.Add("Root", "Deleted");
#pragma warning restore CS0618 // Type or member is obsolete
    }

    protected override void OnDeserialized(System.Runtime.Serialization.StreamingContext context)
    {
#pragma warning disable CS0618 // Type or member is obsolete
      Csla.ApplicationContext.GlobalContext.Add("Deserialized", "root Deserialized");
#pragma warning restore CS0618 // Type or member is obsolete
    }

    protected override void DataPortal_OnDataPortalInvoke(DataPortalEventArgs e)
    {
#pragma warning disable CS0618 // Type or member is obsolete
      Csla.ApplicationContext.GlobalContext["dpinvoke"] = ApplicationContext.GlobalContext["global"];
#pragma warning restore CS0618 // Type or member is obsolete
    }

    protected override void DataPortal_OnDataPortalInvokeComplete(DataPortalEventArgs e)
    {
#pragma warning disable CS0618 // Type or member is obsolete
      Csla.ApplicationContext.GlobalContext["dpinvokecomplete"] = ApplicationContext.GlobalContext["global"];
#pragma warning restore CS0618 // Type or member is obsolete
    }

    internal int GetEditLevel()
    {
      return EditLevel;
    }
  }
}