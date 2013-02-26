using System;
using Csla;
using Csla.Serialization;
using System.ComponentModel.DataAnnotations;

namespace SilverlightSOA
{
  [Serializable]
  public class Person : BusinessBase<Person>
  {
    #region Properties

    private static PropertyInfo<int> IdProperty = RegisterProperty(new PropertyInfo<int>("Id", "Id"));
    public int Id
    {
      get { return GetProperty(IdProperty); }
      set { SetProperty(IdProperty, value); }
    }

    private static PropertyInfo<string> FirstNameProperty = RegisterProperty(new PropertyInfo<string>("FirstName", "First name"));
    [Required(ErrorMessage = "First name required")]
    public string FirstName
    {
      get { return GetProperty(FirstNameProperty); }
      set { SetProperty(FirstNameProperty, value); }
    }

    private static PropertyInfo<string> LastNameProperty = RegisterProperty(new PropertyInfo<string>("LastName", "Last name"));
    [Required(ErrorMessage = "Last name required")]
    public string LastName
    {
      get { return GetProperty(LastNameProperty); }
      set { SetProperty(LastNameProperty, value); }
    }

    #endregion

    #region Business rules

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
    }

    #endregion

    #region Factory methods

    public static void NewPerson(
      EventHandler<Csla.DataPortalResult<Person>> handler)
    {
      DataPortal.BeginCreate<Person>((o, e) =>
        {
          handler(null, e);
        });
    }

    public static void GetPerson(int id,
      EventHandler<Csla.DataPortalResult<Person>> handler)
    {
      DataPortal.BeginFetch<Person>(
        new SingleCriteria<Person, int>(id),
        (o, e) =>
        {
          handler(null, e);
        });
    }

    #endregion

    #region Service calls

    public void DataPortal_Fetch(
      SingleCriteria<Person, int> criteria,
      Csla.DataPortalClient.LocalProxy<Person>.CompletedHandler handler)
    {
      var svc = new PersonService.PersonServiceClient(
        "BasicHttpBinding_IPersonService");
      svc.GetPersonCompleted += (o, e) =>
        {
          LoadProperty(IdProperty, e.Result.Id);
          LoadProperty(FirstNameProperty, e.Result.FirstName);
          LoadProperty(LastNameProperty, e.Result.LastName);
          handler(this, e.Error);
        };
      svc.GetPersonAsync(criteria.Value);
    }

    public override void DataPortal_Insert(Csla.DataPortalClient.LocalProxy<Person>.CompletedHandler handler)
    {
      var svc = new PersonService.PersonServiceClient(
        "BasicHttpBinding_IPersonService");
      svc.AddPersonCompleted += (o, e) =>
      {
        LoadProperty(IdProperty, e.Result.Id);
        LoadProperty(FirstNameProperty, e.Result.FirstName);
        LoadProperty(LastNameProperty, e.Result.LastName);
        handler(this, e.Error);
      };
      var obj = new PersonService.PersonData();
      obj.Id = ReadProperty(IdProperty);
      obj.FirstName = ReadProperty(FirstNameProperty);
      obj.LastName = ReadProperty(LastNameProperty);
      svc.AddPersonAsync(obj);
    }

    public override void DataPortal_Update(Csla.DataPortalClient.LocalProxy<Person>.CompletedHandler handler)
    {
      var svc = new PersonService.PersonServiceClient(
        "BasicHttpBinding_IPersonService");
      svc.UpdatePersonCompleted += (o, e) =>
      {
        LoadProperty(IdProperty, e.Result.Id);
        LoadProperty(FirstNameProperty, e.Result.FirstName);
        LoadProperty(LastNameProperty, e.Result.LastName);
        handler(this, e.Error);
      };
      var obj = new PersonService.PersonData();
      obj.Id = ReadProperty(IdProperty);
      obj.FirstName = ReadProperty(FirstNameProperty);
      obj.LastName = ReadProperty(LastNameProperty);
      svc.UpdatePersonAsync(obj);
    }

    public override void DataPortal_DeleteSelf(Csla.DataPortalClient.LocalProxy<Person>.CompletedHandler handler)
    {
      var svc = new PersonService.PersonServiceClient(
        "BasicHttpBinding_IPersonService");
      svc.DeletePersonCompleted += (o, e) =>
      {
        LoadProperty(IdProperty, 0);
        handler(this, e.Error);
      };
      svc.DeletePersonAsync(Id);
    }
    #endregion
  }
}
