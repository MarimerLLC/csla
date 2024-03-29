﻿using System;
using Csla;
using DataAccessLayer;
using System.ComponentModel.DataAnnotations;

namespace BusinessLayer
{
  [Serializable]
  public class PersonEdit : BusinessBase<PersonEdit>
  {
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id);
    public int Id
    {
      get { return GetProperty(IdProperty); }
      private set { LoadProperty(IdProperty, value); }
    }

    public static readonly PropertyInfo<string> FirstNameProperty = RegisterProperty<string>(c => c.FirstName);
    [Required]
    public string FirstName
    {
      get { return GetProperty(FirstNameProperty); }
      set { SetProperty(FirstNameProperty, value); }
    }

    public static readonly PropertyInfo<string> LastNameProperty = RegisterProperty<string>(c => c.LastName);
    [Required]
    public string LastName
    {
      get { return GetProperty(LastNameProperty); }
      set { SetProperty(LastNameProperty, value); }
    }

    [Create]
    private void Create()
    {
      var dal = new PersonDal();
      var dto = dal.Create();
      using (BypassPropertyChecks)
      {
        Id = dto.Id;
        FirstName = dto.FirstName;
        LastName = dto.LastName;
      }
      BusinessRules.CheckRules();
    }

    [Fetch]
    private void Fetch(int id)
    {
      var dal = new PersonDal();
      var dto = dal.GetPerson(id);
      using (BypassPropertyChecks)
      {
        Id = dto.Id;
        FirstName = dto.FirstName;
        LastName = dto.LastName;
      }
    }

    [Insert]
    private void Insert()
    {
      var dal = new PersonDal();
      using (BypassPropertyChecks)
      {
        var dto = new PersonDto
        {
          FirstName = this.FirstName,
          LastName = this.LastName
        };
        Id = dal.InsertPerson(dto);
      }
    }

    [Update]
    private void Update()
    {
      var dal = new PersonDal();
      using (BypassPropertyChecks)
      {
        var dto = new PersonDto
        {
          Id = this.Id,
          FirstName = this.FirstName,
          LastName = this.LastName
        };
        dal.UpdatePerson(dto);
      }
    }

    [Delete]
    private void Delete(int id)
    {
      var dal = new PersonDal();
      using (BypassPropertyChecks)
      {
        dal.DeletePerson(id);
      }
    }

    [DeleteSelf]
    private void DeleteSelf()
    {
      Delete(Id);
    }
  }
}
