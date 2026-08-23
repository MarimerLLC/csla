using Csla;
using Csla.Rules.CommonRules;
using DataAccess;

namespace BusinessLibrary;

[Serializable]
public sealed class PersonEdit : BusinessBase<PersonEdit>
{
  public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(nameof(Id));
  public int Id
  {
    get => GetProperty(IdProperty);
    private set => SetProperty(IdProperty, value);
  }

  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  protected override void AddBusinessRules()
  {
    base.AddBusinessRules();
    BusinessRules.AddRule(new Required(NameProperty));
    BusinessRules.AddRule(new MaxLength(NameProperty, 50));
  }

  [Create]
  private void Create()
  {
    using (BypassPropertyChecks)
    {
      Id = 0;
      Name = string.Empty;
    }

    BusinessRules.CheckRules();
  }

  [Fetch]
  private void Fetch(int id, [Inject] IPersonDal dal)
  {
    var data = dal.Get(id);

    using (BypassPropertyChecks)
    {
      Id = data.Id;
      Name = data.Name;
    }
  }

  [Insert]
  private void Insert([Inject] IPersonDal dal)
  {
    var result = dal.Insert(new PersonEntity
    {
      Id = Id,
      Name = Name
    });

    using (BypassPropertyChecks)
      Id = result.Id;
  }

  [Update]
  private void Update([Inject] IPersonDal dal)
  {
    dal.Update(new PersonEntity
    {
      Id = Id,
      Name = Name
    });
  }

  [DeleteSelf]
  private void DeleteSelf([Inject] IPersonDal dal)
  {
    if (Id != 0)
      dal.Delete(Id);
  }

  [Delete]
  private void Delete(int id, [Inject] IPersonDal dal)
  {
    dal.Delete(id);
  }
}
