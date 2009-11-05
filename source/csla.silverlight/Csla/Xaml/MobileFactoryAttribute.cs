using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Xaml
{
  /// <summary>
  /// Indicates that the data portal should create and invoke
  /// a mobile factory object on the web server that handles
  /// the client data portal call.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class)]
  public class MobileFactoryAttribute : Attribute
  {
    /// <summary>
    /// Assembly qualified type name of the mobile
    /// factory object that should be created by the
    /// data portal.
    /// </summary>
    public string FactoryTypeName { get; private set; }
    /// <summary>
    /// Name of the create method.
    /// </summary>
    public string CreateMethodName { get; private set; }
    /// <summary>
    /// Name of the fetch method.
    /// </summary>
    public string FetchMethodName { get; private set; }
    /// <summary>
    /// Name of the update method.
    /// </summary>
    public string UpdateMethodName { get; private set; }
    /// <summary>
    /// Name of the delete method.
    /// </summary>
    public string DeleteMethodName { get; private set; }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="factoryType">Assembly qualified type
    /// of the mobile factory class.</param>
    /// <param name="createMethod">Name of the create method.</param>
    /// <param name="fetchMethod">Name of the fetch method.</param>
    public MobileFactoryAttribute(string factoryType, string createMethod, string fetchMethod)
    {
      this.FactoryTypeName = factoryType;
      this.CreateMethodName = createMethod;
      this.FetchMethodName = fetchMethod;
    }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="factoryType">Assembly qualified type
    /// of the mobile factory class.</param>
    /// <param name="fetchMethod">Name of the fetch method.</param>
    public MobileFactoryAttribute(string factoryType, string fetchMethod)
    {
      this.FactoryTypeName = factoryType;
      this.FetchMethodName = fetchMethod;
    }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="factoryType">Assembly qualified type
    /// of the mobile factory class.</param>
    /// <param name="createMethod">Name of the create method.</param>
    /// <param name="fetchMethod">Name of the fetch method.</param>
    /// <param name="updateMethod">Name of the update method.</param>
    /// <param name="deleteMethod">Name of the delete method.</param>
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
