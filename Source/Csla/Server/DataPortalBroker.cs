//-----------------------------------------------------------------------
// <copyright file="DataPortalBroker.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Allows interception of DataPortal call</summary>
//-----------------------------------------------------------------------

using System.Xml;
using System.Diagnostics.CodeAnalysis;
using Csla.Core;

namespace Csla.Server
{
  /// <summary>
  /// Allows the Data Portal call to be intercepted by
  /// a custom IDataPortalServer implementation.
  /// </summary>
  /// <remarks>
  /// The broker also checks the not-null property invariant of root objects
  /// (<see cref="INotNullPropertyInfo"/>). Its exit check runs after the
  /// operation has completed, so a root object that fails it has already had
  /// <c>DataPortal_OnDataPortalInvokeComplete</c> called, and does not get
  /// <c>DataPortal_OnDataPortalException</c>; the failure reaches the caller as a
  /// <see cref="DataPortalException"/>. A child object is checked before
  /// <c>Child_OnDataPortalInvokeComplete</c>, and a failure there raises
  /// <c>Child_OnDataPortalException</c> instead.
  /// </remarks>
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
      _dataPortalSelector = dataPortalSelector ?? throw new ArgumentNullException(nameof(dataPortalSelector));
    }

    /// <summary>
    /// Gets or sets a reference to a implementation of
    /// IDataPortalServer to be used.
    /// </summary>
    public static IDataPortalServer? DataPortalServer { get; set; }

    /// <inheritdoc />
    /// <remarks>
    /// Except for a command, the created object must satisfy the not-null
    /// property invariant (<see cref="INotNullPropertyInfo"/>); otherwise
    /// <see cref="InvalidOperationException"/> is thrown after the operation ran.
    /// </remarks>
    public async Task<DataPortalResult> Create([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      if (objectType is null)
        throw new ArgumentNullException(nameof(objectType));
      if (criteria is null)
        throw new ArgumentNullException(nameof(criteria));
      if (context is null)
        throw new ArgumentNullException(nameof(context));

      DataPortalResult result;
      if (DataPortalServer != null)
      {
        result = await DataPortalServer.Create(objectType, criteria, context, isSync).ConfigureAwait(false);
      }
      else
      {
        result = await _dataPortalSelector.Create(objectType, criteria, context, isSync).ConfigureAwait(false);
      }

      if (result.ReturnObject is not ICommandObject)
        NotNullInvariant.Check(result.ReturnObject, nameof(Create), isEntry: false);
      return result;
    }

    /// <inheritdoc />
    /// <remarks>
    /// The fetched object (including a command run through Execute) must
    /// satisfy the not-null property invariant (<see cref="INotNullPropertyInfo"/>);
    /// otherwise <see cref="InvalidOperationException"/> is thrown after the operation ran.
    /// </remarks>
    public async Task<DataPortalResult> Fetch([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      if (objectType is null)
        throw new ArgumentNullException(nameof(objectType));
      if (criteria is null)
        throw new ArgumentNullException(nameof(criteria));
      if (context is null)
        throw new ArgumentNullException(nameof(context));

      DataPortalResult result;
      if (DataPortalServer != null)
      {
        result = await DataPortalServer.Fetch(objectType, criteria, context, isSync).ConfigureAwait(false);
      }
      else
      {
        result = await _dataPortalSelector.Fetch(objectType, criteria, context, isSync).ConfigureAwait(false);
      }

      NotNullInvariant.Check(result.ReturnObject, nameof(Fetch), isEntry: false);
      return result;
    }

    /// <inheritdoc />
    /// <remarks>
    /// The object must satisfy the not-null property invariant
    /// (<see cref="INotNullPropertyInfo"/>) before the operation starts (except
    /// for a command) and after it ran; otherwise <see cref="InvalidOperationException"/> is thrown.
    /// </remarks>
    public async Task<DataPortalResult> Update(ICslaObject obj, DataPortalContext context, bool isSync)
    {
      if (obj is null)
        throw new ArgumentNullException(nameof(obj));
      if (context is null)
        throw new ArgumentNullException(nameof(context));

      if (obj is not ICommandObject)
        NotNullInvariant.Check(obj, nameof(Update), isEntry: true);

      DataPortalResult result;
      if (DataPortalServer != null)
      {
        result = await DataPortalServer.Update(obj, context, isSync).ConfigureAwait(false);
      }
      else
      {
        result = await _dataPortalSelector.Update(obj, context, isSync).ConfigureAwait(false);
      }

      NotNullInvariant.Check(result.ReturnObject, nameof(Update), isEntry: false);
      return result;
    }

    /// <inheritdoc />
    public Task<DataPortalResult> Delete([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType, object criteria, DataPortalContext context, bool isSync)
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
        return _dataPortalSelector.Delete(objectType, criteria, context, isSync);
      }
    }
  }
}