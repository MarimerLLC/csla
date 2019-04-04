//-----------------------------------------------------------------------
// <copyright file="ReadOnlyPerson.partial.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Testing.Business.ReadOnlyTest
{
  public partial class ReadOnlyPerson
  {
    private ReadOnlyPerson() { }

    private void DataPortal_Fetch(Guid criteria)
    {
      LoadProperty<Guid>(IdProperty, criteria);
      LoadProperty(NameProperty, criteria.ToString());
      LoadProperty(BirthdateProperty, new DateTime(1980, 1, 1));
    }
  }
}