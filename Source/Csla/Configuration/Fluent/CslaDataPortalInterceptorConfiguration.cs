using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Csla.Configuration
{

  /// <summary>
  /// Extension methods supporting external manipulation of data portal interceptors
  /// </summary>
  public static class CslaDataPortalInterceptorConfigurationExtensions
  {

    /// <summary>
    /// Add an interceptor to the pipeline of interceptors in use
    /// </summary>
    /// <typeparam name="T">The type of interceptor to add to the end of the pipeline</typeparam>
    /// <param name="builder">The CslaDataPortalInterceptorConfiguration instance that we extend</param>
    /// <returns>The CslaDataPortalInterceptorConfiguration provided, to support method chaining/fluent configuration</returns>
    public static CslaDataPortalInterceptorConfiguration Add<T>(this CslaDataPortalInterceptorConfiguration builder) where T : class, Server.IInterceptDataPortal
    {
      builder.AddInterceptor<T>();
      return builder;
    }

    /// <summary>
    /// Remove an interceptor from the pipeline of interceptors in use
    /// </summary>
    /// <typeparam name="T">The type of interceptor to add to the end of the pipeline</typeparam>
    /// <param name="builder">The CslaDataPortalInterceptorConfiguration instance that we extend</param>
    /// <returns>The CslaDataPortalInterceptorConfiguration provided, to support method chaining/fluent configuration</returns>
    public static CslaDataPortalInterceptorConfiguration Remove<T>(this CslaDataPortalInterceptorConfiguration builder) where T : class, Server.IInterceptDataPortal
    {
      builder.RemoveInterceptor<T>();
      return builder;
    }

    /// <summary>
    /// Perform registration of the required interceptors using prebuilt configuration
    /// </summary>
    /// <param name="builder">The interceptor configuration that we are extending</param>
    internal static void ApplyFinalConfiguration(this CslaDataPortalInterceptorConfiguration builder)
    {
      IServiceCollection services = builder.CslaConfiguration.Services;

      // Add the interception manager
      services.AddTransient<Server.InterceptionManager>();
      
      // Add each of the configured interceptors in order of addition
      foreach (Type interceptorType in builder.RequiredInterceptors)
      {
        services.AddTransient(typeof(Server.IInterceptDataPortal), interceptorType);
      }
    }

  }

  /// <summary>
  /// Configuration of data portal interceptors
  /// </summary>
  public class CslaDataPortalInterceptorConfiguration
  {
    private readonly ICslaConfiguration _cslaConfiguration;
    private readonly List<Type> _dataPortalInterceptors = new List<Type>();

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="builder">The root CslaConfiguration of which we form a part</param>
    internal CslaDataPortalInterceptorConfiguration(ICslaConfiguration builder)
    {
      _cslaConfiguration = builder;

      // Add the default interceptors (secure by default configuration)
      _dataPortalInterceptors.Add(typeof(Server.Interceptors.ServerSide.RevalidatingInterceptor));
    }

    /// <summary>
    /// Expose the root CslaConfiguration with which we are associated
    /// </summary>
    public ICslaConfiguration CslaConfiguration { get => _cslaConfiguration; }

    /// <summary>
    /// Add a new interceptor to the pipeline of interceptors required
    /// </summary>
    /// <typeparam name="T">The type of the interceptor being added</typeparam>
    internal void AddInterceptor<T>() where T: class, Server.IInterceptDataPortal
    {
      _dataPortalInterceptors.Add(typeof(T));
    }

    /// <summary>
    /// Remove an interceptor from the pipeline of interceptors required
    /// </summary>
    /// <typeparam name="T">The type of the interceptor being added</typeparam>
    internal void RemoveInterceptor<T>() where T : class, Server.IInterceptDataPortal
    {
      Type interceptor;

      interceptor = _dataPortalInterceptors.FirstOrDefault(i => i == typeof(T));
      if (interceptor is not null)
      { 
        _dataPortalInterceptors.Remove(interceptor);
      }
      
    }

    /// <summary>
    /// Expose the list of interceptors configured for use
    /// </summary>
    internal IReadOnlyList<Type> RequiredInterceptors { get => _dataPortalInterceptors; }

  }
}
