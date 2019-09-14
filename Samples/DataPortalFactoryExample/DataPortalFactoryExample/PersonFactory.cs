using System;
using System.Collections.Generic;
using System.Text;

namespace DataPortalFactoryExample
{
  public class PersonFactory : Csla.Server.ObjectFactory
  {
    public PersonEdit Create()
    {
      var result = new PersonEdit();
      LoadProperty(result, PersonEdit.NameProperty, "Xiaoping");
      CheckRules(result);
      MarkNew(result);
      return result;
    }

    public PersonEdit Fetch()
    {
      var result = new PersonEdit();
      LoadProperty(result, PersonEdit.NameProperty, "Xiaoping");
      CheckRules(result);
      MarkOld(result);
      return result;
    }
  }
}
