//-----------------------------------------------------------------------
// <copyright file="RevalidatingInterceptor.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Initiates revalidation on business objects during data portal operations</summary>
//-----------------------------------------------------------------------

using System.Collections;
using Csla.Core;
using Csla.Properties;

namespace Csla.Server.Interceptors.ServerSide
{
  /// <summary>
  /// DataPortal interceptor to perform revalidation on business objects
  /// </summary>
  public class RevalidatingInterceptor : IInterceptDataPortal
  {
    private readonly ApplicationContext _applicationContext;

    /// <summary>
    /// Public constructor, intended to be executed by DI
    /// </summary>
    /// <param name="applicationContext">The context under which the DataPortal operation is executing</param>
    /// <exception cref="ArgumentNullException"><paramref name="applicationContext"/> is <see langword="null"/>.</exception>
    public RevalidatingInterceptor(ApplicationContext applicationContext)
    {
      _applicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
    }

    /// <summary>
    /// Interception handler run before a DataPortal operation
    /// </summary>
    /// <param name="e">The interception arguments from the DataPortal</param>
    /// <exception cref="Rules.ValidationException"></exception>
    /// <exception cref="ArgumentNullException"><paramref name="e"/> is <see langword="null"/>.</exception>
    public async Task InitializeAsync(InterceptArgs e)
    {
      if (_applicationContext.ExecutionLocation != ApplicationContext.ExecutionLocations.Server
        || _applicationContext.LogicalExecutionLocation != ApplicationContext.LogicalExecutionLocations.Server)
      {
        return;
      }

      if (e is null)
        throw new ArgumentNullException(nameof(e));

      if (e.Parameter is not ITrackStatus checkableObject)
      {
        return;
      }
      
      await RevalidateObjectAsync(checkableObject);
      if (!checkableObject.IsValid)
      {
        throw new Rules.ValidationException(Resources.NoSaveInvalidException);
      }
    }

    /// <inheritdoc />
    public void Complete(InterceptArgs e)
    {
    }

    /// <summary>
    /// Perform revalidation of business rules on any supporting type
    /// </summary>
    /// <param name="parameter">The parameter that was passed to the DataPortal as part of the operation</param>
    private async Task RevalidateObjectAsync(object? parameter)
    {
      if (parameter is null)
      {
        return;
      }

      if (parameter is IEnumerable list)
      {
        // Handle the object being a collection
        foreach (object item in list)
        {
          await RevalidateObjectAsync(item);
        }
      }
      else
      {
        if (parameter is ICheckRules checkableObject)
        {
          // object not a collection, see if it has rules
          // wait for all async rules to complete before proceeding
          await checkableObject.CheckRulesAsync();
        }

        // Cascade to any child objects
        if (parameter is IUseFieldManager fieldHolder)
        {
          var properties = fieldHolder.FieldManager.GetRegisteredProperties();
          foreach (var property in properties.Where(r=>r.IsChild && fieldHolder.FieldManager.FieldExists(r)))
          {
            var fieldData = fieldHolder.FieldManager.GetFieldData(property);
            if (fieldData is null)
            {
              continue;
            }
            await RevalidateObjectAsync(fieldData.Value);
          }
        }
        else if (parameter is IManageProperties propertyHolder)
        {
          foreach (object child in propertyHolder.GetChildren())
          {
            await RevalidateObjectAsync(child);
          }
        }
      }
    }
  }
}
