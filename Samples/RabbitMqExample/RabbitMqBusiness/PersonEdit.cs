using Csla;

namespace RabbitMqBusiness;

[CslaImplementProperties]
public partial class PersonEdit : BusinessBase<PersonEdit>
{
  public partial string Name { get; set; }

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
