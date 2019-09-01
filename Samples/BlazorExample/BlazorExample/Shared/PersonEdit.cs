using System;
using System.Collections.Generic;
using System.Text;
using Csla;

namespace BlazorExample.Shared
{
  [Serializable]
  public class PersonEdit : BusinessBase<PersonEdit>
  {
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(nameof(Id));
    public int Id
    {
      get { return GetProperty(IdProperty); }
      set { SetProperty(IdProperty, value); }
    }

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    private static int lastId;

    [Create]
    private void Create()
    {
      Id = lastId++;
    }

    [Create]
    private void Create(string name)
    {
      using (BypassPropertyChecks)
      {
        Id = lastId++;
        Name = name;
      }
    }

    [Fetch]
    private void Fetch(int id)
    {
      // TODO: load values into object

    }

    [Insert]
    private void Insert()
    {
      // TODO: insert object's data
    }

    [Update]
    private void Update()
    {
      // TODO: update object's data
    }

    [DeleteSelf]
    private void DeleteSelf()
    {
      Delete(ReadProperty(IdProperty));
    }

    [Delete]
    private void Delete(int id)
    {
      // TODO: delete object's data
    }

  }
}
