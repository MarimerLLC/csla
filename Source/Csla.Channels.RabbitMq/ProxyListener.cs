//-----------------------------------------------------------------------
// <copyright file="ProxyListener.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Handles replies from data portal server</summary>
//-----------------------------------------------------------------------

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
    protected IModel? Channel { get; set; }

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
          if (_instance == null)
          {
            _instance = new ProxyListener(queueUri);
          }
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
#if NET8_0_OR_GREATER
    [MemberNotNull(nameof(Connection), nameof(Channel), nameof(ReplyQueue))]
#endif
    private void InitializeRabbitMQ()
    {
      var factory = new ConnectionFactory { HostName = _queueUri.Host };
      if (_queueUri.Port < 0)
        factory.Port = _queueUri.Port;
      var userInfo = _queueUri.UserInfo.Split(':');
      if (userInfo.Length > 0 && !string.IsNullOrWhiteSpace(userInfo[0]))
        factory.UserName = userInfo[0];
      if (userInfo.Length > 1)
        factory.Password = userInfo[1];
      Connection = factory.CreateConnection();
      Channel = Connection.CreateModel();
      string[] query;
      if (string.IsNullOrWhiteSpace(_queueUri.Query))
        query = [];
      else
        query = _queueUri.Query.Substring(1).Split('&');
      if (query.Length == 0 || !query[0].StartsWith("reply="))
      {
        IsNamedReplyQueue = false;
        ReplyQueue = Channel.QueueDeclare();
      }
      else
      {
        IsNamedReplyQueue = true;
        var split = query[0].Split('=');
        ReplyQueue = Channel.QueueDeclare(
          queue: split[1],
          durable: false,
          exclusive: false,
          autoDelete: false,
          arguments: null);
      }
    }

    private volatile bool IsListening;
    private readonly object ListeningLock = new();

    public void StartListening()
    {
      if (IsListening) return;
      lock (ListeningLock)
      {
        if (IsListening) return;
        IsListening = true;
      }

      InitializeRabbitMQ();

      var consumer = new EventingBasicConsumer(Channel);
      consumer.Received += (_, ea) =>
      {
        Console.WriteLine($"Received reply for {ea.BasicProperties.CorrelationId}");
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
            ea.BasicProperties.Priority++;
            Channel.BasicPublish(
              exchange: "",
              routingKey: ReplyQueue?.QueueName,
              basicProperties: ea.BasicProperties,
              body: ea.Body);
          }
          else
          {
            Console.WriteLine($"## WARN Undeliverable reply for {ea.BasicProperties.CorrelationId} (discarded by {Environment.MachineName})");
          }
        }
      };
      Console.WriteLine($"Listening on queue {ReplyQueue?.QueueName}");
      Channel.BasicConsume(queue: ReplyQueue?.QueueName, autoAck: true, consumer: consumer);
    }

    public void Dispose()
    {
      Connection?.Close();
      Channel?.Dispose();
      Connection?.Dispose();
      IsListening = false;
    }
  }
}