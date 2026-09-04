namespace DataAccess;

/// <summary>
/// Small in-memory DAL in the same spirit as the CSLA MauiExample.
/// It keeps the Avalonia sample self-contained and database-free.
/// </summary>
public sealed class PersonDal : IPersonDal
{
  private static readonly object SyncRoot = new();

  private static readonly List<PersonEntity> Data =
  [
    new() { Id = 1, Name = "Rocky Lhotka" },
    new() { Id = 2, Name = "Ada Lovelace" },
    new() { Id = 3, Name = "Grace Hopper" }
  ];

  public bool Exists(int id)
  {
    lock (SyncRoot)
      return Data.Any(p => p.Id == id);
  }

  public PersonEntity Get(int id)
  {
    lock (SyncRoot)
    {
      var item = Data.Single(p => p.Id == id);
      return Clone(item);
    }
  }

  public List<PersonEntity> Get()
  {
    lock (SyncRoot)
      return Data.Select(Clone).ToList();
  }

  public PersonEntity Insert(PersonEntity person)
  {
    lock (SyncRoot)
    {
      var nextId = Data.Count == 0 ? 1 : Data.Max(p => p.Id) + 1;
      var result = Clone(person);
      result.Id = nextId;
      Data.Add(result);
      return Clone(result);
    }
  }

  public PersonEntity Update(PersonEntity person)
  {
    lock (SyncRoot)
    {
      var existing = Data.Single(p => p.Id == person.Id);
      existing.Name = person.Name;
      return Clone(existing);
    }
  }

  public bool Delete(int id)
  {
    lock (SyncRoot)
    {
      var existing = Data.SingleOrDefault(p => p.Id == id);
      if (existing is null)
        return false;

      Data.Remove(existing);
      return true;
    }
  }

  private static PersonEntity Clone(PersonEntity source) =>
    new() { Id = source.Id, Name = source.Name };
}
