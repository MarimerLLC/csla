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
  public class RabbitMqProxy : IDataPortalProxy, IDisposable
  {
    /// <summary>
    /// Creates an instance of the object, initializing
    /// it to use the DefaultUrl value.
    /// </summary>
    public RabbitMqProxy()
    {
      this.DataPortalUrl = ApplicationContext.DataPortalUrlString;
    }

    /// <summary>
    /// Creates an instance of the object, initializing
    /// it to use the supplied URL.
    /// </summary>
    /// <param name="dataPortalUrl">RabbitMQ service URL</param>
    public RabbitMqProxy(string dataPortalUrl)
    {
      this.DataPortalUrl = dataPortalUrl;
    }

    /// <summary>
    /// Gets the URL address for the RabbitMQ
    /// service used by the data portal.
    /// </summary>
    public string DataPortalUrl { get; protected set; }

    /// <summary>
    /// Gets a value indicating whether the data portal
    /// is hosted on a remote server.
    /// </summary>
    public bool IsServerRemote => true;

    /// <summary>
    /// Gets or sets the timeout for network
    /// operations in seconds (default is 30 seconds).
    /// </summary>
    public int Timeout { get; set; } = 30;

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
    public async Task<DataPortalResult> Create(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      if (isSync)
        throw new NotSupportedException("isSync == true");

      InitializeRabbitMQ();
      DataPortalResult result;
      try
      {
        var request = GetBaseCriteriaRequest();
        request.TypeName = AssemblyNameTranslator.GetAssemblyQualifiedName(objectType.AssemblyQualifiedName);
        if (!(criteria is IMobileObject))
        {
          criteria = new PrimitiveCriteria(criteria);
        }
        request.CriteriaData = SerializationFormatterFactory.GetFormatter().Serialize(criteria);
        request = ConvertRequest(request);

        var serialized = SerializationFormatterFactory.GetFormatter().Serialize(request);

        serialized = await CallDataPortalServer(serialized, "create");

        var response = (Server.Hosts.HttpChannel.HttpResponse)SerializationFormatterFactory.GetFormatter().Deserialize(serialized);
        response = ConvertResponse(response);
        var globalContext = (ContextDictionary)SerializationFormatterFactory.GetFormatter().Deserialize(response.GlobalContext);
        if (response != null && response.ErrorData == null)
        {
          var obj = SerializationFormatterFactory.GetFormatter().Deserialize(response.ObjectData);
          result = new DataPortalResult(obj, null, globalContext);
        }
        else if (response != null && response.ErrorData != null)
        {
          var ex = new DataPortalException(response.ErrorData);
          result = new DataPortalResult(null, ex, globalContext);
        }
        else
        {
          throw new DataPortalException("null response", null);
        }
      }
      catch (Exception ex)
      {
        result = new DataPortalResult(null, ex, null);
      }
      if (result.Error != null)
        throw result.Error;
      return result;
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
    public async Task<DataPortalResult> Fetch(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      if (isSync)
        throw new NotSupportedException("isSync == true");

      InitializeRabbitMQ();
      DataPortalResult result;
      try
      {
        var request = GetBaseCriteriaRequest();
        request.TypeName = AssemblyNameTranslator.GetAssemblyQualifiedName(objectType.AssemblyQualifiedName);
        if (!(criteria is IMobileObject))
        {
          criteria = new PrimitiveCriteria(criteria);
        }
        request.CriteriaData = SerializationFormatterFactory.GetFormatter().Serialize(criteria);
        request = ConvertRequest(request);

        var serialized = SerializationFormatterFactory.GetFormatter().Serialize(request);

        serialized = await CallDataPortalServer(serialized, "fetch");

        var response = (Server.Hosts.HttpChannel.HttpResponse)SerializationFormatterFactory.GetFormatter().Deserialize(serialized);
        response = ConvertResponse(response);
        var globalContext = (ContextDictionary)SerializationFormatterFactory.GetFormatter().Deserialize(response.GlobalContext);
        if (response != null && response.ErrorData == null)
        {
          var obj = SerializationFormatterFactory.GetFormatter().Deserialize(response.ObjectData);
          result = new DataPortalResult(obj, null, globalContext);
        }
        else if (response != null && response.ErrorData != null)
        {
          var ex = new DataPortalException(response.ErrorData);
          result = new DataPortalResult(null, ex, globalContext);
        }
        else
        {
          throw new DataPortalException("null response", null);
        }
      }
      catch (Exception ex)
      {
        result = new DataPortalResult(null, ex, null);
      }
      if (result.Error != null)
        throw result.Error;
      return result;
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
    public async Task<DataPortalResult> Update(object obj, DataPortalContext context, bool isSync)
    {
      if (isSync)
        throw new NotSupportedException("isSync == true");

      InitializeRabbitMQ();
      DataPortalResult result;
      try
      {
        var request = GetBaseUpdateCriteriaRequest();
        request.ObjectData = SerializationFormatterFactory.GetFormatter().Serialize(obj);
        request = ConvertRequest(request);

        var serialized = SerializationFormatterFactory.GetFormatter().Serialize(request);

        serialized = await CallDataPortalServer(serialized, "update");

        var response = (Csla.Server.Hosts.HttpChannel.HttpResponse)SerializationFormatterFactory.GetFormatter().Deserialize(serialized);
        response = ConvertResponse(response);
        var globalContext = (ContextDictionary)SerializationFormatterFactory.GetFormatter().Deserialize(response.GlobalContext);
        if (response != null && response.ErrorData == null)
        {
          var newobj = SerializationFormatterFactory.GetFormatter().Deserialize(response.ObjectData);
          result = new DataPortalResult(newobj, null, globalContext);
        }
        else if (response != null && response.ErrorData != null)
        {
          var ex = new DataPortalException(response.ErrorData);
          result = new DataPortalResult(null, ex, globalContext);
        }
        else
        {
          throw new DataPortalException("null response", null);
        }
      }
      catch (Exception ex)
      {
        result = new DataPortalResult(null, ex, null);
      }
      if (result.Error != null)
        throw result.Error;
      return result;
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
    public async Task<DataPortalResult> Delete(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      if (isSync)
        throw new NotSupportedException("isSync == true");

      InitializeRabbitMQ();
      DataPortalResult result;
      try
      {
        var request = GetBaseCriteriaRequest();
        request.TypeName = AssemblyNameTranslator.GetAssemblyQualifiedName(objectType.AssemblyQualifiedName);
        if (!(criteria is IMobileObject))
        {
          criteria = new PrimitiveCriteria(criteria);
        }
        request.CriteriaData = SerializationFormatterFactory.GetFormatter().Serialize(criteria);
        request = ConvertRequest(request);

        var serialized = SerializationFormatterFactory.GetFormatter().Serialize(request);

        serialized = await CallDataPortalServer(serialized, "delete");

        var response = (Server.Hosts.HttpChannel.HttpResponse)SerializationFormatterFactory.GetFormatter().Deserialize(serialized);
        response = ConvertResponse(response);
        var globalContext = (ContextDictionary)SerializationFormatterFactory.GetFormatter().Deserialize(response.GlobalContext);
        if (response != null && response.ErrorData == null)
        {
          var obj = SerializationFormatterFactory.GetFormatter().Deserialize(response.ObjectData);
          result = new DataPortalResult(obj, null, globalContext);
        }
        else if (response != null && response.ErrorData != null)
        {
          var ex = new DataPortalException(response.ErrorData);
          result = new DataPortalResult(null, ex, globalContext);
        }
        else
        {
          throw new DataPortalException("null response", null);
        }
      }
      catch (Exception ex)
      {
        result = new DataPortalResult(null, ex, null);
      }
      if (result.Error != null)
        throw result.Error;
      return result;
    }

    private async Task<byte[]> CallDataPortalServer(byte[] serialized, string operation)
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

    #region Conversions

    /// <summary>
    /// Override this method to manipulate the message
    /// request data sent to the server.
    /// </summary>
    /// <param name="request">Update request data.</param>
    protected virtual Csla.Server.Hosts.HttpChannel.UpdateRequest ConvertRequest(Csla.Server.Hosts.HttpChannel.UpdateRequest request)
    {
      return request;
    }

    /// <summary>
    /// Override this method to manipulate the message
    /// request data sent to the server.
    /// </summary>
    /// <param name="request">Criteria request data.</param>
    protected virtual Csla.Server.Hosts.HttpChannel.CriteriaRequest ConvertRequest(Csla.Server.Hosts.HttpChannel.CriteriaRequest request)
    {
      return request;
    }

    /// <summary>
    /// Override this method to manipulate the message
    /// request data returned from the server.
    /// </summary>
    /// <param name="response">Response data.</param>
    protected virtual Csla.Server.Hosts.HttpChannel.HttpResponse ConvertResponse(Csla.Server.Hosts.HttpChannel.HttpResponse response)
    {
      return response;
    }

    #endregion

    #region Criteria

    private Csla.Server.Hosts.HttpChannel.CriteriaRequest GetBaseCriteriaRequest()
    {
      var request = new Csla.Server.Hosts.HttpChannel.CriteriaRequest
      {
        CriteriaData = null,
        ClientContext = SerializationFormatterFactory.GetFormatter().Serialize(ApplicationContext.ClientContext),
#pragma warning disable CS0618 // Type or member is obsolete
        GlobalContext = SerializationFormatterFactory.GetFormatter().Serialize(ApplicationContext.GlobalContext)
#pragma warning restore CS0618 // Type or member is obsolete
      };
      if (ApplicationContext.AuthenticationType == "Windows")
      {
        request.Principal = SerializationFormatterFactory.GetFormatter().Serialize(null);
      }
      else
      {
        request.Principal = SerializationFormatterFactory.GetFormatter().Serialize(ApplicationContext.User);
      }
      request.ClientCulture = System.Globalization.CultureInfo.CurrentCulture.Name;
      request.ClientUICulture = System.Globalization.CultureInfo.CurrentUICulture.Name;
      return request;
    }

    private Csla.Server.Hosts.HttpChannel.UpdateRequest GetBaseUpdateCriteriaRequest()
    {
      var request = new Csla.Server.Hosts.HttpChannel.UpdateRequest
      {
        ObjectData = null,
        ClientContext = SerializationFormatterFactory.GetFormatter().Serialize(ApplicationContext.ClientContext),
#pragma warning disable CS0618 // Type or member is obsolete
        GlobalContext = SerializationFormatterFactory.GetFormatter().Serialize(ApplicationContext.GlobalContext)
#pragma warning restore CS0618 // Type or member is obsolete
      };
      if (ApplicationContext.AuthenticationType == "Windows")
      {
        request.Principal = SerializationFormatterFactory.GetFormatter().Serialize(null);
      }
      else
      {
        request.Principal = SerializationFormatterFactory.GetFormatter().Serialize(ApplicationContext.User);
      }
      request.ClientCulture = Thread.CurrentThread.CurrentCulture.Name;
      request.ClientUICulture = Thread.CurrentThread.CurrentUICulture.Name;
      return request;
    }

    #endregion
  }
}
