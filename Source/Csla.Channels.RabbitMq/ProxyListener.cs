//-----------------------------------------------------------------------
// <copyright file="ProxyListener.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Handles replies from data portal server</summary>
//-----------------------------------------------------------------------
using System;
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
    protected IConnection Connection { get; set; }
    /// <summary>
    /// Gets or sets the channel (model) for RabbitMQ.
    /// </summary>
    protected IModel Channel { get; set; }
    /// <summary>
    /// Gets or sets the queue for inbound messages
    /// and replies.
    /// </summary>
    public QueueDeclareOk ReplyQueue { get; set; }

    private Uri QueueUri;

    private static ProxyListener _instance;

    private ProxyListener() { }

    public static ProxyListener GetListener(Uri queueUri)
    {
      if (_instance == null)
      {
        lock (typeof(ProxyListener))
        {
          if (_instance == null)
          {
            _instance = new ProxyListener { QueueUri = queueUri };
          }
        }
      }
      return _instance;
    }

    /// <summary>
    /// Method responsible for creating the Connection,
    /// Channel, ReplyQueue, and DataPortalQueueName values
    /// used for bi-directional communication.
    /// </summary>
    private void InitializeRabbitMQ()
    {
      var factory = new ConnectionFactory() { HostName = QueueUri.Host };
      if (QueueUri.Port < 0)
        factory.Port = QueueUri.Port;
      var userInfo = QueueUri.UserInfo.Split(':');
      if (userInfo.Length > 0 && !string.IsNullOrWhiteSpace(userInfo[0]))
        factory.UserName = userInfo[0];
      if (userInfo.Length > 1)
        factory.Password = userInfo[1];
      Connection = factory.CreateConnection();
      Connection.ConnectionShutdown += (s, e) =>
      {
        Connection?.Dispose();
        Connection = null;
        Channel = null;
        ReplyQueue = null;
      };
      Channel = Connection.CreateModel();
      Channel.ModelShutdown += (s, e) =>
      {
        Channel?.Dispose();
        Channel = null;
        ReplyQueue = null;
      };
      string[] query;
      if (string.IsNullOrWhiteSpace(QueueUri.Query))
        query = new string[] { };
      else
        query = QueueUri.Query.Substring(1).Split('&');
      if (query.Length == 0 || !query[0].StartsWith("reply="))
      {
        ReplyQueue = Channel.QueueDeclare();
      }
      else
      {
        var split = query[0].Split('=');
        ReplyQueue = Channel.QueueDeclare(
          queue: split[1],
          durable: false,
          exclusive: false,
          autoDelete: false,
          arguments: null);
      }
    }

    public void StartListening()
    {
      InitializeRabbitMQ();

      var consumer = new EventingBasicConsumer(Channel);
      consumer.Received += (model, ea) =>
      {
        Console.WriteLine($"Received reply for {ea.BasicProperties.CorrelationId}");
        var correlationId = ea.BasicProperties.CorrelationId;
        if (Wip.WorkInProgress.TryRemove(correlationId, out WipItem item))
        {
          item.Response = ea.Body;
          item.ResetEvent.Set();
        }
      };
      Console.WriteLine($"Listening on queue {ReplyQueue.QueueName}");
      Channel.BasicConsume(queue: ReplyQueue.QueueName, autoAck: true, consumer: consumer);
    }

    public void Dispose()
    {
      Connection?.Close();
      Channel?.Dispose();
      Connection?.Dispose();
    }
  }
}
