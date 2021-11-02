//-----------------------------------------------------------------------
// <copyright file="RabbitMqPortal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Exposes server-side DataPortal functionality through RabbitMQ</summary>
//-----------------------------------------------------------------------
using System;
using System.Security.Principal;
using System.Threading.Tasks;
using Csla.Core;
using Csla.Serialization;
using Csla.Serialization.Mobile;
using Csla.Server;
using Csla.Server.Hosts.DataPortalChannel;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Csla.Channels.RabbitMq
{
  /// <summary>
  /// Exposes server-side DataPortal functionality through RabbitMQ
  /// </summary>
  public class RabbitMqPortal : IDisposable
  {
    private IDataPortalServer dataPortalServer;
    private ApplicationContext ApplicationContext { get; set; }

    /// <summary>
    /// Creates an instance of the type
    /// </summary>
    /// <param name="applicationContext"></param>
    /// <param name="dataPortal">Data portal server service</param>
    public RabbitMqPortal(ApplicationContext applicationContext, IDataPortalServer dataPortal)
    {
      dataPortalServer = dataPortal;
      ApplicationContext = applicationContext;
    }

    /// <summary>
    /// Gets the URI for the data portal service.
    /// </summary>
    public string DataPortalUrl { get; private set; }

    private IConnection Connection;
    private IModel Channel;
    private string DataPortalQueueName;

    /// <summary>
    /// Gets or sets the timeout for network
    /// operations in seconds (default is 30 seconds).
    /// </summary>
    public int Timeout { get; set; } = 30;

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    public RabbitMqPortal()
    {
      DataPortalUrl = ApplicationContext.DataPortalUrlString;
    }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="dataPortalUrl">URI for the data portal</param>
    public RabbitMqPortal(string dataPortalUrl)
    {
      DataPortalUrl = dataPortalUrl;
    }

    private Uri DataPortalUri { get; set; }

    private void InitializeRabbitMQ()
    {
      if (Connection == null)
      {
        Console.WriteLine($"Initializing connection to {DataPortalUrl}");
        DataPortalUri = new Uri(DataPortalUrl);
        var url = DataPortalUri;
        if (url.Scheme != "rabbitmq")
          throw new UriFormatException("Scheme != rabbitmq://");
        if (string.IsNullOrWhiteSpace(url.Host))
          throw new UriFormatException("Host");
        DataPortalQueueName = url.AbsolutePath.Substring(1);
        if (string.IsNullOrWhiteSpace(DataPortalQueueName))
          throw new UriFormatException("DataPortalQueueName");

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
      }
    }

    /// <summary>
    /// Start processing inbound messages.
    /// </summary>
    public void StartListening()
    {
      InitializeRabbitMQ();
      Channel.QueueDeclare(
        queue: DataPortalQueueName,
        durable: false,
        exclusive: false,
        autoDelete: false,
        arguments: null);

      var consumer = new EventingBasicConsumer(Channel);
      consumer.Received += (model, ea) =>
      {
        Console.WriteLine($"Received {ea.BasicProperties.Type} for {ea.BasicProperties.CorrelationId} from {ea.BasicProperties.ReplyTo}");
        InvokePortal(ea, ea.Body.ToArray());
      };
      Console.WriteLine($"Listening on queue {DataPortalQueueName}");
      Channel.BasicConsume(queue: DataPortalQueueName, autoAck: true, consumer: consumer);
    }

    private async void InvokePortal(BasicDeliverEventArgs ea, byte[] requestData)
    {
      var result = ApplicationContext.CreateInstanceDI<DataPortalResponse>();
      try
      {
        var request = SerializationFormatterFactory.GetFormatter(ApplicationContext).Deserialize(requestData);
        result = await CallPortal(ea.BasicProperties.Type, request);
      }
      catch (Exception ex)
      {
        result.ErrorData = ApplicationContext.CreateInstanceDI<DataPortalErrorInfo>(ex);
      }

      try
      {
        var response = SerializationFormatterFactory.GetFormatter(ApplicationContext).Serialize(result);
        SendMessage(ea.BasicProperties.ReplyTo, ea.BasicProperties.CorrelationId, response);
      }
      catch (Exception ex)
      {
        try
        {
          result = ApplicationContext.CreateInstanceDI<DataPortalResponse>();
          result.ErrorData = ApplicationContext.CreateInstanceDI<DataPortalErrorInfo>(ex);
          var response = SerializationFormatterFactory.GetFormatter(ApplicationContext).Serialize(result);
          SendMessage(ea.BasicProperties.ReplyTo, ea.BasicProperties.CorrelationId, response);
        }
        catch (Exception ex1)
        {
          Console.Error.WriteLine($"## ERROR {ex1.Message}");
        }
      }
    }

    private void SendMessage(string target, string correlationId, byte[] request)
    {
      InitializeRabbitMQ();
      var props = Channel.CreateBasicProperties();
      props.CorrelationId = correlationId;
      Channel.BasicPublish(
        exchange: "",
        routingKey: target,
        basicProperties: props,
        body: request);
    }

    private async Task<DataPortalResponse> CallPortal(string operation, object request)
    {
      DataPortalResponse result;
      switch (operation)
      {
        case "create":
          result = await Create((CriteriaRequest)request).ConfigureAwait(false);
          break;

        case "fetch":
          result = await Fetch((CriteriaRequest)request).ConfigureAwait(false);
          break;

        case "update":
          result = await Update((UpdateRequest)request).ConfigureAwait(false);
          break;

        case "delete":
          result = await Delete((CriteriaRequest)request).ConfigureAwait(false);
          break;

        default:
          throw new InvalidOperationException(operation);
      }
      return result;
    }

    /// <summary>
    /// Create and initialize an existing business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    public async Task<DataPortalResponse> Create(CriteriaRequest request)
    {
      var result = ApplicationContext.CreateInstanceDI<DataPortalResponse>();
      try
      {
        request = ConvertRequest(request);

        // unpack criteria data into object
        object criteria = GetCriteria(ApplicationContext, request.CriteriaData);
        if (criteria is Csla.DataPortalClient.PrimitiveCriteria)
        {
          criteria = ((Csla.DataPortalClient.PrimitiveCriteria)criteria).Value;
        }

        var objectType = Csla.Reflection.MethodCaller.GetType(AssemblyNameTranslator.GetAssemblyQualifiedName(request.TypeName), true);
        var context = new DataPortalContext(
          (IPrincipal)SerializationFormatterFactory.GetFormatter(ApplicationContext).Deserialize(request.Principal),
          true,
          request.ClientCulture,
          request.ClientUICulture,
          (ContextDictionary)SerializationFormatterFactory.GetFormatter(ApplicationContext).Deserialize(request.ClientContext));

        var dpr = await dataPortalServer.Create(objectType, criteria, context, true);

        if (dpr.Error != null)
          result.ErrorData = ApplicationContext.CreateInstanceDI<DataPortalErrorInfo>(dpr.Error);
        result.ObjectData = SerializationFormatterFactory.GetFormatter(ApplicationContext).Serialize(dpr.ReturnObject);
      }
      catch (Exception ex)
      {
        result.ErrorData = ApplicationContext.CreateInstanceDI<DataPortalErrorInfo>(ex);
        throw;
      }
      finally
      {
        result = ConvertResponse(result);
      }
      return result;
    }

    /// <summary>
    /// Get an existing business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    public async Task<DataPortalResponse> Fetch(CriteriaRequest request)
    {
      var result = ApplicationContext.CreateInstanceDI<DataPortalResponse>();
      try
      {
        request = ConvertRequest(request);

        // unpack criteria data into object
        object criteria = GetCriteria(ApplicationContext, request.CriteriaData);
        if (criteria is Csla.DataPortalClient.PrimitiveCriteria)
        {
          criteria = ((Csla.DataPortalClient.PrimitiveCriteria)criteria).Value;
        }

        var objectType = Csla.Reflection.MethodCaller.GetType(AssemblyNameTranslator.GetAssemblyQualifiedName(request.TypeName), true);
        var context = new DataPortalContext(
          (IPrincipal)SerializationFormatterFactory.GetFormatter(ApplicationContext).Deserialize(request.Principal),
          true,
          request.ClientCulture,
          request.ClientUICulture,
          (ContextDictionary)SerializationFormatterFactory.GetFormatter(ApplicationContext).Deserialize(request.ClientContext));

        var dpr = await dataPortalServer.Fetch(objectType, criteria, context, true);

        if (dpr.Error != null)
          result.ErrorData = ApplicationContext.CreateInstanceDI<DataPortalErrorInfo>(dpr.Error);
        result.ObjectData = SerializationFormatterFactory.GetFormatter(ApplicationContext).Serialize(dpr.ReturnObject);
      }
      catch (Exception ex)
      {
        result.ErrorData = ApplicationContext.CreateInstanceDI<DataPortalErrorInfo>(ex);
        throw;
      }
      finally
      {
        result = ConvertResponse(result);
      }
      return result;
    }

    /// <summary>
    /// Update a business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    public async Task<DataPortalResponse> Update(UpdateRequest request)
    {
      var result = ApplicationContext.CreateInstanceDI<DataPortalResponse>();
      try
      {
        request = ConvertRequest(request);
        // unpack object
        object obj = GetCriteria(ApplicationContext, request.ObjectData);

        var context = new DataPortalContext(
          (IPrincipal)SerializationFormatterFactory.GetFormatter(ApplicationContext).Deserialize(request.Principal),
          true,
          request.ClientCulture,
          request.ClientUICulture,
          (ContextDictionary)SerializationFormatterFactory.GetFormatter(ApplicationContext).Deserialize(request.ClientContext));

        var dpr = await dataPortalServer.Update(obj, context, true);

        if (dpr.Error != null)
          result.ErrorData = ApplicationContext.CreateInstanceDI<DataPortalErrorInfo>(dpr.Error);

        result.ObjectData = SerializationFormatterFactory.GetFormatter(ApplicationContext).Serialize(dpr.ReturnObject);
      }
      catch (Exception ex)
      {
        result.ErrorData = ApplicationContext.CreateInstanceDI<DataPortalErrorInfo>(ex);
        throw;
      }
      finally
      {
        result = ConvertResponse(result);
      }
      return result;
    }

    /// <summary>
    /// Delete a business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    public async Task<DataPortalResponse> Delete(CriteriaRequest request)
    {
      var result = ApplicationContext.CreateInstanceDI<DataPortalResponse>();
      try
      {
        request = ConvertRequest(request);

        // unpack criteria data into object
        object criteria = GetCriteria(ApplicationContext, request.CriteriaData);
        if (criteria is Csla.DataPortalClient.PrimitiveCriteria)
        {
          criteria = ((Csla.DataPortalClient.PrimitiveCriteria)criteria).Value;
        }

        var objectType = Csla.Reflection.MethodCaller.GetType(AssemblyNameTranslator.GetAssemblyQualifiedName(request.TypeName), true);
        var context = new DataPortalContext(
          (IPrincipal)SerializationFormatterFactory.GetFormatter(ApplicationContext).Deserialize(request.Principal),
          true,
          request.ClientCulture,
          request.ClientUICulture,
          (ContextDictionary)SerializationFormatterFactory.GetFormatter(ApplicationContext).Deserialize(request.ClientContext));

        var dpr = await dataPortalServer.Delete(objectType, criteria, context, true);

        if (dpr.Error != null)
          result.ErrorData = ApplicationContext.CreateInstanceDI<DataPortalErrorInfo>(dpr.Error);
        result.ObjectData = SerializationFormatterFactory.GetFormatter(ApplicationContext).Serialize(dpr.ReturnObject);
      }
      catch (Exception ex)
      {
        result.ErrorData = ApplicationContext.CreateInstanceDI<DataPortalErrorInfo>(ex);
        throw;
      }
      finally
      {
        result = ConvertResponse(result);
      }
      return result;
    }

    #region Criteria

    private static object GetCriteria(ApplicationContext applicationContext, byte[] criteriaData)
    {
      object criteria = null;
      if (criteriaData != null)
        criteria = SerializationFormatterFactory.GetFormatter(applicationContext).Deserialize(criteriaData);
      return criteria;
    }

    #endregion Criteria

    #region Conversion methods

    /// <summary>
    /// Override to convert the request data before it
    /// is transferred over the network.
    /// </summary>
    /// <param name="request">Request object.</param>
    protected virtual UpdateRequest ConvertRequest(UpdateRequest request)
    {
      return request;
    }

    /// <summary>
    /// Override to convert the request data before it
    /// is transferred over the network.
    /// </summary>
    /// <param name="request">Request object.</param>
    protected virtual CriteriaRequest ConvertRequest(CriteriaRequest request)
    {
      return request;
    }

    /// <summary>
    /// Override to convert the response data after it
    /// comes back from the network.
    /// </summary>
    /// <param name="response">Response object.</param>
    protected virtual DataPortalResponse ConvertResponse(DataPortalResponse response)
    {
      return response;
    }

    #endregion Conversion methods

    /// <summary>
    /// Dispose this object.
    /// </summary>
    public void Dispose()
    {
      Connection?.Close();
      Channel?.Dispose();
      Connection?.Dispose();
    }
  }
}