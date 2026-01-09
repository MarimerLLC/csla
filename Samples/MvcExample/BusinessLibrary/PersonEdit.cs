using System.ComponentModel.DataAnnotations;
using Csla;
using Csla.Rules;

namespace BusinessLibrary
{
  [CslaImplementProperties]
  public partial class PersonEdit : BusinessBase<PersonEdit>
  {
    public partial int Id { get; private set; }

    [Required]
    public partial string? Name { get; set; }

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();
      BusinessRules.AddRule(new InfoText(NameProperty, "Person name (required)"));
      BusinessRules.AddRule(new CheckCase(NameProperty));
      BusinessRules.AddRule(new NoZAllowed(NameProperty));
    }

    [Create]
    private void Create()
    {
      Id = -1;
      BusinessRules.CheckRules();
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
