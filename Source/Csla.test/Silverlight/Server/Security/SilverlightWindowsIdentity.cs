//-----------------------------------------------------------------------
// <copyright file="SilverlightWindowsIdentity.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
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


    private void DataPortal_Fetch()
    {
      base.PopulateWindowsIdentity();
    }
  }
}