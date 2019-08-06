//-----------------------------------------------------------------------
// <copyright file="ObjectFactoryAttribute.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Specifies that the data portal</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Reflection;

namespace Csla.Server
{
  /// <summary>
  /// Specifies that the data portal
  /// should invoke a factory object rather than
  /// the business object.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
  public class ObjectFactoryAttribute : Attribute
  {
    internal static ObjectFactoryAttribute GetObjectFactoryAttribute(Type objectType)
    {
      var result = objectType.GetCustomAttributes(typeof(ObjectFactoryAttribute), true);
      if (result != null && result.Length > 0)
        return result[0] as ObjectFactoryAttribute;
      else
        return null;
    }

    /// <summary>
    /// Assembly qualified type name of the factory object.
    /// </summary>
    /// <remarks>
    /// Factory class must have a parameterless 
    /// default constructor.
    /// </remarks>
    public string FactoryTypeName { get; private set; }
    /// <summary>
    /// Name of the method to call for a create operation.
    /// </summary>
    /// <remarks>
    /// The appropriate overload of this method will be
    /// invoked based on the parameters passed from the client.
    /// </remarks>
    public string CreateMethodName { get; private set; }
    /// <summary>
    /// Name of the method to call for a fetch operation.
    /// </summary>
    /// <remarks>
    /// The appropriate overload of this method will be
    /// invoked based on the parameters passed from the client.
    /// </remarks>
    public string FetchMethodName { get; private set; }
    /// <summary>
    /// Name of the method to call for a update operation.
    /// </summary>
    /// <remarks>
    /// The appropriate overload of this method will be
    /// invoked based on the parameters passed from the client.
    /// </remarks>
    public string UpdateMethodName { get; private set; }
    /// <summary>
    /// Name of the method to call for a delete operation.
    /// </summary>
    /// <remarks>
    /// The appropriate overload of this method will be
    /// invoked based on the parameters passed from the client.
    /// </remarks>
    public string DeleteMethodName { get; private set; }
    /// <summary>
    /// Name of the method to call for a Execute operation.
    /// </summary>
    /// <remarks>
    /// The appropriate overload of this method will be
    /// invoked based on the parameters passed from the client.
    /// </remarks>
    public string ExecuteMethodName { get; private set; }

    /// <summary>
    /// Creates an instance of the attribute.
    /// </summary>
    /// <param name="factoryType">
    /// Assembly qualified type name of the factory object.
    /// </param>
    /// <remarks>
    /// The method names default to Create, Fetch,
    /// Update and Delete.
    /// </remarks>
    public ObjectFactoryAttribute(string factoryType)
    {
      this.FactoryTypeName = factoryType;
      this.CreateMethodName = "Create";
      this.FetchMethodName = "Fetch";
      this.UpdateMethodName = "Update";
      this.DeleteMethodName = "Delete";
      this.ExecuteMethodName = "Execute";
    }

    /// <summary>
    /// Creates an instance of the attribute.
    /// </summary>
    /// <param name="factoryType">
    /// Assembly qualified type name of the factory object.
    /// </param>
    /// <param name="createMethod">
    /// Name of the method to call for a create operation.</param>
    /// <param name="fetchMethod">
    /// Name of the method to call for a fetch operation.
    /// </param>
    public ObjectFactoryAttribute(string factoryType, string createMethod, string fetchMethod)
    {
      this.FactoryTypeName = factoryType;
      this.CreateMethodName = createMethod;
      this.FetchMethodName = fetchMethod;
      this.UpdateMethodName = "Update";
      this.DeleteMethodName = "Delete";
      this.ExecuteMethodName = "Execute";
    }

    /// <summary>
    /// Creates an instance of the attribute.
    /// </summary>
    /// <param name="factoryType">
    /// Assembly qualified type name of the factory object.
    /// </param>
    /// <param name="fetchMethod">
    /// Name of the method to call for a fetch operation.
    /// </param>
    public ObjectFactoryAttribute(string factoryType, string fetchMethod)
    {
      this.FactoryTypeName = factoryType;
      this.FetchMethodName = fetchMethod;
      this.CreateMethodName = "Create";
      this.UpdateMethodName = "Update";
      this.DeleteMethodName = "Delete";
      this.ExecuteMethodName = "Execute";
    }


    /// <summary>
    /// Creates an instance of the attribute.
    /// </summary>
    /// <param name="factoryType">
    /// Assembly qualified type name of the factory object.
    /// </param>
    /// <param name="createMethod">
    /// Name of the method to call for a create operation.</param>
    /// <param name="fetchMethod">
    /// Name of the method to call for a fetch operation.
    /// </param>
    /// <param name="updateMethod">
    /// Name of the method to call for a update operation.</param>
    /// <param name="deleteMethod">
    /// Name of the method to call for a delete operation.</param>
    public ObjectFactoryAttribute(
      string factoryType, string createMethod, string fetchMethod, string updateMethod, string deleteMethod)
    {
      this.FactoryTypeName = factoryType;
      this.CreateMethodName = createMethod;
      this.FetchMethodName = fetchMethod;
      this.UpdateMethodName = updateMethod;
      this.DeleteMethodName = deleteMethod;
      this.ExecuteMethodName = "Execute";
    }

    /// <summary>
    /// Creates an instance of the attribute.
    /// </summary>
    /// <param name="factoryType">
    /// Assembly qualified type name of the factory object.
    /// </param>
    /// <param name="createMethod">
    /// Name of the method to call for a create operation.
    /// </param>
    /// <param name="fetchMethod">
    /// Name of the method to call for a fetch operation.
    /// </param>
    /// <param name="updateMethod">
    /// Name of the method to call for a update operation.
    /// </param>
    /// <param name="deleteMethod">
    /// Name of the method to call for a delete operation.
    /// </param>
    /// <param name="executeMethod">
    /// Name of the method to call for a Execute operation.
    /// </param>
    public ObjectFactoryAttribute(
      string factoryType, string createMethod, string fetchMethod, string updateMethod, string deleteMethod, string executeMethod)
    {
      this.FactoryTypeName = factoryType;
      this.CreateMethodName = createMethod;
      this.FetchMethodName = fetchMethod;
      this.UpdateMethodName = updateMethod;
      this.DeleteMethodName = deleteMethod;
      this.ExecuteMethodName = executeMethod;
    }

    /// <summary>
    /// Creates an instance of the attribute.
    /// </summary>
    /// <param name="factoryType">
    /// The type of factory class or interface.
    /// </param>
    public ObjectFactoryAttribute(Type factoryType)
      : this(GetAssemblyQualifiedName(factoryType))
    { }

    /// <summary>
    /// Creates an instance of the attribute.
    /// </summary>
    /// <param name="factoryType">
    /// The type of factory class or interface.
    /// </param>
    /// <param name="fetchMethod">
    /// Name of the method to call for a fetch operation.
    /// </param>
    public ObjectFactoryAttribute(Type factoryType, string fetchMethod)
      : this(GetAssemblyQualifiedName(factoryType), fetchMethod)
    { }

    /// <summary>
    /// Creates an instance of the attribute.
    /// </summary>
    /// <param name="factoryType">
    /// The type of factory class or interface.
    /// </param>
    /// <param name="createMethod">
    /// Name of the method to call for a create operation.
    /// </param>
    /// <param name="fetchMethod">
    /// Name of the method to call for a fetch operation.
    /// </param>
    public ObjectFactoryAttribute(Type factoryType, string createMethod, string fetchMethod)
      : this(GetAssemblyQualifiedName(factoryType), createMethod, fetchMethod)
    { }

    /// <summary>
    /// Creates an instance of the attribute.
    /// </summary>
    /// <param name="factoryType">
    /// The type of factory class or interface.
    /// </param>
    /// <param name="createMethod">
    /// Name of the method to call for a create operation.
    /// </param>
    /// <param name="fetchMethod">
    /// Name of the method to call for a fetch operation.
    /// </param>
    /// <param name="updateMethod">
    /// Name of the method to call for a update operation.
    /// </param>
    /// <param name="deleteMethod">
    /// Name of the method to call for a delete operation.
    /// </param>
    public ObjectFactoryAttribute(Type factoryType, string createMethod, string fetchMethod, string updateMethod, string deleteMethod)
      : this(GetAssemblyQualifiedName(factoryType), createMethod, fetchMethod, updateMethod, deleteMethod)
    { }

    /// <summary>
    /// Creates an instance of the attribute.
    /// </summary>
    /// <param name="factoryType">
    /// The type of factory class or interface.
    /// </param>
    /// <param name="createMethod">
    /// Name of the method to call for a create operation.
    /// </param>
    /// <param name="fetchMethod">
    /// Name of the method to call for a fetch operation.
    /// </param>
    /// <param name="updateMethod">
    /// Name of the method to call for a update operation.
    /// </param>
    /// <param name="deleteMethod">
    /// Name of the method to call for a delete operation.
    /// </param>
    /// <param name="executeMethod">
    /// Name of the method to call for a Execute operation.
    /// </param>
    public ObjectFactoryAttribute(Type factoryType, string createMethod, string fetchMethod, string updateMethod, string deleteMethod, string executeMethod)
      : this(GetAssemblyQualifiedName(factoryType), createMethod, fetchMethod, updateMethod, deleteMethod, executeMethod)
    { }

    /// <summary>
    /// Gets the short version of assembly qualified name.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns></returns>
    private static string GetAssemblyQualifiedName(Type type)
    {
#if NETFX_CORE
      if (type.IsGenericType())
#else
      if (type.IsGenericType)
#endif
      {
        return type.AssemblyQualifiedName;
      }
      else
      {
        if (type.AssemblyQualifiedName == null) return string.Empty;

        var elements = type.AssemblyQualifiedName.Split(',');
        return string.Format("{0},{1}", elements[0], elements[1]);

      }
    }
  }
}