//-----------------------------------------------------------------------
// <copyright file="DataPortalBroker.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Allows interception of DataPortal call</summary>
//-----------------------------------------------------------------------

using System.Xml;

namespace Csla.Server
{
  /// <summary>
  /// Allows the Data Portal call to be intercepted by
  /// a custom IDataPortalServer implementation.
  /// </summary>
  public class DataPortalBroker : IDataPortalServer
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="dataPortalSelector"></param>
    /// <exception cref="ArgumentNullException"><paramref name="dataPortalSelector"/> is <see langword="null"/>.</exception>
    public DataPortalBroker(DataPortalSelector dataPortalSelector)
    {
      DataPortalSelector = dataPortalSelector ?? throw new ArgumentNullException(nameof(dataPortalSelector));
    }

    private DataPortalSelector DataPortalSelector { get; set; }
    /// <summary>
    /// Gets or sets a reference to a implementation of
    /// IDataPortalServer to be used.
    /// </summary>
    public static IDataPortalServer? DataPortalServer { get; set; }

    /// <inheritdoc />
    public Task<DataPortalResult> Create(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      if (objectType is null)
        throw new ArgumentNullException(nameof(objectType));
      if (criteria is null)
        throw new ArgumentNullException(nameof(criteria));
      if (context is null)
        throw new ArgumentNullException(nameof(context));

      if (DataPortalServer != null)
      {
        return DataPortalServer.Create(objectType, criteria, context, isSync);
      }
      else
      {
        return DataPortalSelector.Create(objectType, criteria, context, isSync);
      }      
    }

    /// <inheritdoc />
    public Task<DataPortalResult> Fetch(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      if (objectType is null)
        throw new ArgumentNullException(nameof(objectType));
      if (criteria is null)
        throw new ArgumentNullException(nameof(criteria));
      if (context is null)
        throw new ArgumentNullException(nameof(context));

      if (DataPortalServer != null)
      {
        return DataPortalServer.Fetch(objectType, criteria, context, isSync);
      }
      else
      {
        return DataPortalSelector.Fetch(objectType, criteria, context, isSync);
      }
    }

    /// <inheritdoc />
    public Task<DataPortalResult> Update(object obj, DataPortalContext context, bool isSync)
    {
      if (obj is null)
        throw new ArgumentNullException(nameof(obj));
      if (context is null)
        throw new ArgumentNullException(nameof(context));

      if (DataPortalServer != null)
      {
        return DataPortalServer.Update(obj, context, isSync);
      }
      else
      {
        return DataPortalSelector.Update(obj, context, isSync);
      }
    }

    /// <inheritdoc />
    public Task<DataPortalResult> Delete(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      if (objectType is null)
        throw new ArgumentNullException(nameof(objectType));
      if (criteria is null)
        throw new ArgumentNullException(nameof(criteria));
      if (context is null)
        throw new ArgumentNullException(nameof(context));

      if (DataPortalServer != null)
      {
        return DataPortalServer.Delete(objectType, criteria, context, isSync);
      }
      else
      {
        return DataPortalSelector.Delete(objectType, criteria, context, isSync);
      }
    }
  }
}