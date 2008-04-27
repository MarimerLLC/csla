using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Silverlight
{
  [AttributeUsage(AttributeTargets.Class)]
  public class MobileFactoryAttribute : Attribute
  {
    public string FactoryTypeName { get; private set; }
    public string CreateMethodName { get; private set; }
    public string FetchMethodName { get; private set; }
    public string UpdateMethodName { get; private set; }
    public string DeleteMethodName { get; private set; }

    public MobileFactoryAttribute(string factoryType, string createMethod, string fetchMethod)
    {
      this.FactoryTypeName = factoryType;
      this.CreateMethodName = createMethod;
      this.FetchMethodName = fetchMethod;
    }

    public MobileFactoryAttribute(string factoryType, string fetchMethod)
    {
      this.FactoryTypeName = factoryType;
      this.FetchMethodName = fetchMethod;
    }

    public MobileFactoryAttribute(
      string factoryType, string createMethod, string fetchMethod, string updateMethod, string deleteMethod)
    {
      this.FactoryTypeName = factoryType;
      this.CreateMethodName = createMethod;
      this.FetchMethodName = fetchMethod;
      this.UpdateMethodName = updateMethod;
      this.DeleteMethodName = deleteMethod;
    }
  }
}
