//-----------------------------------------------------------------------
// <copyright file="CriteriaRequest.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Message sent to the server</summary>
//-----------------------------------------------------------------------

using Csla.Serialization.Mobile;

namespace Csla.Server.Hosts.DataPortalChannel
{
  /// <summary>
  /// Message sent to the data portal.
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
    /// <exception cref="ArgumentNullException">value  is <see langword="null"/>.</exception>
    public string TypeName
    {
      get => GetProperty(TypeNameProperty)!;
      set => LoadProperty(TypeNameProperty, value ?? throw new ArgumentNullException(nameof(TypeName)));
    }

    /// <summary>
    /// Serialized data for the criteria object.
    /// </summary>
    public static readonly PropertyInfo<byte[]> CriteriaDataProperty = RegisterProperty<byte[]>(nameof(CriteriaData));

    /// <summary>
    /// Serialized data for the criteria object.
    /// </summary>
    public byte[] CriteriaData
    {
      get => GetProperty(CriteriaDataProperty)!;
      set => LoadProperty(CriteriaDataProperty, value ?? throw new ArgumentNullException(nameof(CriteriaData)));
    }

    /// <summary>
    /// Serialized data for the principal object.
    /// </summary>
    public static readonly PropertyInfo<byte[]> PrincipalProperty = RegisterProperty<byte[]>(nameof(Principal));

    /// <summary>
    /// Serialized data for the principal object.
    /// </summary>
    /// <exception cref="ArgumentNullException">value is <see langword="null"/>.</exception>
    public byte[] Principal
    {
      get => GetProperty(PrincipalProperty)!;
      set => LoadProperty(PrincipalProperty, value ?? throw new ArgumentNullException(nameof(Principal)));
    }

    /// <summary>
    /// Serialized data for the client context object.
    /// </summary>
    public static readonly PropertyInfo<byte[]> ClientContextProperty = RegisterProperty<byte[]>(nameof(ClientContext));

    /// <summary>
    /// Serialized data for the client context object.
    /// </summary>
    /// <exception cref="ArgumentNullException">value is <see langword="null"/>.</exception>
    public byte[] ClientContext
    {
      get => GetProperty(ClientContextProperty)!;
      set => LoadProperty(ClientContextProperty, value ?? throw new ArgumentNullException(nameof(ClientContext)));
    }

    /// <summary>
    /// Serialized client culture.
    /// </summary>
    /// <value>The client culture.</value>
    public static readonly PropertyInfo<string> ClientCultureProperty = RegisterProperty<string>(nameof(ClientCulture));

    /// <summary>
    /// Serialized client culture.
    /// </summary>
    /// <value>The client culture.</value>
    /// <exception cref="ArgumentNullException">value is <see langword="null"/>.</exception>
    public string ClientCulture
    {
      get => GetProperty(ClientCultureProperty)!;
      set => LoadProperty(ClientCultureProperty, value ?? throw new ArgumentNullException(nameof(ClientCulture)));
    }

    /// <summary>
    /// Serialized client UI culture.
    /// </summary>
    /// <value>The client UI culture.</value>
    public static readonly PropertyInfo<string> ClientUICultureProperty = RegisterProperty<string>(nameof(ClientUICulture));

    /// <summary>
    /// Serialized client UI culture.
    /// </summary>
    /// <value>The client UI culture.</value>
    /// <exception cref="ArgumentNullException">value is <see langword="null"/>.</exception>
    public string ClientUICulture
    {
      get => GetProperty(ClientUICultureProperty)!;
      set => LoadProperty(ClientUICultureProperty, value ?? throw new ArgumentNullException(nameof(ClientUICulture)));
    }

    /// <summary>
    /// Initializes a new instance of <see cref="CriteriaRequest"/>-object.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="applicationContext"/>, <paramref name="principal"/>, <paramref name="clientContext"/>, <paramref name="clientCulture"/>, <paramref name="clientUICulture"/> or <paramref name="criteriaData"/> is <see langword="null"/>.</exception>
    public CriteriaRequest(ApplicationContext applicationContext, byte[] principal, byte[] clientContext, string clientCulture, string clientUICulture, byte[] criteriaData)
    {
      ApplicationContext = applicationContext;
      Principal = principal ?? throw new ArgumentNullException(nameof(principal));
      ClientContext = clientContext ?? throw new ArgumentNullException(nameof(clientContext));
      ClientCulture = clientCulture ?? throw new ArgumentNullException(nameof(clientCulture));
      ClientUICulture = clientUICulture ?? throw new ArgumentNullException(nameof(clientUICulture));
      TypeName = string.Empty;
      CriteriaData = criteriaData ?? throw new ArgumentNullException(nameof(criteriaData));
    }

    /// <summary>
    /// Initializes an empty instance for <see cref="MobileFormatter"/>.
    /// </summary>
    [Obsolete(MobileFormatter.DefaultCtorObsoleteMessage, error: true)]
    public CriteriaRequest()
    {
    }
  }
}