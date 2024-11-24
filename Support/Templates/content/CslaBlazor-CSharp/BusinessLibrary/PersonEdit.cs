﻿using System.ComponentModel.DataAnnotations;
using Csla;
using Csla.Rules;

namespace CslaBlazor.BusinessLibrary
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

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    [ObjectAuthorizationRules]
    public static void AddObjectAuthorizationRules()
    {
      //Csla.Rules.BusinessRules.AddRule(typeof(PersonEdit),
      //  new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.CreateObject, "Admin"));
      //Csla.Rules.BusinessRules.AddRule(typeof(PersonEdit),
      //  new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.EditObject, "Admin"));
      //Csla.Rules.BusinessRules.AddRule(typeof(PersonEdit),
      //  new Csla.Rules.CommonRules.IsInRole(Csla.Rules.AuthorizationActions.DeleteObject, "Admin"));
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
      base.Child_Create();
    }

    [Fetch]
    private void Fetch(int id, [Inject]CslaBlazor.DataAccess.IPersonDal dal)
    {
      var data = dal.Get(id);
      using (BypassPropertyChecks)
        Csla.Data.DataMapper.Map(data, this);
      BusinessRules.CheckRules();
    }

    [Insert]
    private void Insert([Inject]CslaBlazor.DataAccess.IPersonDal dal)
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
    private void Update([Inject]CslaBlazor.DataAccess.IPersonDal dal)
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
    private void DeleteSelf([Inject]CslaBlazor.DataAccess.IPersonDal dal)
    {
      Delete(ReadProperty(IdProperty), dal);
    }

    [Delete]
    private void Delete(int id, [Inject]CslaBlazor.DataAccess.IPersonDal dal)
    {
      dal.Delete(id);
    }

  }
}
