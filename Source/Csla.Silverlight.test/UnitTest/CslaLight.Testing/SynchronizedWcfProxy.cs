﻿//-----------------------------------------------------------------------
// <copyright file="SynchronizedWcfProxy.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Implements a data portal proxy object that only</summary>
//-----------------------------------------------------------------------
using System;
using System.Threading.Tasks;
using Csla.Serialization.Mobile;
using Csla.Threading;

namespace Csla.DataPortalClient
{
  /// <summary>
  /// Implements a data portal proxy object that only
  /// allows one call at a time.
  /// </summary>
  /// <typeparam name="T"></typeparam>
  public class SynchronizedWcfProxy : IDataPortalProxy
  {
    WcfProxy _proxy = new WcfProxy();

    public bool IsServerRemote
    {
      get { return _proxy.IsServerRemote; }
    }

    public async System.Threading.Tasks.Task<Server.DataPortalResult> Create(Type objectType, object criteria, Server.DataPortalContext context, bool isAsync)
    {
      return await _proxy.Create(objectType, criteria, context, isAsync);
    }

    public async System.Threading.Tasks.Task<Server.DataPortalResult> Fetch(Type objectType, object criteria, Server.DataPortalContext context, bool isAsync)
    {
      return await _proxy.Fetch(objectType, criteria, context, isAsync);
    }

    public async System.Threading.Tasks.Task<Server.DataPortalResult> Update(object obj, Server.DataPortalContext context, bool isAsync)
    {
      return await _proxy.Update(obj, context, isAsync);
    }

    public async System.Threading.Tasks.Task<Server.DataPortalResult> Delete(Type objectType, object criteria, Server.DataPortalContext context, bool isAsync)
    {
      return await _proxy.Delete(objectType, criteria, context, isAsync);
    }
  }
}
