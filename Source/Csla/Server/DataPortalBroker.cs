//-----------------------------------------------------------------------
// <copyright file="DataPortalBroker.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Allows interception of DataPortal call</summary>
//-----------------------------------------------------------------------

using System.Xml;

using System.Diagnostics.CodeAnalysis;

namespace Csla.Server
{
  /// <summary>
  /// Allows the Data Portal call to be intercepted by
  /// a custom IDataPortalServer implementation.
  /// </summary>
  public class DataPortalBroker : IDataPortalServer
  {
    private readonly DataPortalSelector _dataPortalSelector;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="dataPortalSelector"></param>
    /// <exception cref="ArgumentNullException"><paramref name="dataPortalSelector"/> is <see langword="null"/>.</exception>
    public DataPortalBroker(DataPortalSelector dataPortalSelector)
    {
      _dataPortalSelector = Guard.NotNull(dataPortalSelector);
    }

    /// <summary>
    /// Gets or sets a reference to a implementation of
    /// IDataPortalServer to be used.
    /// </summary>
    public static IDataPortalServer? DataPortalServer { get; set; }

    /// <inheritdoc />
    public Task<DataPortalResult> Create([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      Guard.NotNull(objectType);
      Guard.NotNull(criteria);
      Guard.NotNull(context);

      if (DataPortalServer != null)
      {
        return DataPortalServer.Create(objectType, criteria, context, isSync);
      }
      else
      {
        return _dataPortalSelector.Create(objectType, criteria, context, isSync);
      }      
    }

    /// <inheritdoc />
    public Task<DataPortalResult> Fetch([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      Guard.NotNull(objectType);
      Guard.NotNull(criteria);
      Guard.NotNull(context);

      if (DataPortalServer != null)
      {
        return DataPortalServer.Fetch(objectType, criteria, context, isSync);
      }
      else
      {
        return _dataPortalSelector.Fetch(objectType, criteria, context, isSync);
      }
    }

    /// <inheritdoc />
    public Task<DataPortalResult> Update(object obj, DataPortalContext context, bool isSync)
    {
      Guard.NotNull(obj);
      Guard.NotNull(context);

      if (DataPortalServer != null)
      {
        return DataPortalServer.Update(obj, context, isSync);
      }
      else
      {
        return _dataPortalSelector.Update(obj, context, isSync);
      }
    }

    /// <inheritdoc />
    public Task<DataPortalResult> Delete([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      Guard.NotNull(objectType);
      Guard.NotNull(criteria);
      Guard.NotNull(context);

      if (DataPortalServer != null)
      {
        return DataPortalServer.Delete(objectType, criteria, context, isSync);
      }
      else
      {
        return _dataPortalSelector.Delete(objectType, criteria, context, isSync);
      }
    }
  }
}