//-----------------------------------------------------------------------
// <copyright file="RabbitMqPortal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Exposes server-side DataPortal functionality through RabbitMQ</summary>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Security.Principal;
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
    internal RabbitMqPortal(ApplicationContext applicationContext, IDataPortalServer dataPortal, RabbitMqPortalOptions rabbitMqPortalOptions)
    {
      _dataPortalServer = dataPortal ?? throw new ArgumentNullException(nameof(dataPortal));
      _applicationContext = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
      DataPortalUri = rabbitMqPortalOptions.DataPortalUri;
    }

    private readonly IDataPortalServer _dataPortalServer;
    private readonly ApplicationContext _applicationContext;

    private IConnection? Connection;
    private IModel? Channel;
    private string? DataPortalQueueName;

    private Uri DataPortalUri { get; set; }

#if NET8_0_OR_GREATER
    [MemberNotNull(nameof(DataPortalUri), nameof(DataPortalQueueName), nameof(Connection), nameof(Channel))]
#endif
    private void InitializeRabbitMQ()
    {
      if (Connection == null || DataPortalUri == null || Channel == null || DataPortalQueueName == null)
      {
        if (DataPortalUri == null)
          throw new InvalidOperationException($"{nameof(DataPortalUri)} == null");
        DataPortalQueueName = DataPortalUri.AbsolutePath[1..];

        var factory = new ConnectionFactory { HostName = DataPortalUri.Host };
        if (DataPortalUri.Port < 0)
          factory.Port = DataPortalUri.Port;
        var userInfo = DataPortalUri.UserInfo.Split(':');
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
      Channel?.QueueDeclare(
        queue: DataPortalQueueName,
        durable: false,
        exclusive: false,
        autoDelete: false,
        arguments: null);

      var consumer = new EventingBasicConsumer(Channel);
      consumer.Received += (_, ea) =>
      {
        InvokePortal(ea, ea.Body.ToArray());
      };
      Channel.BasicConsume(queue: DataPortalQueueName, autoAck: true, consumer: consumer);
    }

    private async void InvokePortal(BasicDeliverEventArgs ea, byte[] requestData)
    {
      var result = _applicationContext.CreateInstanceDI<DataPortalResponse>();
      try
      {
        var request = _applicationContext.GetRequiredService<ISerializationFormatter>().Deserialize(requestData);
        result = await CallPortal(ea.BasicProperties.Type, request);
      }
      catch (Exception ex)
      {
        result.ErrorData = _applicationContext.CreateInstanceDI<DataPortalErrorInfo>(ex);
      }

      try
      {
        var response = _applicationContext.GetRequiredService<ISerializationFormatter>().Serialize(result);
        SendMessage(ea.BasicProperties.ReplyTo, ea.BasicProperties.CorrelationId, response);
      }
      catch (Exception ex)
      {
        result = _applicationContext.CreateInstanceDI<DataPortalResponse>();
        result.ErrorData = _applicationContext.CreateInstanceDI<DataPortalErrorInfo>(ex);
        var response = _applicationContext.GetRequiredService<ISerializationFormatter>().Serialize(result);
        SendMessage(ea.BasicProperties.ReplyTo, ea.BasicProperties.CorrelationId, response);
      }
    }

    private void SendMessage(string target, string correlationId, byte[] request)
    {
      InitializeRabbitMQ();
      if (Channel is null)
        throw new InvalidOperationException($"{nameof(Channel)} == null");
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
      return operation switch
      {
        "create" => await Create((CriteriaRequest)request).ConfigureAwait(false),
        "fetch" => await Fetch((CriteriaRequest)request).ConfigureAwait(false),
        "update" => await Update((UpdateRequest)request).ConfigureAwait(false),
        "delete" => await Delete((CriteriaRequest)request).ConfigureAwait(false),
        _ => throw new InvalidOperationException(operation),
      };
    }

    /// <summary>
    /// Create and initialize an existing business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    private async Task<DataPortalResponse> Create(CriteriaRequest request)
    {
      var result = _applicationContext.CreateInstanceDI<DataPortalResponse>();
      try
      {
        request = ConvertRequest(request);

        // unpack criteria data into object
        object? criteria = GetCriteria(_applicationContext, request.CriteriaData);
        if (criteria is DataPortalClient.PrimitiveCriteria primitiveCriteria)
        {
          criteria = primitiveCriteria.Value;
        }

        var objectType = Reflection.MethodCaller.GetType(AssemblyNameTranslator.GetAssemblyQualifiedName(request.TypeName), true);
        var context = new DataPortalContext(
          _applicationContext, (IPrincipal)_applicationContext.GetRequiredService<ISerializationFormatter>().Deserialize(request.Principal),
          true,
          request.ClientCulture,
          request.ClientUICulture,
          (IContextDictionary)_applicationContext.GetRequiredService<ISerializationFormatter>().Deserialize(request.ClientContext));

        var dpr = await _dataPortalServer.Create(objectType, criteria, context, true);

        if (dpr.Error != null)
          result.ErrorData = _applicationContext.CreateInstanceDI<DataPortalErrorInfo>(dpr.Error);
        result.ObjectData = _applicationContext.GetRequiredService<ISerializationFormatter>().Serialize(dpr.ReturnObject);
      }
      catch (Exception ex)
      {
        result.ErrorData = _applicationContext.CreateInstanceDI<DataPortalErrorInfo>(ex);
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
    private async Task<DataPortalResponse> Fetch(CriteriaRequest request)
    {
      var result = _applicationContext.CreateInstanceDI<DataPortalResponse>();
      try
      {
        request = ConvertRequest(request);

        // unpack criteria data into object
        object? criteria = GetCriteria(_applicationContext, request.CriteriaData);
        if (criteria is DataPortalClient.PrimitiveCriteria primitiveCriteria)
        {
          criteria = primitiveCriteria.Value;
        }

        var objectType = Reflection.MethodCaller.GetType(AssemblyNameTranslator.GetAssemblyQualifiedName(request.TypeName), true);
        var context = new DataPortalContext(
          _applicationContext, (IPrincipal)_applicationContext.GetRequiredService<ISerializationFormatter>().Deserialize(request.Principal),
          true,
          request.ClientCulture,
          request.ClientUICulture,
          (IContextDictionary)_applicationContext.GetRequiredService<ISerializationFormatter>().Deserialize(request.ClientContext));

        var dpr = await _dataPortalServer.Fetch(objectType, criteria, context, true);

        if (dpr.Error != null)
          result.ErrorData = _applicationContext.CreateInstanceDI<DataPortalErrorInfo>(dpr.Error);
        result.ObjectData = _applicationContext.GetRequiredService<ISerializationFormatter>().Serialize(dpr.ReturnObject);
      }
      catch (Exception ex)
      {
        result.ErrorData = _applicationContext.CreateInstanceDI<DataPortalErrorInfo>(ex);
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
    private async Task<DataPortalResponse> Update(UpdateRequest request)
    {
      var result = _applicationContext.CreateInstanceDI<DataPortalResponse>();
      try
      {
        request = ConvertRequest(request);
        // unpack object
        object? obj = GetCriteria(_applicationContext, request.ObjectData);

        var context = new DataPortalContext(
          _applicationContext, (IPrincipal)_applicationContext.GetRequiredService<ISerializationFormatter>().Deserialize(request.Principal),
          true,
          request.ClientCulture,
          request.ClientUICulture,
          (IContextDictionary)_applicationContext.GetRequiredService<ISerializationFormatter>().Deserialize(request.ClientContext));

        var dpr = await _dataPortalServer.Update(obj, context, true);

        if (dpr.Error != null)
          result.ErrorData = _applicationContext.CreateInstanceDI<DataPortalErrorInfo>(dpr.Error);

        result.ObjectData = _applicationContext.GetRequiredService<ISerializationFormatter>().Serialize(dpr.ReturnObject);
      }
      catch (Exception ex)
      {
        result.ErrorData = _applicationContext.CreateInstanceDI<DataPortalErrorInfo>(ex);
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
    private async Task<DataPortalResponse> Delete(CriteriaRequest request)
    {
      var result = _applicationContext.CreateInstanceDI<DataPortalResponse>();
      try
      {
        request = ConvertRequest(request);

        // unpack criteria data into object
        object? criteria = GetCriteria(_applicationContext, request.CriteriaData);
        if (criteria is DataPortalClient.PrimitiveCriteria primitiveCriteria)
        {
          criteria = primitiveCriteria.Value;
        }

        var objectType = Reflection.MethodCaller.GetType(AssemblyNameTranslator.GetAssemblyQualifiedName(request.TypeName), true);
        var context = new DataPortalContext(
          _applicationContext, (IPrincipal)_applicationContext.GetRequiredService<ISerializationFormatter>().Deserialize(request.Principal),
          true,
          request.ClientCulture,
          request.ClientUICulture,
          (IContextDictionary)_applicationContext.GetRequiredService<ISerializationFormatter>().Deserialize(request.ClientContext));

        var dpr = await _dataPortalServer.Delete(objectType, criteria, context, true);

        if (dpr.Error != null)
          result.ErrorData = _applicationContext.CreateInstanceDI<DataPortalErrorInfo>(dpr.Error);
        result.ObjectData = _applicationContext.GetRequiredService<ISerializationFormatter>().Serialize(dpr.ReturnObject);
      }
      catch (Exception ex)
      {
        result.ErrorData = _applicationContext.CreateInstanceDI<DataPortalErrorInfo>(ex);
        throw;
      }
      finally
      {
        result = ConvertResponse(result);
      }
      return result;
    }

    #region Criteria

    private static object? GetCriteria(ApplicationContext applicationContext, byte[]? criteriaData)
    {
      object? criteria = null;
      if (criteriaData != null)
        criteria = applicationContext.GetRequiredService<ISerializationFormatter>().Deserialize(criteriaData);
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
      Channel?.Dispose();
      Connection?.Dispose();
      GC.SuppressFinalize(this);
    }
  }
}