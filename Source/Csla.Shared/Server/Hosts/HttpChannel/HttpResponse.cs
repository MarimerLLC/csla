//-----------------------------------------------------------------------
// <copyright file="HttpResponse.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Response message for returning</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csla.Server.Hosts.HttpChannel
{
  /// <summary>
  /// Response message for returning
  /// the results of a data portal call.
  /// </summary>
  [Serializable]
  public class HttpResponse : ReadOnlyBase<HttpResponse>
  {
    /// <summary>
    /// Server-side exception data if an exception occurred on the server.
    /// </summary>
    public static readonly PropertyInfo<HttpErrorInfo> ErrorDataProperty = RegisterProperty<HttpErrorInfo>(c => c.ErrorData);
    /// <summary>
    /// Server-side exception data if an exception occurred on the server.
    /// </summary>
    public HttpErrorInfo ErrorData
    {
      get { return GetProperty(ErrorDataProperty); }
      set { LoadProperty(ErrorDataProperty, value); }
    }
    /// <summary>
    /// Serialized GlobalContext data returned from the server (deserialize with MobileFormatter).
    /// </summary>
    public static readonly PropertyInfo<byte[]> GlobalContextProperty = RegisterProperty<byte[]>(c => c.GlobalContext);
    /// <summary>
    /// Serialized GlobalContext data returned from the server (deserialize with MobileFormatter).
    /// </summary>
    public byte[] GlobalContext
    {
      get { return GetProperty(GlobalContextProperty); }
      set { LoadProperty(GlobalContextProperty, value); }
    }
    /// <summary>
    /// Serialized business object data returned from the server (deserialize with MobileFormatter).
    /// </summary>
    public static readonly PropertyInfo<byte[]> ObjectDataProperty = RegisterProperty<byte[]>(c => c.ObjectData);
    /// <summary>
    /// Serialized business object data returned from the server (deserialize with MobileFormatter).
    /// </summary>
    public byte[] ObjectData
    {
      get { return GetProperty(ObjectDataProperty); }
      set { LoadProperty(ObjectDataProperty, value); }
    }
  }
}
