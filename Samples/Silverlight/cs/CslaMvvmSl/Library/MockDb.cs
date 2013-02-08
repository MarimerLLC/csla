using System;
using System.Collections.Generic;

namespace Library
{
  internal static class MockDb
  {
    public static List<PersonData> Persons { get; private set; }

    static MockDb()
    {
      Persons = new List<PersonData> { 
        new PersonData { Id = 1, FirstName = "Andrew", LastName="Smith" },
        new PersonData { Id = 33, FirstName = "Alecia", LastName="Drew" },
        new PersonData { Id = 938, FirstName = "Aaron", LastName="Kowalski" },
        new PersonData { Id = 148, FirstName = "Nancy", LastName="Ripley" },
        new PersonData { Id = 534, FirstName = "Abdul", LastName="McCarter" },
        new PersonData { Id = 23, FirstName = "Jason", LastName="Halabi" }
      };
    }
  }

  internal class PersonData
  {
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
  }
}
