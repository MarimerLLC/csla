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

    public partial int NameLength { get; private set; }

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
      LoadProperty(IdProperty, -1);
    }

    [Fetch]
    private void Fetch(int id, [Inject] DataAccess.IPersonDal dal)
    {
      var data = dal.Get(id);
      Csla.Data.DataMapper.Map(data, this);
      BusinessRules.CheckRules();
    }

    [Insert]
    private void Insert([Inject] DataAccess.IPersonDal dal)
    {
      var data = new DataAccess.PersonEntity
      {
        Name = Name
      };
      var result = dal.Insert(data);
      LoadProperty(IdProperty, result.Id);
    }

    [Update]
    private void Update([Inject] DataAccess.IPersonDal dal)
    {
      var data = new DataAccess.PersonEntity
      {
        Id = Id,
        Name = Name
      };
      dal.Update(data);
    }

    [DeleteSelf]
    private void DeleteSelf([Inject] DataAccess.IPersonDal dal)
    {
      Delete(Id, dal);
    }

    [Delete]
    private void Delete(int id, [Inject] DataAccess.IPersonDal dal)
    {
      dal.Delete(id);
    }
  }
}
