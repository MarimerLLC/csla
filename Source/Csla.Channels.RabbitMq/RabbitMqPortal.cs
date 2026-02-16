//-----------------------------------------------------------------------
// <copyright file="RabbitMqPortal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Exposes server-side DataPortal functionality through RabbitMQ</summary>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Security.Principal;
using Csla.Core;
using Csla.Properties;
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
    private IChannel? Channel;
    private string? DataPortalQueueName;

    private Uri DataPortalUri { get; set; }

    [MemberNotNull(nameof(DataPortalUri), nameof(DataPortalQueueName), nameof(Connection), nameof(Channel))]
    private async Task InitializeRabbitMQ()
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
#pragma warning disable CS8774 // 11/22/2024, Nullable analysis can't track nullability with async/await
        Connection = await factory.CreateConnectionAsync();
        Channel = await Connection.CreateChannelAsync();
#pragma warning restore CS8774
      }
    }

    /// <summary>
    /// Start processing inbound messages.
    /// </summary>
    public async Task StartListening()
    {
      await InitializeRabbitMQ();
      await Channel!.QueueDeclareAsync(queue: DataPortalQueueName!, durable: false, exclusive: false, autoDelete: false);

      var consumer = new AsyncEventingBasicConsumer(Channel);
      consumer.ReceivedAsync += async (_, ea) => await InvokePortal(ea, ea.Body.ToArray());
      await Channel.BasicConsumeAsync(queue: DataPortalQueueName!, autoAck: true, consumer: consumer);
    }

    private async Task InvokePortal(BasicDeliverEventArgs ea, byte[] requestData)
    {
      if (ea.BasicProperties.ReplyTo is null)
      {
        throw new InvalidOperationException($"{nameof(BasicDeliverEventArgs.BasicProperties)}.{nameof(IReadOnlyBasicProperties.ReplyTo)} == null");
      }
      if (ea.BasicProperties.CorrelationId is null)
      {
        throw new InvalidOperationException($"{nameof(BasicDeliverEventArgs.BasicProperties)}.{nameof(IReadOnlyBasicProperties.CorrelationId)} == null");
      }

      DataPortalResponse result;
      try
      {
        var request = Deserialize<object>(requestData);
        result = await CallPortal(ea.BasicProperties.Type ?? throw new InvalidOperationException($"{nameof(BasicDeliverEventArgs.BasicProperties)}.{nameof(IReadOnlyBasicProperties.Type)} == null"), request);
      }
      catch (Exception ex)
      {
        result = new DataPortalResponse(_applicationContext.CreateInstanceDI<DataPortalErrorInfo>(ex));
      }

      try
      {
        var response = _applicationContext.GetRequiredService<ISerializationFormatter>().Serialize(result);
        await SendMessage(ea.BasicProperties.ReplyTo, ea.BasicProperties.CorrelationId, response);
      }
      catch (Exception ex)
      {
        var errorResult = new DataPortalResponse(_applicationContext.CreateInstanceDI<DataPortalErrorInfo>(ex));
        var response = _applicationContext.GetRequiredService<ISerializationFormatter>().Serialize(errorResult);
        await SendMessage(ea.BasicProperties.ReplyTo, ea.BasicProperties.CorrelationId, response);
      }
    }

    private async Task SendMessage(string target, string correlationId, byte[] request)
    {
      await InitializeRabbitMQ();
      if (Channel is null)
        throw new InvalidOperationException($"{nameof(Channel)} == null");
      var props = new BasicProperties
      {
        CorrelationId = correlationId
      };
      await Channel.BasicPublishAsync(exchange: "", routingKey: target, mandatory: true, basicProperties: props, body: request);
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
      var result = new DataPortalResponse();
      try
      {
        request = ConvertRequest(request);

        // unpack criteria data into object
        object? criteria = GetCriteria(_applicationContext, request.CriteriaData);
        if (criteria is DataPortalClient.PrimitiveCriteria primitiveCriteria)
        {
          criteria = primitiveCriteria.Value;
        }

        var objectType = Reflection.MethodCaller.GetType(AssemblyNameTranslator.GetAssemblyQualifiedName(request.TypeName));
        var context = new DataPortalContext(
          _applicationContext, Deserialize<IPrincipal>(request.Principal),
          true,
          request.ClientCulture,
          request.ClientUICulture,
          Deserialize<IContextDictionary>(request.ClientContext));

        var dpr = await _dataPortalServer.Create(objectType, criteria, context, true);

        if (dpr.Error != null)
          result.ErrorData = _applicationContext.CreateInstanceDI<DataPortalErrorInfo>(dpr.Error);
        result.ObjectData = _applicationContext.GetRequiredService<ISerializationFormatter>().Serialize(dpr.ReturnObject);
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
      var result = new DataPortalResponse();
      try
      {
        request = ConvertRequest(request);

        // unpack criteria data into object
        object? criteria = GetCriteria(_applicationContext, request.CriteriaData);
        if (criteria is DataPortalClient.PrimitiveCriteria primitiveCriteria)
        {
          criteria = primitiveCriteria.Value;
        }

        var objectType = Reflection.MethodCaller.GetType(AssemblyNameTranslator.GetAssemblyQualifiedName(request.TypeName));
        var context = new DataPortalContext(
          _applicationContext, Deserialize<IPrincipal>(request.Principal),
          true,
          request.ClientCulture,
          request.ClientUICulture,
          Deserialize<IContextDictionary>(request.ClientContext));

        var dpr = await _dataPortalServer.Fetch(objectType, criteria, context, true);

        if (dpr.Error != null)
          result.ErrorData = _applicationContext.CreateInstanceDI<DataPortalErrorInfo>(dpr.Error);
        result.ObjectData = _applicationContext.GetRequiredService<ISerializationFormatter>().Serialize(dpr.ReturnObject);
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
      var result = new DataPortalResponse();
      try
      {
        request = ConvertRequest(request);
        // unpack object
        object obj = GetCriteria(_applicationContext, request.ObjectData ?? throw new InvalidOperationException(Resources.ObjectToBeUpdatedCouldNotBeDeserialized));

        var context = new DataPortalContext(
          _applicationContext, Deserialize<IPrincipal>(request.Principal),
          true,
          request.ClientCulture,
          request.ClientUICulture,
          Deserialize<IContextDictionary>(request.ClientContext));

        var dpr = await _dataPortalServer.Update((ICslaObject)obj, context, true);

        if (dpr.Error != null)
          result.ErrorData = _applicationContext.CreateInstanceDI<DataPortalErrorInfo>(dpr.Error);

        result.ObjectData = _applicationContext.GetRequiredService<ISerializationFormatter>().Serialize(dpr.ReturnObject);
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
      var result = new DataPortalResponse();
      try
      {
        request = ConvertRequest(request);

        // unpack criteria data into object
        object criteria = GetCriteria(_applicationContext, request.CriteriaData);
        if (criteria is DataPortalClient.PrimitiveCriteria primitiveCriteria)
        {
          criteria = primitiveCriteria.Value;
        }

        var objectType = Reflection.MethodCaller.GetType(AssemblyNameTranslator.GetAssemblyQualifiedName(request.TypeName));
        var context = new DataPortalContext(
          _applicationContext, Deserialize<IPrincipal>(request.Principal),
          true,
          request.ClientCulture,
          request.ClientUICulture,
          Deserialize<IContextDictionary>(request.ClientContext));

        var dpr = await _dataPortalServer.Delete(objectType, criteria, context, true);

        if (dpr.Error != null)
          result.ErrorData = _applicationContext.CreateInstanceDI<DataPortalErrorInfo>(dpr.Error);
        result.ObjectData = _applicationContext.GetRequiredService<ISerializationFormatter>().Serialize(dpr.ReturnObject);
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
      return applicationContext.GetRequiredService<ISerializationFormatter>().Deserialize(criteriaData) ?? throw new SerializationException(Resources.ServerSideDataPortalRequestDeserializationFailed);
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

    private T Deserialize<T>(byte[] data)
    {
      var deserializedData = _applicationContext.GetRequiredService<ISerializationFormatter>().Deserialize(data) ?? throw new SerializationException(Resources.ServerSideDataPortalRequestDeserializationFailed);
      if (deserializedData is not T castedData)
      {
        throw new SerializationException(string.Format(Resources.DeserializationFailedDueToWrongData, typeof(T).FullName));
      }

      return castedData;
    }

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