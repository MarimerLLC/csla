//-----------------------------------------------------------------------
// <copyright file="MobileFactoryAttribute.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Specifies that the WCF data portal</summary>
//-----------------------------------------------------------------------
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Server
{
  /// <summary>
  /// Specifies that the WCF data portal
  /// should invoke a factory object rather than
  /// the business object.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class)]
  public class MobileFactoryAttribute : Attribute
  {
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
    /// Name of the method to call for a execute operation.
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
    /// The default names for the factory methods are
    /// Create(), Fetch(), Update() and Delete().
    /// </remarks>
    public MobileFactoryAttribute(string factoryType)
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
    public MobileFactoryAttribute(string factoryType, string createMethod, string fetchMethod)
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
    public MobileFactoryAttribute(string factoryType, string fetchMethod)
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
    public MobileFactoryAttribute(
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
    /// Name of the method to call for a create operation.</param>
    /// <param name="fetchMethod">
    /// Name of the method to call for a fetch operation.
    /// </param>
    /// <param name="updateMethod">
    /// Name of the method to call for a update operation.</param>
    /// <param name="deleteMethod">
    /// Name of the method to call for a delete operation.</param>
    /// <param name="executeMethod">
    /// Name of the method to call for a execute operation.</param>
    public MobileFactoryAttribute(
      string factoryType, string createMethod, string fetchMethod, string updateMethod, string deleteMethod, string executeMethod)
    {
      this.FactoryTypeName = factoryType;
      this.CreateMethodName = createMethod;
      this.FetchMethodName = fetchMethod;
      this.UpdateMethodName = updateMethod;
      this.DeleteMethodName = deleteMethod;
      this.ExecuteMethodName = executeMethod;
    }

  }
}