﻿//-----------------------------------------------------------------------
// <copyright file="ObjectAccessor.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>internal implemetation to expose BypassPropertyChecks</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csla.Server;

namespace Csla.Validation
{
  internal class ObjectAccessor : ObjectFactory
  {
    internal new IDisposable BypassPropertyChecks(Csla.Core.BusinessBase target)
    {
      return base.BypassPropertyChecks(target);
    }
  }
}
