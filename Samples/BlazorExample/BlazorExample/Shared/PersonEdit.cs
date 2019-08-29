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

    [Create]
    private void Create(int id, string name)
    {
      using (BypassPropertyChecks)
      {
        Id = id;
        Name = name;
      }
    }

    [Fetch]
    private void Csla_Fetch(int id)
    {
      // TODO: load values into object

    }

    [Insert]
    private void Csla_Insert()
    {
      // TODO: insert object's data
    }

    [Update]
    private void Csla_Update()
    {
      // TODO: update object's data
    }

    [DeleteSelf]
    private void Csla_DeleteSelf()
    {
      Csla_Delete(ReadProperty(IdProperty));
    }

    [Delete]
    private void Csla_Delete(int id)
    {
      // TODO: delete object's data
    }

  }
}
