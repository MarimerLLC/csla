//-----------------------------------------------------------------------
// <copyright file="ObjectFactoryLoader.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

namespace Csla.Test.ObjectFactory
{
  public class ObjectFactoryLoader<T> : Server.IObjectFactoryLoader where T : class
  {
    private readonly ApplicationContext _applicationContext;
    public ObjectFactoryLoader(ApplicationContext applicationContext)
    {
      _applicationContext = applicationContext;
    }

    public Type GetFactoryType(string factoryName)
    {
      if (factoryName == "Csla.Test.ObjectFactory.RootFactory, Csla.Tests")
      {
        return typeof(T);
      }
      else
        return null;
    }

    public object GetFactory(string factoryName)
    {
      if (factoryName == "Csla.Test.ObjectFactory.RootFactory, Csla.Tests")
      {
        return _applicationContext.CreateInstanceDI<T>();
      }
      else
        return null;
    }
  }
}