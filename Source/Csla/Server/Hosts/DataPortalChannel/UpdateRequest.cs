//-----------------------------------------------------------------------
// <copyright file="UpdateRequest.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Request message for updating</summary>
//-----------------------------------------------------------------------

using Csla.Serialization;
using Csla.Serialization.Mobile;

namespace Csla.Server.Hosts.DataPortalChannel
{
  /// <summary>
  /// Request message for updating
  /// a business object.
  /// </summary>
  [AutoSerializable]
  public partial class UpdateRequest
  {
    /// <summary>
    /// Serialized object data.
    /// </summary>
    public byte[] ObjectData
    {
      get;
      private set;
    }

    /// <summary>
    /// Serialized data for the principal object.
    /// </summary>
    /// <exception cref="ArgumentNullException">value is <see langword="null"/>.</exception>
    public byte[] Principal
    {
      get;
      private set;
    }

    /// <summary>
    /// Serialized data for the client context object.
    /// </summary>
    /// <exception cref="ArgumentNullException">value is <see langword="null"/>.</exception>
    public byte[] ClientContext
    {
      get;
      private set;
    }

    /// <summary>
    /// Serialized client culture.
    /// </summary>
    /// <value>The client culture.</value>
    /// <exception cref="ArgumentNullException">value is <see langword="null"/>.</exception>
    public string ClientCulture
    {
      get;
      private set;
    }

    /// <summary>
    /// Serialized client UI culture.
    /// </summary>
    /// <value>The client UI culture.</value>
    /// <exception cref="ArgumentNullException">value is <see langword="null"/>.</exception>
    public string ClientUICulture
    {
      get;
      private set;
    }

    /// <summary>
    /// Initializes a new instance of <see cref="UpdateRequest"/>-object.
    /// </summary>
    /// <param name="principal"></param>
    /// <param name="clientContext"></param>
    /// <param name="clientCulture"></param>
    /// <param name="clientUICulture"></param>
    /// <param name="objectData"></param>
    /// <exception cref="ArgumentNullException"><paramref name="principal"/>, <paramref name="clientContext"/>, <paramref name="clientCulture"/>, <paramref name="clientUICulture"/> or <paramref name="objectData"/> is <see langword="null"/>.</exception>
    public UpdateRequest(byte[] principal, byte[] clientContext, string clientCulture, string clientUICulture, byte[] objectData)
    {
      Principal = principal ?? throw new ArgumentNullException(nameof(principal));
      ClientContext = clientContext ?? throw new ArgumentNullException(nameof(clientContext));
      ClientCulture = clientCulture ?? throw new ArgumentNullException(nameof(clientCulture));
      ClientUICulture = clientUICulture ?? throw new ArgumentNullException(nameof(clientUICulture));
      ObjectData = objectData ?? throw new ArgumentNullException(nameof(objectData));
    }

    /// <summary>
    /// Initializes an empty instance for <see cref="MobileFormatter"/>.
    /// </summary>
    [Obsolete(MobileFormatter.DefaultCtorObsoleteMessage, error: true)]
    public UpdateRequest() : this([], [], string.Empty, string.Empty, [])
    {
    }
  }
}