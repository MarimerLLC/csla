using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace SilverlightSOAWeb
{
  public class PersonService : IPersonService
  {
    public PersonData GetPerson(int id)
    {
      var result = new PersonData();
      result.Id = id;
      result.FirstName = "Jonathan";
      result.LastName = "Andrews";
      System.Threading.Thread.Sleep(1000);
      return result;
    }

    public PersonData AddPerson(PersonData obj)
    {
      obj.Id = 123;
      System.Threading.Thread.Sleep(1000);
      return obj;
    }

    public PersonData UpdatePerson(PersonData obj)
    {
      System.Threading.Thread.Sleep(1000);
      return obj;
    }

    public void DeletePerson(int id)
    {
      System.Threading.Thread.Sleep(1000);
    }
  }
}
