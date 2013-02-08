using System;
using System.Linq;
using Csla;
using Csla.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Library
{
  [Serializable]
  public class Person : BusinessBase<Person>
  {
    public static PropertyInfo<int> IdProperty = 
      RegisterProperty<int>(c => c.Id);
    public int Id
    {
      get { return GetProperty(IdProperty); }
      private set { LoadProperty(IdProperty, value); }
    }

    public static PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    [Required]
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRules.AddRule(new SpaceRequired(NameProperty));
    }

    private class SpaceRequired : Csla.Rules.BusinessRule
    {
      public SpaceRequired(Csla.Core.IPropertyInfo primaryProperty)
        : base(primaryProperty)
      {
        InputProperties = new System.Collections.Generic.List<Csla.Core.IPropertyInfo> { PrimaryProperty };
      }

      protected override void Execute(Csla.Rules.RuleContext context)
      {
        var val = (string)context.InputPropertyValues[PrimaryProperty];
        if (!val.Contains(" "))
          context.AddErrorResult("Value must contain a space");
      }
    }

    #region Factory Methods

    public static void NewPerson(EventHandler<DataPortalResult<Person>> callback)
    {
      var dp = new DataPortal<Person>();
      dp.CreateCompleted += callback;
      dp.BeginCreate();
    }

    public static void GetPerson(int id, EventHandler<DataPortalResult<Person>> callback)
    {
      var dp = new DataPortal<Person>();
      dp.FetchCompleted += callback;
      dp.BeginFetch(id);
    }

    #endregion

    #region Data Access

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public void Child_Fetch(int id, string name)
    {
      using (BypassPropertyChecks)
      {
        Id = id;
        Name = name;
      }
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public void Child_DeleteSelf()
    {
      var data = MockDb.Persons.Where(c => c.Id == this.Id).FirstOrDefault();
      if (data != null)
        MockDb.Persons.Remove(data);
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public void DataPortal_Fetch(int id, Csla.DataPortalClient.LocalProxy<Person>.CompletedHandler handler)
    {
      try
      {
        using (BypassPropertyChecks)
        {
          var data = MockDb.Persons.Where(c => c.Id == id).FirstOrDefault();
          if (data == null)
            throw new InvalidOperationException(string.Format("Person {0} not found", id));
          Id = data.Id;
          Name = data.FirstName + " " + data.LastName;
        }
        handler(this, null);
      }
      catch (Exception ex)
      {
        handler(this, ex);
      }
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public override void DataPortal_Insert(Csla.DataPortalClient.LocalProxy<Person>.CompletedHandler handler)
    {
      try
      {
        using (BypassPropertyChecks)
        {
          this.Id = MockDb.Persons.Max(c => c.Id) + 1;
          var fname = this.Name.Split(' ')[0];
          var lname = this.Name.Split(' ')[1];
          MockDb.Persons.Add(new PersonData { Id = this.Id, FirstName = fname, LastName = lname });
        }
        handler(this, null);
      }
      catch (Exception ex)
      {
        handler(this, ex);
      }
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public override void DataPortal_Update(Csla.DataPortalClient.LocalProxy<Person>.CompletedHandler handler)
    {
      try
      {
        using (BypassPropertyChecks)
        {
          var data = MockDb.Persons.Where(c => c.Id == this.Id).FirstOrDefault();
          if (data == null)
            throw new InvalidOperationException(string.Format("Person {0} not found", this.Id));
          var fname = this.Name.Split(' ')[0];
          var lname = this.Name.Split(' ')[1];
          data.FirstName = fname;
          data.LastName = lname;
        }
        handler(this, null);
      }
      catch (Exception ex)
      {
        handler(this, ex);
      }
    }

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public override void DataPortal_DeleteSelf(Csla.DataPortalClient.LocalProxy<Person>.CompletedHandler handler)
    {
      try
      {
        using (BypassPropertyChecks)
        {
          var data = MockDb.Persons.Where(c => c.Id == this.Id).FirstOrDefault();
          if (data != null)
            MockDb.Persons.Remove(data);
        }
        handler(this, null);
      }
      catch (Exception ex)
      {
        handler(this, ex);
      }
    }

    #endregion
  }
}
