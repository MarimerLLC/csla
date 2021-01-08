//-----------------------------------------------------------------------
// <copyright file="CriteriaRequest.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Message sent to the server</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csla;

namespace Csla.Server.Hosts.HttpChannel
{
  /// <summary>
  /// Message sent to the WCF data portal.
  /// </summary>
  [Serializable]
  public class CriteriaRequest : ReadOnlyBase<CriteriaRequest>
  {
    /// <summary>
    /// Assembly qualified name of the 
    /// business object type to create.
    /// </summary>
    public static readonly PropertyInfo<string> TypeNameProperty = RegisterProperty<string>(c => c.TypeName);
    /// <summary>
    /// Assembly qualified name of the 
    /// business object type to create.
    /// </summary>
    public string TypeName
    {
      get { return GetProperty(TypeNameProperty); }
      set { LoadProperty(TypeNameProperty, value); }
    }

    /// <summary>
    /// Serialized data for the criteria object.
    /// </summary>
    public static readonly PropertyInfo<byte[]> CriteriaDataProperty = RegisterProperty<byte[]>(c => c.CriteriaData);
    /// <summary>
    /// Serialized data for the criteria object.
    /// </summary>
    public byte[] CriteriaData
    {
      get { return GetProperty(CriteriaDataProperty); }
      set { LoadProperty(CriteriaDataProperty, value); }
    }

    /// <summary>
    /// Serialized data for the principal object.
    /// </summary>
    public static readonly PropertyInfo<byte[]> PrincipalProperty = RegisterProperty<byte[]>(c => c.Principal);
    /// <summary>
    /// Serialized data for the principal object.
    /// </summary>
    public byte[] Principal
    {
      get { return GetProperty(PrincipalProperty); }
      set { LoadProperty(PrincipalProperty, value); }
    }

    /// <summary>
    /// Serialized data for the global context object.
    /// </summary>
    public static readonly PropertyInfo<byte[]> GlobalContextProperty = RegisterProperty<byte[]>(c => c.GlobalContext);
    /// <summary>
    /// Serialized data for the global context object.
    /// </summary>
    public byte[] GlobalContext
    {
      get { return GetProperty(GlobalContextProperty); }
      set { LoadProperty(GlobalContextProperty, value); }
    }

    /// <summary>
    /// Serialized data for the client context object.
    /// </summary>
    public static readonly PropertyInfo<byte[]> ClientContextProperty = RegisterProperty<byte[]>(c => c.ClientContext);
    /// <summary>
    /// Serialized data for the client context object.
    /// </summary>
    public byte[] ClientContext
    {
      get { return GetProperty(ClientContextProperty); }
      set { LoadProperty(ClientContextProperty, value); }
    }

    /// <summary>
    /// Serialized client culture.
    /// </summary>
    /// <value>The client culture.</value>
    public static readonly PropertyInfo<string> ClientCultureProperty = RegisterProperty<string>(c => c.ClientCulture);
    /// <summary>
    /// Serialized client culture.
    /// </summary>
    /// <value>The client culture.</value>
    public string ClientCulture
    {
      get { return GetProperty(ClientCultureProperty); }
      set { LoadProperty(ClientCultureProperty, value); }
    }

    /// <summary>
    /// Serialized client UI culture.
    /// </summary>
    /// <value>The client UI culture.</value>
    public static readonly PropertyInfo<string> ClientUICultureProperty = RegisterProperty<string>(c => c.ClientUICulture);
    /// <summary>
    /// Serialized client UI culture.
    /// </summary>
    /// <value>The client UI culture.</value>
    public string ClientUICulture
    {
      get { return GetProperty(ClientUICultureProperty); }
      set { LoadProperty(ClientUICultureProperty, value); }
    }
  }
}
