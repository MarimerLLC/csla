using System;
using Csla.Xaml;
using Csla.Serialization;
using Csla;

namespace cslalighttest.Serialization
{
  [Serializable]
  public class ReadOnlyPersonList : ReadOnlyListBase<ReadOnlyPersonList, ReadOnlyPerson>
  {
    public static ReadOnlyPersonList GetReadOnlyPersonList()
    {
      ReadOnlyPersonList list = new ReadOnlyPersonList();
      list.RaiseListChangedEvents = false;
      list.IsReadOnly = false;
      list.Add(ReadOnlyPerson.GetReadOnlyPerson("John Doe", 1981));
      list.Add(ReadOnlyPerson.GetReadOnlyPerson("Jane Doe", 1982));
      list.IsReadOnly = true;
      list.RaiseListChangedEvents = true;
      return list;
    }
  }
}
