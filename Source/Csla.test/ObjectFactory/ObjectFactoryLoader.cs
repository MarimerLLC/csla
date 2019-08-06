//-----------------------------------------------------------------------
// <copyright file="ObjectFactoryLoader.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Test.ObjectFactory
{
  public class ObjectFactoryLoader : Csla.Server.IObjectFactoryLoader
  {
    private int _mode;

    public ObjectFactoryLoader()
    {    }

    public ObjectFactoryLoader(int mode)
    {
      _mode = mode;
    }

    public Type GetFactoryType(string factoryName)
    {
      if (factoryName == "Csla.Test.ObjectFactory.RootFactory, Csla.Test")
      {
        switch (_mode)
        {
          case 1:
            return typeof(RootFactory1);
          case 2:
            return typeof(RootFactory2);
          case 3:
            return typeof(RootFactory3);
          case 4:
            return typeof(RootFactory4);
          case 5:
            return typeof(RootFactory5);
          case 6:
            return typeof(RootFactory6);
          case 7:
            return typeof(RootFactory7);
          case 8:
            return typeof(RootFactoryC);
          default:
            return typeof(RootFactory);
        }
      }
      else
        return null;
    }

    public object GetFactory(string factoryName)
    {
      if (factoryName == "Csla.Test.ObjectFactory.RootFactory, Csla.Test")
      {
        switch (_mode)
        {
          case 1:
            return new RootFactory1();
          case 2:
            return new RootFactory2();
          case 3:
            return new RootFactory3();
          case 4:
            return new RootFactory4();
          case 5:
            return new RootFactory5();
          case 6:
            return new RootFactory6();
          case 7:
            return new RootFactory7();
          case 8:
            return new RootFactoryC();
          default:
            return new RootFactory();
        }
      }
      else
        return null;
    }
  }
}