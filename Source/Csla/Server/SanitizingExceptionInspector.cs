//-----------------------------------------------------------------------
// <copyright file="SanitizingExceptionInspector.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Server-side sanitizing exception inspector.</summary>
//-----------------------------------------------------------------------
using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Csla.Server
{
  /// <summary>
  /// Sanitizing implementation of exception inspector, for hiding 
  /// sensitive information in exception details.
  /// </summary>
  /// <remarks>Only sanitizes exceptions from remote dataportals</remarks>
  public class SanitizingExceptionInspector : IDataPortalExceptionInspector
  {

    private readonly IHostEnvironment _hostEnvironment;
    private readonly ApplicationContext _applicationContext;
    private readonly ILogger _logger;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="environment">The host environment in which we are operating</param>
    /// <param name="applicationContext">The context of the current request</param>
    /// <param name="logger">The logger to which to log exceptions</param>
    public SanitizingExceptionInspector(IHostEnvironment environment, ApplicationContext applicationContext, ILogger<SanitizingExceptionInspector> logger)
    {
      _hostEnvironment = environment;
      _applicationContext = applicationContext;
      _logger = logger;
    }

    /// <summary>
    /// Logs the exception that occurred during DataPortal call, then
    /// throws a generic, less detailed exception for return to the client
    /// </summary>
    /// <param name="objectType">Type of the object.</param>
    /// <param name="businessObject">The business object, if available.</param>
    /// <param name="criteria">The criteria.</param>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="ex">The exception.</param>
    public void InspectException(Type objectType, object businessObject, object criteria, string methodName, Exception ex)
    {
      // Shortcut if we are not running on the server-side of a remote data portal operation
      if (_applicationContext.ExecutionLocation != ApplicationContext.ExecutionLocations.Server ||
        _applicationContext.LogicalExecutionLocation != ApplicationContext.LogicalExecutionLocations.Server)
        return;

      // Shortcut if we are running in development (max. developer productivity)
      if (_hostEnvironment.IsDevelopment())
        return;

      // Sanitize in all remaining scenarios
      string identifier = Guid.NewGuid().ToString();
      string message = Properties.Resources.ServerSideDataPortalException + Environment.NewLine + ex.ToString();
      _logger.LogError(message, identifier);
      throw new SanitizedServerSideDataPortalException(identifier);
    }
  }
}
