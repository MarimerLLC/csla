﻿//-----------------------------------------------------------------------
// <copyright file="ReadOnlyPersonList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Csla;
using Csla.Serialization;
using Csla.Silverlight;
using Csla.Core.FieldManager;
using System.Linq;
using System.Text;
using Csla.DataPortalClient;

namespace Csla.Testing.Business.ReadOnlyTest
{
  [Serializable]
  public class ReadOnlyPersonList : ReadOnlyBindingListBase<ReadOnlyPersonList, ReadOnlyPerson>
  {


    public static void Fetch(EventHandler<DataPortalResult<ReadOnlyPersonList>> completed)
    {
      DataPortal<ReadOnlyPersonList> dp = new DataPortal<ReadOnlyPersonList>();
      dp.FetchCompleted += completed;
      dp.BeginFetch();
    }

#if SILVERLIGHT
    public void DataPortal_Fetch()
    {
      Fetch();
    }
#else
    private void DataPortal_Fetch()
    {
      Fetch();
    }
#endif
    private void Fetch()
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