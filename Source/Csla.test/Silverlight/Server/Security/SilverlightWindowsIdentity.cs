//-----------------------------------------------------------------------
// <copyright file="SilverlightWindowsIdentity.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Serialization;
using System.Runtime.Serialization;
using Csla.DataPortalClient;
namespace Csla.Testing.Business.Security
{
  [Serializable()]
    public class SilverlightWindowsIdentity : Csla.Silverlight.Security.WindowsIdentity
  {
    protected SilverlightWindowsIdentity() { }

    internal static void GetSilverlightWindowsIdentity(EventHandler<DataPortalResult<SilverlightWindowsIdentity>> completed)
    {
      DataPortal<SilverlightWindowsIdentity> dp = new DataPortal<SilverlightWindowsIdentity>();
      dp.FetchCompleted += completed;
      dp.BeginFetch();
    }

    private void DataPortal_Fetch()
    {
      base.PopulateWindowsIdentity();
    }
  }
}