//-----------------------------------------------------------------------
// <copyright file="IContextManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Defines the interface for an application </summary>
//-----------------------------------------------------------------------
using System;
using System.Security.Principal;

namespace Csla.Core
{
  /// <summary>
  /// Defines the interface for an application 
  /// context manager type.
  /// </summary>
  public interface IContextManager
  {
    /// <summary>
    /// Gets a value indicating whether this
    /// context manager is valid for use in
    /// the current environment.
    /// </summary>
    bool IsValid { get; }
    /// <summary>
    /// Gets the current principal.
    /// </summary>
    IPrincipal GetUser();
    /// <summary>
    /// Sets the current principal.
    /// </summary>
    /// <param name="principal">Principal object.</param>
    void SetUser(IPrincipal principal);
    /// <summary>
    /// Gets the local context.
    /// </summary>
    ContextDictionary GetLocalContext();
    /// <summary>
    /// Sets the local context.
    /// </summary>
    /// <param name="localContext">Local context.</param>
    void SetLocalContext(ContextDictionary localContext);
    /// <summary>
    /// Gets the client context.
    /// </summary>
    /// <param name="executionLocation"></param>
    ContextDictionary GetClientContext(ApplicationContext.ExecutionLocations executionLocation);
    /// <summary>
    /// Sets the client context.
    /// </summary>
    /// <param name="clientContext">Client context.</param>
    /// <param name="executionLocation"></param>
    void SetClientContext(ContextDictionary clientContext, ApplicationContext.ExecutionLocations executionLocation);
    /// <summary>
    /// Gets the global context.
    /// </summary>
    ContextDictionary GetGlobalContext();
    /// <summary>
    /// Sets the global context.
    /// </summary>
    /// <param name="globalContext">Global context.</param>
    void SetGlobalContext(ContextDictionary globalContext);
    /// <summary>
    /// Gets the service provider.
    /// </summary>
#pragma warning disable CS3002 // Return type is not CLS-compliant
    IServiceProvider GetServiceProvider();
#pragma warning restore CS3002 // Return type is not CLS-compliant
    /// <summary>
    /// Sets the service provider for current scope.
    /// </summary>
    /// <param name="provider">IServiceProvider instance</param>
#pragma warning disable CS3001 // Argument type is not CLS-compliant
    void SetServiceProvider(IServiceProvider provider);
#pragma warning restore CS3001 // Argument type is not CLS-compliant
  }
}