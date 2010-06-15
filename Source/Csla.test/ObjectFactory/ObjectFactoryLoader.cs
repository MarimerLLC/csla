//-----------------------------------------------------------------------
// <copyright file="ObjectFactoryLoader.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
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

    #region IObjectFactoryLoader Members

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
          default:
            return new RootFactory();
        }
      }
      else
        return null;
    }

    #endregion
  }
}