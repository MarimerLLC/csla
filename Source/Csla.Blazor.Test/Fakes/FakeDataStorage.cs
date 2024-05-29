using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Blazor.Test.Fakes
{
  public static class FakeDataStorage
  {
    private static readonly Dictionary<Guid, FakePerson> FakePersonsStorage = new();

    public static FakePerson GetFakePerson(Guid id)
    {
      _ = FakePersonsStorage.TryGetValue(id, out var result);
      return result;
    }

    public static void InsertFakePerson(FakePerson person)
    {
      if (person == null) throw new ArgumentNullException(nameof(person));
      if (FakePersonsStorage.ContainsKey(person.Id)) throw new Exception($"Cannot add duplicate person having Id {person.Id}");
      FakePersonsStorage[person.Id] = person;
    }

    public static void UpdateFakePerson(FakePerson person)
    {
      if (person == null) throw new ArgumentNullException(nameof(person));
      if (!FakePersonsStorage.ContainsKey(person.Id)) throw new Exception($"Person having Id {person.Id} not found");
      FakePersonsStorage[person.Id] = person;
    }
  }
}
