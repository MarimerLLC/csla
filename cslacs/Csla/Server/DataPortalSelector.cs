using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Properties;

namespace Csla.Server
{
  /// <summary>
  /// Selects the appropriate data portal implementation
  /// to invoke based on the object and configuration.
  /// </summary>
  public class DataPortalSelector : IDataPortalServer
  {
    #region IDataPortalServer Members

    /// <summary>
    /// Create a new business object.
    /// </summary>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    public DataPortalResult Create(Type objectType, object criteria, DataPortalContext context)
    {
      try
      {
        context.FactoryInfo = ObjectFactoryAttribute.GetObjectFactoryAttribute(objectType);
        if (context.FactoryInfo == null)
        {
          var dp = new SimpleDataPortal();
          return dp.Create(objectType, criteria, context);
        }
        else
        {
          var dp = new FactoryDataPortal();
          return dp.Create(objectType, criteria, context);
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
    public DataPortalResult Fetch(Type objectType, object criteria, DataPortalContext context)
    {
      try
      {
        context.FactoryInfo = ObjectFactoryAttribute.GetObjectFactoryAttribute(objectType);
        if (context.FactoryInfo == null)
        {
          var dp = new SimpleDataPortal();
          return dp.Fetch(objectType, criteria, context);
        }
        else
        {
          var dp = new FactoryDataPortal();
          return dp.Fetch(objectType, criteria, context);
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
    public DataPortalResult Update(object obj, DataPortalContext context)
    {
      try
      {
        context.FactoryInfo = ObjectFactoryAttribute.GetObjectFactoryAttribute(obj.GetType());
        if (context.FactoryInfo == null)
        {
          var dp = new SimpleDataPortal();
          return dp.Update(obj, context);
        }
        else
        {
          var dp = new FactoryDataPortal();
          return dp.Update(obj, context);
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
    public DataPortalResult Delete(Type objectType, object criteria, DataPortalContext context)
    {
      try
      {
        context.FactoryInfo = ObjectFactoryAttribute.GetObjectFactoryAttribute(objectType);
        if (context.FactoryInfo == null)
        {
          var dp = new SimpleDataPortal();
          return dp.Delete(objectType, criteria, context);
        }
        else
        {
          var dp = new FactoryDataPortal();
          return dp.Delete(objectType, criteria, context);
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

    #endregion
  }
}
