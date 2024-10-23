//-----------------------------------------------------------------------
// <copyright file="DataPortalSelector.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Selects the appropriate data portal implementation</summary>
//-----------------------------------------------------------------------

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
    private SimpleDataPortal SimpleDataPortal { get; }
    private FactoryDataPortal FactoryDataPortal { get; }
    private Configuration.DataPortalOptions DataPortalOptions { get; }

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
      _applicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
      SimpleDataPortal = simpleDataPortal ?? throw new ArgumentNullException(nameof(simpleDataPortal));
      FactoryDataPortal = factoryDataPortal ?? throw new ArgumentNullException(nameof(factoryDataPortal));
      DataPortalOptions = dataPortalOptions ?? throw new ArgumentNullException(nameof(dataPortalOptions));
    }

    /// <inheritdoc />
    public async Task<DataPortalResult> Create(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      try
      {
        context.FactoryInfo = ObjectFactoryAttribute.GetObjectFactoryAttribute(objectType);
        if (context.FactoryInfo == null)
        {
          return await SimpleDataPortal.Create(objectType, criteria, context, isSync).ConfigureAwait(false);
        }
        else
        {
          return await FactoryDataPortal.Create(objectType, criteria, context, isSync).ConfigureAwait(false);
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
          ex, null, DataPortalOptions);
      }
    }

    /// <inheritdoc />
    public async Task<DataPortalResult> Fetch(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      try
      {
        context.FactoryInfo = ObjectFactoryAttribute.GetObjectFactoryAttribute(objectType);
        if (context.FactoryInfo == null)
        {
          return await SimpleDataPortal.Fetch(objectType, criteria, context, isSync).ConfigureAwait(false);
        }
        else
        {
          return await FactoryDataPortal.Fetch(objectType, criteria, context, isSync).ConfigureAwait(false);
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
            ex, null, DataPortalOptions);
        else
          throw DataPortal.NewDataPortalException(
            _applicationContext, "DataPortal.Fetch " + Resources.FailedOnServer,
            ex, null, DataPortalOptions);
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
          return await SimpleDataPortal.Update(obj, context, isSync).ConfigureAwait(false);
        }
        else
        {
          return await FactoryDataPortal.Update(obj, context, isSync).ConfigureAwait(false);
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
          ex, obj, DataPortalOptions);
      }
    }

    /// <inheritdoc />
    public async Task<DataPortalResult> Delete(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      try
      {
        context.FactoryInfo = ObjectFactoryAttribute.GetObjectFactoryAttribute(objectType);
        if (context.FactoryInfo == null)
        {
          return await SimpleDataPortal.Delete(objectType, criteria, context, isSync).ConfigureAwait(false);
        }
        else
        {
          return await FactoryDataPortal.Delete(objectType, criteria, context, isSync).ConfigureAwait(false);
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
          ex, null, DataPortalOptions);
      }
    }
  }
}