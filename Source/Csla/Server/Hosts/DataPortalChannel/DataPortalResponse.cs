//-----------------------------------------------------------------------
// <copyright file="HttpResponse.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Response message for returning</summary>
//-----------------------------------------------------------------------

using System;

namespace Csla.Server.Hosts.DataPortalChannel
{
  /// <summary>
  /// Response message for returning
  /// the results of a data portal call.
  /// </summary>
  [Serializable]
  public class DataPortalResponse : ReadOnlyBase<DataPortalResponse>
  {
    /// <summary>
    /// Server-side exception data if an exception occurred on the server.
    /// </summary>
    public static readonly PropertyInfo<DataPortalErrorInfo> ErrorDataProperty = RegisterProperty<DataPortalErrorInfo>(c => c.ErrorData);

    /// <summary>
    /// Server-side exception data if an exception occurred on the server.
    /// </summary>
    public DataPortalErrorInfo ErrorData
    {
      get { return GetProperty(ErrorDataProperty); }
      set { LoadProperty(ErrorDataProperty, value); }
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