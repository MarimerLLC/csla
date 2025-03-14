//-----------------------------------------------------------------------
// <copyright file="IContextManager.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Defines the interface for an application </summary>
//-----------------------------------------------------------------------
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
    /// <exception cref="ArgumentNullException"><paramref name="principal"/> is <see langword="null"/>.</exception>
    void SetUser(IPrincipal principal);
    /// <summary>
    /// Gets the local context.
    /// </summary>
    IContextDictionary? GetLocalContext();
    /// <summary>
    /// Sets the local context.
    /// </summary>
    void SetLocalContext(IContextDictionary? localContext);
    /// <summary>
    /// Gets the client context.
    /// </summary>
    IContextDictionary? GetClientContext(ApplicationContext.ExecutionLocations executionLocation);
    /// <summary>
    /// Sets the client context.
    /// </summary>
    void SetClientContext(IContextDictionary? clientContext, ApplicationContext.ExecutionLocations executionLocation);
    /// <summary>
    /// Gets or sets a reference to the current ApplicationContext.
    /// </summary>
    ApplicationContext? ApplicationContext { get; set; }
  }
}