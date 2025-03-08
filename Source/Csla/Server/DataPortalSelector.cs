//-----------------------------------------------------------------------
// <copyright file="DataPortalSelector.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Selects the appropriate data portal implementation</summary>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Csla.Properties;

namespace Csla.Server
{
  /// <summary>
  /// Selects the appropriate data portal implementation
  /// to invoke based on the object and configuration.
  /// </summary>
  public class DataPortalSelector : IDataPortalServer
  {
    private readonly ApplicationContext _applicationContext;
    private readonly SimpleDataPortal _simpleDataPortal;
    private readonly FactoryDataPortal _factoryDataPortal;
    private readonly Configuration.DataPortalOptions _dataPortalOptions;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="applicationContext"></param>
    /// <param name="simpleDataPortal"></param>
    /// <param name="factoryDataPortal"></param>
    /// <param name="dataPortalOptions"></param>
    /// <exception cref="ArgumentNullException"><paramref name="applicationContext"/>, <paramref name="simpleDataPortal"/>, <paramref name="factoryDataPortal"/> or <paramref name="dataPortalOptions"/> is <see langword="null"/>.</exception>
    public DataPortalSelector(ApplicationContext applicationContext, SimpleDataPortal simpleDataPortal, FactoryDataPortal factoryDataPortal, Configuration.DataPortalOptions dataPortalOptions)
    {
      _applicationContext = Guard.NotNull(applicationContext);
      _simpleDataPortal = Guard.NotNull(simpleDataPortal);
      _factoryDataPortal = Guard.NotNull(factoryDataPortal);
      _dataPortalOptions = Guard.NotNull(dataPortalOptions);
    }

    /// <inheritdoc />
    public async Task<DataPortalResult> Create([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      try
      {
        context.FactoryInfo = ObjectFactoryAttribute.GetObjectFactoryAttribute(objectType);
        if (context.FactoryInfo == null)
        {
          return await _simpleDataPortal.Create(objectType, criteria, context, isSync).ConfigureAwait(false);
        }
        else
        {
          return await _factoryDataPortal.Create(objectType, criteria, context, isSync).ConfigureAwait(false);
        }
      }
      catch (DataPortalException)
      {
        throw;
      }
      catch (Exception ex)
      {
        throw DataPortal.NewDataPortalException(
          _applicationContext, "DataPortal.Create " + Resources.FailedOnServer,
          ex, null, _dataPortalOptions);
      }
    }

    /// <inheritdoc />
    public async Task<DataPortalResult> Fetch([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      try
      {
        context.FactoryInfo = ObjectFactoryAttribute.GetObjectFactoryAttribute(objectType);
        if (context.FactoryInfo == null)
        {
          return await _simpleDataPortal.Fetch(objectType, criteria, context, isSync).ConfigureAwait(false);
        }
        else
        {
          return await _factoryDataPortal.Fetch(objectType, criteria, context, isSync).ConfigureAwait(false);
        }
      }
      catch (DataPortalException)
      {
        throw;
      }
      catch (Exception ex)
      {
        if (typeof(Core.ICommandObject).IsAssignableFrom(objectType))
          throw DataPortal.NewDataPortalException(
            _applicationContext, "DataPortal.Execute " + Resources.FailedOnServer,
            ex, null, _dataPortalOptions);
        else
          throw DataPortal.NewDataPortalException(
            _applicationContext, "DataPortal.Fetch " + Resources.FailedOnServer,
            ex, null, _dataPortalOptions);
      }
    }

    /// <inheritdoc />
    public async Task<DataPortalResult> Update(object obj, DataPortalContext context, bool isSync)
    {
      try
      {
        context.FactoryInfo = ObjectFactoryAttribute.GetObjectFactoryAttribute(obj.GetType());
        if (context.FactoryInfo == null)
        {
          return await _simpleDataPortal.Update(obj, context, isSync).ConfigureAwait(false);
        }
        else
        {
          return await _factoryDataPortal.Update(obj, context, isSync).ConfigureAwait(false);
        }
      }
      catch (DataPortalException)
      {
        throw;
      }
      catch (Exception ex)
      {
        throw DataPortal.NewDataPortalException(
          _applicationContext, "DataPortal.Update " + Resources.FailedOnServer,
          ex, obj, _dataPortalOptions);
      }
    }

    /// <inheritdoc />
    public async Task<DataPortalResult> Delete([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      try
      {
        context.FactoryInfo = ObjectFactoryAttribute.GetObjectFactoryAttribute(objectType);
        if (context.FactoryInfo == null)
        {
          return await _simpleDataPortal.Delete(objectType, criteria, context, isSync).ConfigureAwait(false);
        }
        else
        {
          return await _factoryDataPortal.Delete(objectType, criteria, context, isSync).ConfigureAwait(false);
        }
      }
      catch (DataPortalException)
      {
        throw;
      }
      catch (Exception ex)
      {
        throw DataPortal.NewDataPortalException(
          _applicationContext, "DataPortal.Delete " + Resources.FailedOnServer,
          ex, null, _dataPortalOptions);
      }
    }
  }
}