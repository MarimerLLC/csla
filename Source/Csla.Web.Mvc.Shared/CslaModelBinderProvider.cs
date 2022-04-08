//-----------------------------------------------------------------------
// <copyright file="CslaModelBinderProvider.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Model binder provider.</summary>
//-----------------------------------------------------------------------
#if NETSTANDARD2_0 || NET5_0_OR_GREATER || NETCOREAPP3_1
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Csla.Web.Mvc
{
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
    public CslaModelBinderProvider(ApplicationContext applicationContext)
    { 
      ApplicationContext = applicationContext;
    }

    /// <summary>
    /// Gets a reference to the current ApplicationContext.
    /// </summary>
    protected ApplicationContext ApplicationContext { get; private set; }

    /// <summary>
    /// Gets the CslaModelBinder provider.
    /// </summary>
    /// <param name="context">Model binder provider context.</param>
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
      if (typeof(Core.IEditableCollection).IsAssignableFrom(context.Metadata.ModelType) ||
          typeof(IBusinessBase).IsAssignableFrom(context.Metadata.ModelType))
        return new CslaModelBinder(ApplicationContext);
      else
        return null;
    }
  }
}
#endif