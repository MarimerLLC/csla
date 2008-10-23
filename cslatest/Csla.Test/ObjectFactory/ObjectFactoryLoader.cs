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
