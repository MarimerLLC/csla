//-----------------------------------------------------------------------
// <copyright file="CslaModelBinderProvider.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Model binder provider.</summary>
//-----------------------------------------------------------------------
#if NETSTANDARD2_0 || NET5_0_OR_GREATER || NETCOREAPP3_1
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Options;

namespace Csla.Web.Mvc
{
  /// <summary>
  /// Configure options for CslaModelBinder
  /// </summary>
  public class CslaModelBinderMvcOptions : IConfigureOptions<MvcOptions>
  {
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Creates an instance of the type
    /// </summary>
    /// <param name="serviceProvider"></param>
    public CslaModelBinderMvcOptions(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    /// <summary>
    /// Configure options
    /// </summary>
    /// <param name="options">MvcOptions instance</param>
    public void Configure(MvcOptions options)
        => options.ModelBinderProviders.Add(new CslaModelBinderProvider(_serviceProvider, null, null));
  }

  /// <summary>
  /// Model binder provider that will use the CslaModelBinder for
  /// any type that implements the IBusinessBase interface.
  /// </summary>
  public class CslaModelBinderProvider : IModelBinderProvider
  {
    /// <summary>
    /// Creates a model binder provider that use custom
    /// instance and child creators.
    /// </summary>
    /// <param name="serviceProvider"></param>
    /// <param name="instanceCreator">Instance creator for root objects.</param>
    /// <param name="childCreator">Instance creator for child objects.</param>
    public CslaModelBinderProvider(IServiceProvider serviceProvider, Func<Type, Task<object>> instanceCreator, Func<IList, Type, Dictionary<string, string>, object> childCreator)
    {
      ApplicationContext = (ApplicationContext)serviceProvider.GetService(typeof(ApplicationContext));
      if (instanceCreator == null)
        _instanceCreator = CreateInstance;
      else
        _instanceCreator = instanceCreator;
      if (childCreator == null)
        _childCreator = CreateChild;
      else
        _childCreator = childCreator;
    }

    private ApplicationContext ApplicationContext { get; }

    internal Task<object> CreateInstance(Type type)
    {
      var tcs = new TaskCompletionSource<object>();
      tcs.SetResult(ApplicationContext.CreateInstanceDI(type));
      return tcs.Task;
    }

    internal object CreateChild(IList parent, Type type, Dictionary<string, string> values)
    {
      return ApplicationContext.CreateInstanceDI(type);
    }

    private readonly Func<Type, Task<object>> _instanceCreator;
    private readonly Func<IList, Type, Dictionary<string, string>, object> _childCreator;

    /// <summary>
    /// Gets the CslaModelBinder provider.
    /// </summary>
    /// <param name="context">Model binder provider context.</param>
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
      if (typeof(Core.IEditableCollection).IsAssignableFrom(context.Metadata.ModelType))
        return new CslaModelBinder(_instanceCreator, _childCreator);
      if (typeof(IBusinessBase).IsAssignableFrom(context.Metadata.ModelType))
        return new CslaModelBinder(_instanceCreator);
      return null;
    }
  }
}
#endif