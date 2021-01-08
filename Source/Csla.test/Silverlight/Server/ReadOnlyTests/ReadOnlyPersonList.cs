//-----------------------------------------------------------------------
// <copyright file="ReadOnlyPersonList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Testing.Business.ReadOnlyTest
{
  [Serializable]
  public class ReadOnlyPersonList : ReadOnlyBindingListBase<ReadOnlyPersonList, ReadOnlyPerson>
  {
    private void DataPortal_Fetch()
    {
      RaiseListChangedEvents = false;
      IsReadOnly = false;
      Add(ReadOnlyPerson.GetReadOnlyPersonForList("John Doe", 1981));
      Add(ReadOnlyPerson.GetReadOnlyPersonForList("Jane Doe", 1982));
      IsReadOnly = true;
      RaiseListChangedEvents = true;
    }
  }
}