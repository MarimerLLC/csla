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

    private ProxyListener()
    {
    }

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

    private bool IsNamedReplyQueue { get; set; }

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
      Channel = Connection.CreateModel();
      string[] query;
      if (string.IsNullOrWhiteSpace(QueueUri.Query))
        query = new string[] { };
      else
        query = QueueUri.Query.Substring(1).Split('&');
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
    private object ListeningLock = new object();

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
      consumer.Received += (model, ea) =>
      {
        Console.WriteLine($"Received reply for {ea.BasicProperties.CorrelationId}");
        if (Wip.WorkInProgress.TryRemove(ea.BasicProperties.CorrelationId, out WipItem item))
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
              routingKey: ReplyQueue.QueueName,
              basicProperties: ea.BasicProperties,
              body: ea.Body);
          }
          else
          {
            Console.WriteLine($"## WARN Undeliverable reply for {ea.BasicProperties.CorrelationId} (discarded by {Environment.MachineName})");
          }
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
      IsListening = false;
    }
  }
}