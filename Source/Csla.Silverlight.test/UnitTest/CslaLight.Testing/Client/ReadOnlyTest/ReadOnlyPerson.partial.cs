//-----------------------------------------------------------------------
// <copyright file="ReadOnlyPerson.partial.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Csla.Testing.Business.ReadOnlyTest
{
  public partial class ReadOnlyPerson
  {
    public ReadOnlyPerson() { }

    public static void Fetch(Guid id, Action<ReadOnlyPerson, Exception> completed)
    {
      Csla.DataPortalClient.WcfProxy.DefaultUrl = DataPortalUrl;
      DataPortal<ReadOnlyPerson> dp = new DataPortal<ReadOnlyPerson>();
      dp.FetchCompleted += (o, e) =>
      {
        if (completed != null)
          completed(e.Object, e.Error);
      };
      dp.BeginFetch(new SingleCriteria<ReadOnlyPerson, Guid>(id));
    }

  }
}