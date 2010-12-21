using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectTracker.Dal
{
  public static class DalFactory
  {
    private static IDalFactory _factory;
    public static IDalFactory FactoryInstance
    {
      get
      {
        if (_factory == null)
        {
          var dalTypeName = System.Configuration.ConfigurationManager.AppSettings["DalTypeName"];
          var dalType = Type.GetType(dalTypeName);
          _factory = (IDalFactory)Activator.CreateInstance(dalType);
        }
        return _factory;
      }
      set
      {
        _factory = value;
      }
    }

    public static T GetDal<T>()
    {
      return FactoryInstance.GetDal<T>();
    }
  }
}
