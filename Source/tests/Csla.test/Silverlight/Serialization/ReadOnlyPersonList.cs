//-----------------------------------------------------------------------
// <copyright file="ReadOnlyPersonList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using Csla.Serialization;
using Csla;

namespace cslalighttest.Serialization
{
  [Serializable]
  public class ReadOnlyPersonList : ReadOnlyBindingListBase<ReadOnlyPersonList, ReadOnlyPerson>
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