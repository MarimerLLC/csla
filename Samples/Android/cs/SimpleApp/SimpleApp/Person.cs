using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;
using System.ComponentModel.DataAnnotations;

namespace SimpleApp
{
  [Serializable]
  public class Person : BusinessBase<Person>
  {
    public static PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id);
    public int Id
    {
      get { return GetProperty(IdProperty); }
      set { SetProperty(IdProperty, value); }
    }

    public static PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    [Required]
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    public static PropertyInfo<string> StatusProperty = RegisterProperty<string>(c => c.Status);
    public string Status
    {
      get { return GetProperty(StatusProperty); }
      set { SetProperty(StatusProperty, value); }
    }

    public static void NewPerson(EventHandler<DataPortalResult<Person>> callback)
    {
      DataPortal.BeginCreate<Person>(callback);
    }

    public static void GetPerson(int id, EventHandler<DataPortalResult<Person>> callback)
    {
      DataPortal.BeginFetch<Person>(id, callback);
    }

    public override void DataPortal_Create(Csla.DataPortalClient.LocalProxy<Person>.CompletedHandler handler)
    {
      using (BypassPropertyChecks)
      {
        Status = "Created";
      }
      base.DataPortal_Create(handler);
    }

    public void DataPortal_Fetch(int id, Csla.DataPortalClient.LocalProxy<Person>.CompletedHandler callback)
    {
      try
      {
        using (BypassPropertyChecks)
        {
          Id = id;
          Name = "Rocky";
          Status = "Retrieved";
        }
        callback(this, null);
      }
      catch (Exception ex)
      {
        callback(null, ex);
      }
    }

    public override void DataPortal_Insert(Csla.DataPortalClient.LocalProxy<Person>.CompletedHandler handler)
    {
      using (BypassPropertyChecks)
      {
        Status = "Inserted";
      }
      handler(this, null);
    }

    public override void DataPortal_Update(Csla.DataPortalClient.LocalProxy<Person>.CompletedHandler handler)
    {
      using (BypassPropertyChecks)
      {
        Status = "Updated";
      }
      handler(this, null);
    }

    public override void DataPortal_DeleteSelf(Csla.DataPortalClient.LocalProxy<Person>.CompletedHandler handler)
    {
      using (BypassPropertyChecks)
      {
        Status = "Deleted";
      }
      handler(this, null);
    }
  }
}