using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccess.EF
{
  public class PersonEFDal : IPersonDal
  {
    //private static readonly List<PersonEntity> _personTable = new List<PersonEntity>
    //{
    //  new PersonEntity { Id = 1, Name = "Andy"},
    //  new PersonEntity { Id = 3, Name = "Buzz"}
    //};
    private readonly PersonDbContext _context;

    public PersonEFDal(PersonDbContext context)
    {
      _context = context;
    }
    public bool Delete(int id)
    {
      var person = _context.Persons.Where(p => p.Id == id).FirstOrDefault();
      if (person != null)
      {
        _context.Persons.Remove(person);
        _context.SaveChanges();
        //lock (_personTable)
        //  _personTable.Remove(person);
        return true;
      }
      else
      {
        return false;
      }
    }

    public bool Exists(int id)
    {
      var person = _context.Persons.Where(p => p.Id == id).FirstOrDefault();
      return !(person == null);
    }

    public PersonEntity Get(int id)
    {
      var person = _context.Persons.Where(p => p.Id == id).FirstOrDefault();
      if (person != null)
        return person;
      else
        throw new KeyNotFoundException($"Id {id}");
    }

    public List<PersonEntity> Get()
    {
      // return projection of entire list
      return _context.Persons.Where(r => true).ToList();
    }

    public PersonEntity Insert(PersonEntity person)
    {
      if (Exists(person.Id))
        throw new InvalidOperationException($"Key exists {person.Id}");
     // lock (_personTable)
      {
        int lastId = 0;
        try
        {
          lastId = _context.Persons.Max(m => m.Id);
        }
        catch(Exception ex)
        {

        }
        person.Id = ++lastId;
        _context.Persons.Add(person);
        _context.SaveChanges();
      }
      return person;
    }

    public PersonEntity Update(PersonEntity person)
    {
      //lock (_personTable)
      {
        var old = Get(person.Id);
        old.Name = person.Name;
        _context.SaveChanges();
        return old;
      }
    }
  }
}
