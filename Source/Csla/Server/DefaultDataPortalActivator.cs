//-----------------------------------------------------------------------
// <copyright file="DefaultDataPortalActivator.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Defines a type used to activate concrete business instances.</summary>
//-----------------------------------------------------------------------
using Csla.Reflection;
using System;

namespace Csla.Server
{
  internal class DefaultDataPortalActivator : IDataPortalActivator, Core.IUseApplicationContext
  {
    /// <summary>
    /// Gets or sets the current ApplicationContext object.
    /// </summary>
    public ApplicationContext ApplicationContext { get; set; }

    public object CreateInstance(Type requestedType)
    {
      return ApplicationContext.CreateInstance(requestedType);
    }

    public void InitializeInstance(object obj)
    {
      // do no work by default
    }

    public void FinalizeInstance(object obj)
    {
      // do no work by default
    }

    public Type ResolveType(Type requestedType)
    {
      // return requested type by default
      return requestedType;
    }
  }
}