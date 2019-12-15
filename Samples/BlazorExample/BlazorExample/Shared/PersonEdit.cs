using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Csla;
using Csla.Rules;

namespace BlazorExample.Shared
{
  [Serializable]
  public class PersonEdit : BusinessBase<PersonEdit>
  {
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(nameof(Id));
    public int Id
    {
      get { return GetProperty(IdProperty); }
      set { SetProperty(IdProperty, value); }
    }

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
    [Required]
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    public static readonly PropertyInfo<int> NameLengthProperty = RegisterProperty<int>(nameof(NameLength));
    public int NameLength
    {
      get => GetProperty(NameLengthProperty);
      set => SetProperty(NameLengthProperty, value);
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRules.AddRule(new InfoText(NameProperty, "Person name (required)"));
      BusinessRules.AddRule(new CheckCase(NameProperty));
      BusinessRules.AddRule(new NoZAllowed(NameProperty));
      BusinessRules.AddRule(new LetterCount(NameProperty, NameLengthProperty));
    }

    [Create]
    private void Create()
    {
      Id = -1;
      base.DataPortal_Create();
    }

    [Fetch]
    private void Fetch(int id, [Inject]DataAccess.IPersonDal dal)
    {
      var data = dal.Get(id);
      using (BypassPropertyChecks)
        Csla.Data.DataMapper.Map(data, this);
      BusinessRules.CheckRules();
    }

    [Insert]
    private void Insert([Inject]DataAccess.IPersonDal dal)
    {
      using (BypassPropertyChecks)
      {
        var data = new DataAccess.PersonEntity
        {
          Name = Name
        };
        var result = dal.Insert(data);
        Id = result.Id;
      }
    }

    [Update]
    private void Update([Inject]DataAccess.IPersonDal dal)
    {
      using (BypassPropertyChecks)
      {
        var data = new DataAccess.PersonEntity
        {
          Id = Id,
          Name = Name
        };
        dal.Update(data);
      }
    }

    [DeleteSelf]
    private void DeleteSelf([Inject]DataAccess.IPersonDal dal)
    {
      Delete(ReadProperty(IdProperty), dal);
    }

    [Delete]
    private void Delete(int id, [Inject]DataAccess.IPersonDal dal)
    {
      dal.Delete(id);
    }

  }
}
