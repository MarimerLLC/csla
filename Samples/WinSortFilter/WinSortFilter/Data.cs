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
#pragma warning disable CSLA0007 // Properties that use managed backing fields should only use Get/Set/Read/Load methods and nothing else
    public string Name
    {
      get => GetProperty(NameProperty) ?? string.Empty;
      set => SetProperty(NameProperty, value);
    }
#pragma warning restore CSLA0007

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
