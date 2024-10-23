//-----------------------------------------------------------------------
// <copyright file="UpdateRequest.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Request message for updating</summary>
//-----------------------------------------------------------------------

using Csla.Serialization.Mobile;

namespace Csla.Server.Hosts.DataPortalChannel
{
  /// <summary>
  /// Request message for updating
  /// a business object.
  /// </summary>
  [Serializable]
  public class UpdateRequest : ReadOnlyBase<UpdateRequest>
  {
    /// <summary>
    /// Serialized object data.
    /// </summary>
    public static readonly PropertyInfo<byte[]> ObjectDataProperty = RegisterProperty<byte[]>(nameof(ObjectData));

    /// <summary>
    /// Serialized object data.
    /// </summary>
    public byte[] ObjectData
    {
      get { return GetProperty(ObjectDataProperty); }
      set { LoadProperty(ObjectDataProperty, value ?? throw new ArgumentNullException(nameof(ObjectData))); }
    }

    /// <summary>
    /// Serialized data for the principal object.
    /// </summary>
    public static readonly PropertyInfo<byte[]> PrincipalProperty = RegisterProperty<byte[]>(c => c.Principal);

    /// <summary>
    /// Serialized data for the principal object.
    /// </summary>
    /// <exception cref="ArgumentNullException">value is <see langword="null"/>.</exception>
    public byte[] Principal
    {
      get { return GetProperty(PrincipalProperty); }
      set { LoadProperty(PrincipalProperty, value ?? throw new ArgumentNullException(nameof(Principal))); }
    }

    /// <summary>
    /// Serialized data for the client context object.
    /// </summary>
    public static readonly PropertyInfo<byte[]> ClientContextProperty = RegisterProperty<byte[]>(c => c.ClientContext);

    /// <summary>
    /// Serialized data for the client context object.
    /// </summary>
    /// <exception cref="ArgumentNullException">value is <see langword="null"/>.</exception>
    public byte[] ClientContext
    {
      get { return GetProperty(ClientContextProperty); }
      set { LoadProperty(ClientContextProperty, value ?? throw new ArgumentNullException(nameof(ClientContext))); }
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
    /// <exception cref="ArgumentNullException">value is <see langword="null"/>.</exception>
    public string ClientCulture
    {
      get { return GetProperty(ClientCultureProperty); }
      set { LoadProperty(ClientCultureProperty, value ?? throw new ArgumentNullException(nameof(ClientCulture))); }
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
    /// <exception cref="ArgumentNullException">value is <see langword="null"/>.</exception>
    public string ClientUICulture
    {
      get { return GetProperty(ClientUICultureProperty); }
      set { LoadProperty(ClientUICultureProperty, value ?? throw new ArgumentNullException(nameof(ClientUICulture))); }
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
    public UpdateRequest()
    {
    }
  }
}