//-----------------------------------------------------------------------
// <copyright file="HttpResponse.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Response message for returning</summary>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Csla.Core;
using Csla.Serialization;
using Csla.Serialization.Mobile;

namespace Csla.Server.Hosts.DataPortalChannel
{
  /// <summary>
  /// Response message for returning
  /// the results of a data portal call.
  /// </summary>
  [AutoSerializable]
  public partial class DataPortalResponse
  {
    /// <summary>
    /// Indicates whether <see cref="ErrorData"/> are present or not.
    /// </summary>
    [MemberNotNullWhen(true, nameof(ErrorData))]
    [MemberNotNullWhen(false, nameof(ObjectData))]
    public bool HasError => ErrorData is not null;

    /// <summary>
    /// Server-side exception data if an exception occurred on the server.
    /// </summary>
    public DataPortalErrorInfo? ErrorData
    {
      get;
      set;
    }

    /// <summary>
    /// Serialized business object data returned from the server (deserialize with MobileFormatter).
    /// </summary>
    public byte[]? ObjectData
    {
      get;
      set;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="DataPortalResponse"/>-object.
    /// </summary>
    /// <param name="errorData"></param>
    /// <exception cref="ArgumentNullException"><paramref name="errorData"/> is <see langword="null"/>.</exception>
    public DataPortalResponse(DataPortalErrorInfo errorData)
    {
      ErrorData = Guard.NotNull(errorData);
    }

    /// <summary>
    /// Initializes a new instance of <see cref="DataPortalResponse"/>-object.
    /// </summary>
    /// <param name="objectData"></param>
    /// <exception cref="ArgumentNullException"><paramref name="objectData"/> is <see langword="null"/>.</exception>
    public DataPortalResponse(byte[] objectData)
    {
      ObjectData = Guard.NotNull(objectData);
    }

    /// <summary>
    /// Initializes an empty instance for <see cref="MobileFormatter"/>.
    /// </summary>
    public DataPortalResponse()
    {
    }
  }
}