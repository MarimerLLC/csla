//-----------------------------------------------------------------------
// <copyright file="DataPortalSelector.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Selects the appropriate data portal implementation</summary>
//-----------------------------------------------------------------------
using System;
#if !NETFX_CORE
using System.Configuration;
#endif
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Properties;
using System.Threading.Tasks;

namespace Csla.Server
{
  /// <summary>
  /// Selects the appropriate data portal implementation
  /// to invoke based on the object and configuration.
  /// </summary>
  public class DataPortalSelector : IDataPortalServer
  {
    /// <summary>
    /// Create a new business object.
    /// </summary>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    public async Task<DataPortalResult> Create(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      try
      {
        context.FactoryInfo = ObjectFactoryAttribute.GetObjectFactoryAttribute(objectType);
        if (context.FactoryInfo == null)
        {
          var dp = new SimpleDataPortal();
          return await dp.Create(objectType, criteria, context, isSync);
        }
        else
        {
          var dp = new FactoryDataPortal();
          return await dp.Create(objectType, criteria, context, isSync);
        }
      }
      catch (DataPortalException)
      {
        throw;
      }
      catch (Exception ex)
      {
        throw new DataPortalException(
          "DataPortal.Create " + Resources.FailedOnServer,
          ex, new DataPortalResult());
      }
    }

    /// <summary>
    /// Get an existing business object.
    /// </summary>
    /// <param name="objectType">Type of business object to retrieve.</param>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    public async Task<DataPortalResult> Fetch(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      try
      {
        context.FactoryInfo = ObjectFactoryAttribute.GetObjectFactoryAttribute(objectType);
        if (context.FactoryInfo == null)
        {
          var dp = new SimpleDataPortal();
          return await dp.Fetch(objectType, criteria, context, isSync);
        }
        else
        {
          var dp = new FactoryDataPortal();
          return await dp.Fetch(objectType, criteria, context, isSync);
        }
      }
      catch (DataPortalException)
      {
        throw;
      }
      catch (Exception ex)
      {
        throw new DataPortalException(
          "DataPortal.Fetch " + Resources.FailedOnServer,
          ex, new DataPortalResult());
      }
    }

    /// <summary>
    /// Update a business object.
    /// </summary>
    /// <param name="obj">Business object to update.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    public async Task<DataPortalResult> Update(object obj, DataPortalContext context, bool isSync)
    {
      try
      {
        context.FactoryInfo = ObjectFactoryAttribute.GetObjectFactoryAttribute(obj.GetType());
        if (context.FactoryInfo == null)
        {
          var dp = new SimpleDataPortal();
          return await dp.Update(obj, context, isSync);
        }
        else
        {
          var dp = new FactoryDataPortal();
          return await dp.Update(obj, context, isSync);
        }
      }
      catch (DataPortalException)
      {
        throw;
      }
      catch (Exception ex)
      {
        throw new DataPortalException(
          "DataPortal.Update " + Resources.FailedOnServer,
          ex, new DataPortalResult(obj));
      }
    }

    /// <summary>
    /// Delete a business object.
    /// </summary>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    public async Task<DataPortalResult> Delete(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      try
      {
        context.FactoryInfo = ObjectFactoryAttribute.GetObjectFactoryAttribute(objectType);
        if (context.FactoryInfo == null)
        {
          var dp = new SimpleDataPortal();
          return await dp.Delete(objectType, criteria, context, isSync);
        }
        else
        {
          var dp = new FactoryDataPortal();
          return await dp.Delete(objectType, criteria, context, isSync);
        }
      }
      catch (DataPortalException)
      {
        throw;
      }
      catch (Exception ex)
      {
        throw new DataPortalException(
          "DataPortal.Delete " + Resources.FailedOnServer,
          ex, new DataPortalResult());
      }
    }
  }
}