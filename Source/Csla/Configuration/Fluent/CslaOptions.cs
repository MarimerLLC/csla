//-----------------------------------------------------------------------
// <copyright file="CslaConfiguration.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Use this type to configure the settings for CSLA .NET</summary>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Csla.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Csla.Configuration
{
  /// <summary>
  /// Use this type to configure the settings for CSLA .NET.
  /// </summary>
  public class CslaOptions
  {
    /// <summary>
    /// Creates an instance of the type
    /// </summary>
    /// <param name="services">Services collection</param>
    /// <exception cref="ArgumentNullException"><paramref name="services"/> is <see langword="null"/>.</exception>
    public CslaOptions(IServiceCollection services)
    {
      Services = services ?? throw new ArgumentNullException(nameof(services));
      DataPortalOptions = new DataPortalOptions(this);
      SerializationOptions = new SerializationOptions(this);
    }

    /// <summary>
    /// Gets a reference to the current services collection.
    /// </summary>
    public IServiceCollection Services { get; }

    /// <summary>
    /// Sets the type for the IContextManager to 
    /// be used by ApplicationContext.
    /// </summary>
    public CslaOptions UseContextManager<T>() where T : IContextManager
    {
      ContextManagerType = typeof(T);
      return this;
    }

    /// <summary>
    /// Gets the type for the IContextManager 
    /// used by ApplicationContext.
    /// </summary>
    public Type? ContextManagerType { get; private set; }

    /// <summary>
    /// Sets a value indicating whether CSLA
    /// should fallback to using reflection instead of
    /// System.Linq.Expressions (true, default).
    /// </summary>
    /// <param name="value">Value</param>
    public CslaOptions UseReflectionFallback(bool value)
    {
      ApplicationContext.UseReflectionFallback = value;
      return this;
    }

    /// <summary>
    /// Gets or sets the default timeout in seconds
    /// for the WaitForIdle method.
    /// </summary>
    public int DefaultWaitForIdleTimeoutInSeconds { get; set; } = 90;

    /// <summary>
    /// Sets the factory type that creates PropertyInfo objects.
    /// </summary>
    public CslaOptions RegisterPropertyInfoFactory<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T>() where T : IPropertyInfoFactory
    {
      Core.FieldManager.PropertyInfoFactory.FactoryType = typeof(T);
      return this;
    }

    /// <summary>
    /// Indicates whether the data annotations scan is enabled.
    /// </summary>
    public bool ScanDataAnnotations { get; private set; } = true;

    /// <summary>
    /// Configures the scanning for data annotations based on the provided flag.
    /// </summary>
    /// <param name="flag">True to scan for data annotations, false to disable scanning. (default: true)</param>
    /// <returns>Returns the current instance of CslaOptions.</returns>
    public CslaOptions ScanForDataAnnotations(bool flag)
    {
      ScanDataAnnotations = flag;
      return this;
    }

    /// <summary>
    /// Gets the SecurityOptions instance.
    /// </summary>
    internal SecurityOptions SecurityOptions { get; } = new();
    /// <summary>
    /// Gets the SerializationOptions instance.
    /// </summary>
    public SerializationOptions SerializationOptions { get; }
    /// <summary>
    /// Gets the DataPortalClientOptions instance.
    /// </summary>
    internal DataPortalOptions DataPortalOptions { get; }
    /// <summary>
    /// Gets the DataOptions instance.
    /// </summary>
    public DataOptions DataOptions { get; } = new();
    /// <summary>
    /// Gets the DataOptions instance.
    /// </summary>
    public BindingOptions BindingOptions { get; } = new();
    /// <summary>
    /// Gets the CoreOptions instance.
    /// </summary>
    internal CoreOptions CoreOptions { get; } = new();
  }
}