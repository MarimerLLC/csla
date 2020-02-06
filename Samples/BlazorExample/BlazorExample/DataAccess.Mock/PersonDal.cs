using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.Mock
{
  public class PersonDal : IPersonDal
  {
    private static readonly List<PersonEntity> _personTable = new List<PersonEntity>
    {
      new PersonEntity { Id = 1, Name = "Andy"},
      new PersonEntity { Id = 3, Name = "Buzz"}
    };

    public bool Delete(int id)
    {
      var person = _personTable.Where(p => p.Id == id).FirstOrDefault();
      if (person != null)
      {
        lock (_personTable)
          _personTable.Remove(person);
        return true;
      }
      else
      {
        return false;
      }
    }

    public bool Exists(int id)
    {
      var person = _personTable.Where(p => p.Id == id).FirstOrDefault();
      return !(person == null);
    }

    public PersonEntity Get(int id)
    {
      var person = _personTable.Where(p => p.Id == id).FirstOrDefault();
      if (person != null)
        return person;
      else
        throw new KeyNotFoundException($"Id {id}");
    }

    public List<PersonEntity> Get()
    {
      // return projection of entire list
      return _personTable.Where(r => true).ToList();
    }

    public PersonEntity Insert(PersonEntity person)
    {
      if (Exists(person.Id))
        throw new InvalidOperationException($"Key exists {person.Id}");
      lock (_personTable)
      {
        int lastId = _personTable.Max(m => m.Id);
        person.Id = ++lastId;
        _personTable.Add(person);
      }
      return person;
    }

    public PersonEntity Update(PersonEntity person)
    {
      lock (_personTable)
      {
        var old = Get(person.Id);
        old.Name = person.Name;
        return old;
      }
    }
  }
}
