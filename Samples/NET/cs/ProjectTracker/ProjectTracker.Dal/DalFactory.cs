using System;
using System.Configuration;

namespace ProjectTracker.Dal
{
  public static class DalFactory
  {
    private static Type _dalType;

    public static IDalManager GetManager()
    {
      if (_dalType == null)
      {
        var dalTypeName = ConfigurationManager.AppSettings["DalManagerType"];
        if (!string.IsNullOrEmpty(dalTypeName))
          _dalType = Type.GetType(dalTypeName);
        else
          throw new NullReferenceException("DalManagerType");
        if (_dalType == null)
          throw new ArgumentException(string.Format("Type {0} could not be found", dalTypeName));
      }
      return (IDalManager)Activator.CreateInstance(_dalType);
    }
  }
}
