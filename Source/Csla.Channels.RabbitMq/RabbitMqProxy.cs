//-----------------------------------------------------------------------
// <copyright file="RabbitMqProxy.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implements a data portal proxy to relay data portal</summary>
//-----------------------------------------------------------------------
using System;
using System.Threading;
using System.Threading.Tasks;
using Csla.Core;
using Csla.DataPortalClient;
using Csla.Serialization;
using Csla.Serialization.Mobile;
using Csla.Server;
using RabbitMQ.Client;

namespace Csla.Channels.RabbitMq
{
  /// <summary>
  /// Implements a data portal proxy to relay data portal
  /// calls to a remote application server by using RabbitMQ.
  /// </summary>
  public class RabbitMqProxy : DataPortalProxy, IDisposable
  {
    /// <summary>
    /// Creates an instance of the object, initializing
    /// it to use the DefaultUrl value.
    /// </summary>
    public RabbitMqProxy()
    {
    }

    /// <summary>
    /// Creates an instance of the object, initializing
    /// it to use the supplied URL.
    /// </summary>
    /// <param name="dataPortalUrl">RabbitMQ service URL</param>
    public RabbitMqProxy(string dataPortalUrl)
    {
      DataPortalUrl = dataPortalUrl;
    }

    /// <summary>
    /// Gets or sets the timeout for network
    /// operations in seconds (default is 30 seconds).
    /// </summary>
    public override int Timeout { get; set; } = 30;

    /// <summary>
    /// Gets or sets the connection to the RabbitMQ service.
    /// </summary>
    protected IConnection Connection { get; set; }

    /// <summary>
    /// Gets or sets the channel (model) for RabbitMQ.
    /// </summary>
    protected IModel Channel { get; set; }

    /// <summary>
    /// Gets or sets the name of the data portal
    /// service queue.
    /// </summary>
    protected string DataPortalQueueName { get; set; }

    /// <summary>
    /// Gets or sets the queue listener that handles
    /// reply messages.
    /// </summary>
    private ProxyListener QueueListener { get; set; }

    /// <summary>
    /// Method responsible for creating the Connection,
    /// Channel, ReplyQueue, and DataPortalQueueName values
    /// used for bi-directional communication.
    /// </summary>
    protected virtual void InitializeRabbitMQ()
    {
      if (Connection == null)
      {
        var url = new Uri(DataPortalUrl);
        Console.WriteLine($"Initializing {DataPortalUrl}");
        if (url.Scheme != "rabbitmq")
          throw new UriFormatException("Scheme != rabbitmq://");
        if (string.IsNullOrWhiteSpace(url.Host))
          throw new UriFormatException("Host");
        DataPortalQueueName = url.AbsolutePath.Substring(1);
        if (string.IsNullOrWhiteSpace(DataPortalQueueName))
          throw new UriFormatException("DataPortalQueueName");
        Console.WriteLine($"Will send to queue {DataPortalQueueName}");
        var factory = new ConnectionFactory() { HostName = url.Host };
        if (url.Port < 0)
          factory.Port = url.Port;
        var userInfo = url.UserInfo.Split(':');
        if (userInfo.Length > 0 && !string.IsNullOrWhiteSpace(userInfo[0]))
          factory.UserName = userInfo[0];
        if (userInfo.Length > 1)
          factory.Password = userInfo[1];
        Connection = factory.CreateConnection();
        Channel = Connection.CreateModel();
        if (QueueListener == null)
        {
          QueueListener = ProxyListener.GetListener(url);
          QueueListener.StartListening();
        }
      }
    }

    /// <summary>
    /// Called by <see cref="DataPortal" /> to create a
    /// new business object.
    /// </summary>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">DataPortalContext object passed to the server.</param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    public override async Task<DataPortalResult> Create(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      if (isSync)
        throw new NotSupportedException("isSync == true");

      InitializeRabbitMQ();
      return await base.Create(objectType, criteria, context, isSync);
    }

    /// <summary>
    /// Called by <see cref="DataPortal" /> to load an
    /// existing business object.
    /// </summary>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    public override async Task<DataPortalResult> Fetch(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      if (isSync)
        throw new NotSupportedException("isSync == true");

      InitializeRabbitMQ();
      return await base.Fetch(objectType, criteria, context, isSync);
    }

    /// <summary>
    /// Called by <see cref="DataPortal" /> to update a
    /// business object.
    /// </summary>
    /// <param name="obj">The business object to update.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    public override async Task<DataPortalResult> Update(object obj, DataPortalContext context, bool isSync)
    {
      if (isSync)
        throw new NotSupportedException("isSync == true");

      InitializeRabbitMQ();
      return await base.Update(obj, context, isSync);
    }

    /// <summary>
    /// Called by <see cref="DataPortal" /> to delete a
    /// business object.
    /// </summary>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    public override async Task<DataPortalResult> Delete(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      if (isSync)
        throw new NotSupportedException("isSync == true");

      InitializeRabbitMQ();
      return await base.Delete(objectType, criteria, context, isSync);
    }

    /// <summary>
    /// Create message and send to Rabbit MQ server.
    /// Return Response from server
    /// </summary>
    /// <param name="serialized">Serialised request</param>
    /// <param name="operation">DataPortal operation</param>
    /// <param name="routingToken">Routing Tag for server</param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    /// <returns>Serialised response from server</returns>
    protected override async Task<byte[]> CallDataPortalServer(byte[] serialized, string operation, string routingToken, bool isSync)
    {
      var correlationId = Guid.NewGuid().ToString();
      var resetEvent = new Csla.Threading.AsyncManualResetEvent();
      var wip = Wip.WorkInProgress.GetOrAdd(correlationId, new WipItem { ResetEvent = resetEvent });

      SendMessage(QueueListener.ReplyQueue.QueueName, correlationId, operation, serialized);

      var timeout = Task.Delay(Timeout * 1000);
      if (await Task.WhenAny(wip.ResetEvent.WaitAsync(), timeout) == timeout)
        throw new TimeoutException();

      return wip.Response;
    }

    private void SendMessage(string sender, string correlationId, string operation, byte[] request)
    {
      var props = Channel.CreateBasicProperties();
      if (!string.IsNullOrWhiteSpace(sender))
        props.ReplyTo = sender;
      props.CorrelationId = correlationId;
      props.Type = operation;
      Channel.BasicPublish(
        exchange: "",
        routingKey: DataPortalQueueName,
        basicProperties: props,
        body: request);
    }

    /// <summary>
    /// Dispose this object and its resources.
    /// </summary>
    public void Dispose()
    {
      QueueListener?.Dispose();
      Connection?.Close();
      Channel?.Dispose();
      Connection?.Dispose();
      Channel = null;
      Connection = null;
    }
  }
}