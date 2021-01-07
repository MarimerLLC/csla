//-----------------------------------------------------------------------
// <copyright file="MockList.partial.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Data;
using Csla.DataPortalClient;

namespace Csla.Testing.Business.EditableChildTests
{
  public partial class MockList
  {
    private void DataPortal_Fetch()
    {
      // fetch with no filter
      Fetch("");
    }

    private void DataPortal_Fetch(string criteria)
    {
      Fetch(criteria);
    }


    protected override void DataPortal_Update()
    {
      Child_Update();
    }
  }
}