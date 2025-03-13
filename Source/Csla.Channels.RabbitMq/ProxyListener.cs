//-----------------------------------------------------------------------
// <copyright file="ProxyListener.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Handles replies from data portal server</summary>
//-----------------------------------------------------------------------

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Csla.Channels.RabbitMq
{
  /// <summary>
  /// Handles replies from data portal server.
  /// </summary>
  internal class ProxyListener : IDisposable
  {
    /// <summary>
    /// Gets or sets the connection to the RabbitMQ service.
    /// </summary>
    protected IConnection? Connection { get; set; }

    /// <summary>
    /// Gets or sets the channel (model) for RabbitMQ.
    /// </summary>
    protected IChannel? Channel { get; set; }

    /// <summary>
    /// Gets or sets the queue for inbound messages
    /// and replies.
    /// </summary>
    public QueueDeclareOk? ReplyQueue { get; set; }

    private readonly Uri _queueUri;

    private static ProxyListener? _instance;

    private ProxyListener(Uri queueUri)
    {
      _queueUri = queueUri;
    }

    /// <summary>
    /// Gets an already existing listener or creates a new listener for the given URI.
    /// </summary>
    /// <param name="queueUri">The uri to listen for.</param>
    /// <returns>A new listener</returns>
    /// <exception cref="ArgumentNullException"><paramref name="queueUri"/> is <see langword="null"/>.</exception>
    public static ProxyListener GetListener(Uri queueUri)
    {
      if (queueUri is null)
        throw new ArgumentNullException(nameof(queueUri));

      if (_instance == null)
      {
        lock (typeof(ProxyListener))
        {
          _instance ??= new ProxyListener(queueUri);
        }
      }
      return _instance;
    }

    private bool IsNamedReplyQueue { get; set; }

    /// <summary>
    /// Method responsible for creating the Connection,
    /// Channel, ReplyQueue, and DataPortalQueueName values
    /// used for bi-directional communication.
    /// </summary>
    [MemberNotNull(nameof(Connection), nameof(Channel), nameof(ReplyQueue))]
    private async Task InitializeRabbitMQ()
    {
      var factory = new ConnectionFactory { HostName = _queueUri.Host };
      if (_queueUri.Port < 0)
        factory.Port = _queueUri.Port;
      var userInfo = _queueUri.UserInfo.Split(':');
      if (userInfo.Length > 0 && !string.IsNullOrWhiteSpace(userInfo[0]))
        factory.UserName = userInfo[0];
      if (userInfo.Length > 1)
        factory.Password = userInfo[1];
#pragma warning disable CS8774 // 11/22/2024, Nullable analysis can't track nullability with async/await
      Connection = await factory.CreateConnectionAsync();
      Channel = await Connection.CreateChannelAsync();
#pragma warning restore CS8774
      string[] query;
      if (string.IsNullOrWhiteSpace(_queueUri.Query))
        query = [];
      else
        query = _queueUri.Query[1..].Split('&');
      if (query.Length == 0 || !query[0].StartsWith("reply="))
      {
        IsNamedReplyQueue = false;
#pragma warning disable CS8774 // 11/22/2024, Nullable analysis can't track nullability with async/await
        ReplyQueue = await Channel.QueueDeclareAsync();
#pragma warning restore CS8774
      }
      else
      {
        IsNamedReplyQueue = true;
        var split = query[0].Split('=');
#pragma warning disable CS8774 // 11/22/2024, Nullable analysis can't track nullability with async/await
        ReplyQueue = await Channel.QueueDeclareAsync(queue: split[1], durable: false, exclusive: false, autoDelete: false, arguments: null);
#pragma warning restore CS8774
      }
    }

    private volatile bool IsListening;
    private readonly Lock ListeningLock = LockFactory.Create();

    public async Task StartListening()
    {
      if (IsListening) return;
      lock (ListeningLock)
      {
        if (IsListening) return;
        IsListening = true;
      }

      await InitializeRabbitMQ();
      Debug.Assert(Channel != null);

      var consumer = new AsyncEventingBasicConsumer(Channel);
      consumer.ReceivedAsync += async (_, ea) =>
      {
        if (ea.BasicProperties.CorrelationId is null)
        {
          throw new InvalidOperationException($"{nameof(BasicDeliverEventArgs.BasicProperties)}.{nameof(IReadOnlyBasicProperties.CorrelationId)} == null");
        }

        if (Wip.WorkInProgress.TryRemove(ea.BasicProperties.CorrelationId, out WipItem? item))
        {
          item.Response = ea.Body.ToArray();
          item.ResetEvent.Set();
        }
        else
        {
          // reply doesn't match any WIP item on this machine, but
          // if we're using a named reply queue there could be other
          // listeners; if so requeue the message up to 9 times
          if (IsNamedReplyQueue && ea.BasicProperties.Priority < 9)
          {
            var updatedBasicProperties = new BasicProperties(ea.BasicProperties);
            ++updatedBasicProperties.Priority;

            await Channel.BasicPublishAsync(exchange: "", routingKey: ReplyQueue!.QueueName, mandatory: true, basicProperties: updatedBasicProperties, body: ea.Body.ToArray());
          }
        }
      };
      await Channel.BasicConsumeAsync(queue: ReplyQueue!.QueueName, autoAck: true, consumer: consumer);
    }

    public void Dispose()
    {
      Channel?.Dispose();
      Connection?.Dispose();
      IsListening = false;
    }
  }
}