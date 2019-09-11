using System;
using System.Linq;
using Csla;

namespace WinSortFilter
{
  [Serializable]
  public class Data : BusinessBase<Data>
  {
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(nameof(Id));
    public int Id
    {
      get => GetProperty(IdProperty);
      set => SetProperty(IdProperty, value);
    }

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
    public string Name
    {
      get => GetProperty(NameProperty);
      set => SetProperty(NameProperty, value);
    }

    private static int _lastId;

    [CreateChild]
    private void Create()
    {
      Id = --_lastId;
    }

    [CreateChild]
    private void Create(int id, string name)
    {
      Id = id;
      Name = name;
      BusinessRules.CheckRules();
    }
  }
}
