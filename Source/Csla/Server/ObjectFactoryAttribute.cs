﻿//-----------------------------------------------------------------------
// <copyright file="ObjectFactoryAttribute.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Specifies that the data portal</summary>
//-----------------------------------------------------------------------

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
      FactoryTypeName = factoryType;
      CreateMethodName = "Create";
      FetchMethodName = "Fetch";
      UpdateMethodName = "Update";
      DeleteMethodName = "Delete";
      ExecuteMethodName = "Execute";
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
      FactoryTypeName = factoryType;
      CreateMethodName = createMethod;
      FetchMethodName = fetchMethod;
      UpdateMethodName = "Update";
      DeleteMethodName = "Delete";
      ExecuteMethodName = "Execute";
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
      FactoryTypeName = factoryType;
      FetchMethodName = fetchMethod;
      CreateMethodName = "Create";
      UpdateMethodName = "Update";
      DeleteMethodName = "Delete";
      ExecuteMethodName = "Execute";
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
      FactoryTypeName = factoryType;
      CreateMethodName = createMethod;
      FetchMethodName = fetchMethod;
      UpdateMethodName = updateMethod;
      DeleteMethodName = deleteMethod;
      ExecuteMethodName = "Execute";
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
      FactoryTypeName = factoryType;
      CreateMethodName = createMethod;
      FetchMethodName = fetchMethod;
      UpdateMethodName = updateMethod;
      DeleteMethodName = deleteMethod;
      ExecuteMethodName = executeMethod;
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
    private static string GetAssemblyQualifiedName(Type type)
    {
      if (type.IsGenericType)
      {
        return type.AssemblyQualifiedName;
      }
      else
      {
        if (type.AssemblyQualifiedName == null) return string.Empty;

        var elements = type.AssemblyQualifiedName.Split(',');
        return $"{elements[0]},{elements[1]}";
      }
    }
  }
}