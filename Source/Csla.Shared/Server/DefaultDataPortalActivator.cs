//-----------------------------------------------------------------------
// <copyright file="DefaultDataPortalActivator.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Defines a type used to activate concrete business instances.</summary>
//-----------------------------------------------------------------------
using Csla.Reflection;
using System;

namespace Csla.Server
{
  internal class DefaultDataPortalActivator : IDataPortalActivator
  {
    public object CreateInstance(Type requestedType)
    {
      return MethodCaller.CreateInstance(requestedType);
    }

    public void InitializeInstance(object obj)
    {
      // do no work by default
    }

    public void FinalizeInstance(object obj)
    {
      // do no work by default
    }
  }
}