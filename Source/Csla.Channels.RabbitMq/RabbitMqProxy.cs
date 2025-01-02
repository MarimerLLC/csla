//-----------------------------------------------------------------------
// <copyright file="RabbitMqProxy.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implements a data portal proxy to relay data portal</summary>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Csla.DataPortalClient;
using Csla.Server;
using RabbitMQ.Client;

namespace Csla.Channels.RabbitMq
{
  /// <summary>
  /// Implements a data portal proxy to relay data portal
  /// calls to a remote application server by using RabbitMQ.
  /// </summary>
  public class RabbitMqProxy : DataPortalProxy
  {
    /// <summary>
    /// Creates an instance of the object, initializing
    /// it to use the supplied URL.
    /// </summary>
    /// <param name="applicationContext">The application context.</param>
    /// <param name="options">Proxy options</param>
    /// <exception cref="ArgumentNullException"><paramref name="applicationContext"/> or <paramref name="options"/> is <see langword="null"/>.</exception>
    public RabbitMqProxy(ApplicationContext applicationContext, RabbitMqProxyOptions options)
      : base(applicationContext)
    {
      if (options is null)
        throw new ArgumentNullException(nameof(options));

      DataPortalUrl = options.DataPortalUrl;
      Options = options;
    }

    /// <summary>
    /// Gets or sets the connection to the RabbitMQ service.
    /// </summary>
    protected IConnection? Connection { get; set; }

    /// <summary>
    /// Gets or sets the channel (model) for RabbitMQ.
    /// </summary>
    protected IChannel? Channel { get; set; }

    /// <summary>
    /// Gets or sets the name of the data portal
    /// service queue.
    /// </summary>
    protected string DataPortalQueueName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the queue listener that handles
    /// reply messages.
    /// </summary>
    private ProxyListener? QueueListener { get; set; }

    /// <inheritdoc />
    public override string DataPortalUrl { get; }

    /// <summary>
    /// Gets the proxy options.
    /// </summary>
    private RabbitMqProxyOptions Options { get; }

    /// <summary>
    /// Method responsible for creating the Connection,
    /// Channel, ReplyQueue, and DataPortalQueueName values
    /// used for bi-directional communication.
    /// </summary>
#if NET8_0_OR_GREATER
    [MemberNotNull(nameof(Connection), nameof(Channel), nameof(QueueListener))]
#endif
    protected virtual async Task InitializeRabbitMQ()
    {
      if (Connection == null || Channel == null || QueueListener == null)
      {
        var url = new Uri(DataPortalUrl);
        if (url.Scheme != "rabbitmq")
          throw new UriFormatException("Scheme != rabbitmq://");
        if (string.IsNullOrWhiteSpace(url.Host))
          throw new UriFormatException("Host");
        DataPortalQueueName = url.AbsolutePath.Substring(1);
        if (string.IsNullOrWhiteSpace(DataPortalQueueName))
          throw new UriFormatException("DataPortalQueueName");
        var factory = new ConnectionFactory { HostName = url.Host };
        if (url.Port < 0)
          factory.Port = url.Port;
        var userInfo = url.UserInfo.Split(':');
        if (userInfo.Length > 0 && !string.IsNullOrWhiteSpace(userInfo[0]))
          factory.UserName = userInfo[0];
        if (userInfo.Length > 1)
          factory.Password = userInfo[1];
#pragma warning disable CS8774 // 11/22/2024, Nullable analysis can't track nullability with async/await
        Connection = await factory.CreateConnectionAsync();
        Channel = await Connection.CreateChannelAsync();
#pragma warning restore CS8774
        if (QueueListener == null)
        {
          QueueListener = ProxyListener.GetListener(url);
          await QueueListener.StartListening();
        }
      }
    }

    private void DisposeRabbitMq()
    {
      QueueListener?.Dispose();
      Channel?.Dispose();
      Connection?.Dispose();
      Channel = null;
      Connection = null;
    }

    /// <inheritdoc />
    public override async Task<DataPortalResult> Create(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      if (objectType is null)
        throw new ArgumentNullException(nameof(objectType));
      if (criteria is null)
        throw new ArgumentNullException(nameof(criteria));
      if (context is null)
        throw new ArgumentNullException(nameof(context));
      if (isSync)
        throw new NotSupportedException("isSync == true");

      try
      {
        await InitializeRabbitMQ();
        return await base.Create(objectType, criteria, context, isSync);
      }
      finally
      {
        DisposeRabbitMq();
      }
    }

    /// <inheritdoc />
    public override async Task<DataPortalResult> Fetch(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      if (objectType is null)
        throw new ArgumentNullException(nameof(objectType));
      if (criteria is null)
        throw new ArgumentNullException(nameof(criteria));
      if (context is null)
        throw new ArgumentNullException(nameof(context));
      if (isSync)
        throw new NotSupportedException("isSync == true");

      try
      {
        await InitializeRabbitMQ();
        return await base.Fetch(objectType, criteria, context, isSync);
      }
      finally
      {
        DisposeRabbitMq();
      }
    }

    public override async Task<DataPortalResult> Update(object obj, DataPortalContext context, bool isSync)
    {
      if (obj is null)
        throw new ArgumentNullException(nameof(obj));
      if (context is null)
        throw new ArgumentNullException(nameof(context));
      if (isSync)
        throw new NotSupportedException("isSync == true");

      try
      {
        await InitializeRabbitMQ();
        return await base.Update(obj, context, isSync);
      }
      finally
      {
        DisposeRabbitMq();
      }
    }

    /// <inheritdoc />
    public override async Task<DataPortalResult> Delete(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      if (objectType is null)
        throw new ArgumentNullException(nameof(objectType));
      if (criteria is null)
        throw new ArgumentNullException(nameof(criteria));
      if (context is null)
        throw new ArgumentNullException(nameof(context));
      if (isSync)
        throw new NotSupportedException("isSync == true");

      try
      {
        await InitializeRabbitMQ();
        return await base.Delete(objectType, criteria, context, isSync);
      }
      finally
      {
        DisposeRabbitMq();
      }
    }

    /// <summary>
    /// Create message and send to Rabbit MQ server.
    /// Return Response from server
    /// </summary>
    /// <param name="serialized">Serialized request</param>
    /// <param name="operation">DataPortal operation</param>
    /// <param name="routingToken">Routing Tag for server</param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    /// <returns>Serialized response from server</returns>
    protected override async Task<byte[]> CallDataPortalServer(byte[] serialized, string operation, string? routingToken, bool isSync)
    {
      var correlationId = Guid.NewGuid().ToString();
      var resetEvent = new Threading.AsyncManualResetEvent();
      var wip = Wip.WorkInProgress.GetOrAdd(correlationId, new WipItem(resetEvent));

      await SendMessage(QueueListener!.ReplyQueue!.QueueName, correlationId, operation, serialized);

      var timeout = Task.Delay(Options.Timeout);
      if (await Task.WhenAny(wip.ResetEvent.WaitAsync(), timeout) == timeout)
        throw new TimeoutException();

      return wip.Response!;
    }

    private async Task SendMessage(string sender, string correlationId, string operation, byte[] request)
    {
      var props = new BasicProperties
      {
        CorrelationId = correlationId,
        Type = operation
      };
      if (!string.IsNullOrWhiteSpace(sender))
        props.ReplyTo = sender;

      await Channel!.BasicPublishAsync(
        exchange: "",
        routingKey: DataPortalQueueName,
        mandatory: true,
        basicProperties: props,
        body: request);
    }
  }
}