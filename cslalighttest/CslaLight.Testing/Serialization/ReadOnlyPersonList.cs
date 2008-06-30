using System;
using System.Collections.Generic;
using Csla;
using Csla.Serialization;
using Csla.Silverlight;
using Csla.Core.FieldManager;


namespace cslalighttest.Serialization
{
  [Serializable]
  public class ReadOnlyPersonList: ReadOnlyListBase<ReadOnlyPersonList, ReadOnlyPerson>
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
