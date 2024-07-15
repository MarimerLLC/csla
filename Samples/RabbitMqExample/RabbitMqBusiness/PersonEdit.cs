using Csla;

namespace RabbitMqBusiness;

public class PersonEdit : BusinessBase<PersonEdit>
{
  public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
  public string Name
  {
    get => GetProperty(NameProperty);
    set => SetProperty(NameProperty, value);
  }

  [Create]
  private void Create() 
  { }

  [Fetch]
  private void Fetch(string name)
  { 
    using (BypassPropertyChecks)
    {
      Name = name;
    }
  }
}
