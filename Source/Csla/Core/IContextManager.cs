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
    /// context manager is used in a stateful
    /// context (e.g. WPF, Blazor, etc.)
    /// </summary>
    bool IsStatefulContext { get; }

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
    /// Gets or sets a reference to the current ApplicationContext.
    /// </summary>
    ApplicationContext ApplicationContext { get; set; }
  }
}