using Csla;

namespace DataPortalFactoryExample
{
  public class PersonFactory : Csla.Server.ObjectFactory
  {
    public PersonFactory(ApplicationContext applicationContext)
      : base(applicationContext) { }

    public PersonEdit Create()
    {
      var result = ApplicationContext.CreateInstanceDI<PersonEdit>();
      LoadProperty(result, PersonEdit.NameProperty, "Xiaoping");
      CheckRules(result);
      MarkNew(result);
      return result;
    }

    public PersonEdit Fetch()
    {
      var result = ApplicationContext.CreateInstanceDI<PersonEdit>();
      LoadProperty(result, PersonEdit.NameProperty, "Xiaoping");
      CheckRules(result);
      MarkOld(result);
      return result;
    }
  }
}
