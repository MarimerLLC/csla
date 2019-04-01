#if !NETFX_PHONE && !PCL259 || PCL46
//-----------------------------------------------------------------------
// <copyright file="HttpProxy.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Implements a data portal proxy to relay data portal</summary>
//-----------------------------------------------------------------------
using Csla.Core;
using Csla.Serialization.Mobile;
using Csla.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Csla.DataPortalClient
{
  /// <summary>
  /// Implements a data portal proxy to relay data portal
  /// calls to a remote application server by using http.
  /// </summary>
  public class HttpProxy : IDataPortalProxy
  {
    private int _timeoutInMilliseconds = 0;

    /// <summary>
    /// Gets or sets the HttpClient timeout
    /// in milliseconds (0 uses default HttpClient timeout).
    /// </summary>
    public int Timeout
    {
      get { return _timeoutInMilliseconds; }
      set { _timeoutInMilliseconds = value; }
    }

    /// <summary>
    /// Gets or sets the default URL address
    /// for the data portal server.
    /// </summary>
    /// <remarks>
    /// Deprecated: use ApplicationContext.DataPortalUrlString
    /// </remarks>
    public static string DefaultUrl
    {
      get { return ApplicationContext.DataPortalUrlString; }
      set { ApplicationContext.DataPortalUrlString = value; }
    }

    /// <summary>
    /// Creates an instance of the object, initializing
    /// it to use the DefaultUrl 
    /// values.
    /// </summary>
    public HttpProxy()
    {
      this.DataPortalUrl = HttpProxy.DefaultUrl;
    }

    /// <summary>
    /// Creates an instance of the object, initializing
    /// it to use the supplied URL.
    /// </summary>
    /// <param name="dataPortalUrl">Server endpoint URL</param>
    public HttpProxy(string dataPortalUrl)
    {
      this.DataPortalUrl = dataPortalUrl;
    }

    /// <summary>
    /// Gets a value indicating whether the data portal
    /// is hosted on a remote server.
    /// </summary>
    public bool IsServerRemote
    {
      get { return true; }
    }

    /// <summary>
    /// Gets the URL address for the data portal server
    /// used by this proxy instance.
    /// </summary>
    public string DataPortalUrl { get; protected set; }

    private static HttpClient _client;
    
    /// <summary>
    /// Gets an HttpClient object for use in
    /// communication with the server.
    /// </summary>
    protected virtual HttpClient GetClient()
    {
      if (_client == null) {
        _client = new HttpClient();
        if (this.Timeout > 0) {
          _client.Timeout = TimeSpan.FromMilliseconds(this.Timeout);
        }
      }

      return _client;
    }

    /// <summary>
    /// Set HttpClient object for use by data portal.
    /// </summary>
    /// <param name="client">HttpClient instance.</param>
    public static void SetHttpClient(HttpClient client)
    {
      _client = client;
    }

    /// <summary>
    /// Gets or sets a value indicating whether to use
    /// text/string serialization instead of the default
    /// binary serialization.
    /// </summary>
    public static bool UseTextSerialization { get; set; } = false;

    #region Criteria

    private Csla.Server.Hosts.HttpChannel.CriteriaRequest GetBaseCriteriaRequest()
    {
      var request = new Csla.Server.Hosts.HttpChannel.CriteriaRequest();
      request.CriteriaData = null;
      request.ClientContext = MobileFormatter.Serialize(ApplicationContext.ClientContext);
      request.GlobalContext = MobileFormatter.Serialize(ApplicationContext.GlobalContext);
      if (ApplicationContext.AuthenticationType == "Windows")
      {
        request.Principal = MobileFormatter.Serialize(null);
      }
      else
      {
        request.Principal = MobileFormatter.Serialize(ApplicationContext.User);
      }
      request.ClientCulture = System.Globalization.CultureInfo.CurrentCulture.Name;
      request.ClientUICulture = System.Globalization.CultureInfo.CurrentUICulture.Name;
      return request;
    }

    private Csla.Server.Hosts.HttpChannel.UpdateRequest GetBaseUpdateCriteriaRequest()
    {
      var request = new Csla.Server.Hosts.HttpChannel.UpdateRequest();
      request.ObjectData = null;
      request.ClientContext = MobileFormatter.Serialize(ApplicationContext.ClientContext);
      request.GlobalContext = MobileFormatter.Serialize(ApplicationContext.GlobalContext);
      if (ApplicationContext.AuthenticationType == "Windows")
      {
        request.Principal = MobileFormatter.Serialize(null);
      }
      else
      {
        request.Principal = MobileFormatter.Serialize(ApplicationContext.User);
      }
#if NETCORE || PCL46 || PCL259
      request.ClientCulture = System.Globalization.CultureInfo.CurrentCulture.Name;
      request.ClientUICulture = System.Globalization.CultureInfo.CurrentUICulture.Name;
#elif NETFX_CORE || NETFX_PHONE
      var language = Windows.ApplicationModel.Resources.Core.ResourceContext.GetForCurrentView().Languages[0];
      request.ClientCulture = language;
      request.ClientUICulture = language;
#else
      request.ClientCulture = Thread.CurrentThread.CurrentCulture.Name;
      request.ClientUICulture = Thread.CurrentThread.CurrentUICulture.Name;
#endif
      return request;
    }

#endregion

    /// <summary>
    /// Called by <see cref="DataPortal" /> to create a
    /// new business object.
    /// </summary>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context">
    /// <see cref="Server.DataPortalContext" /> object passed to the server.
    /// </param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    public async Task<DataPortalResult> Create(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      DataPortalResult result = null;
      try
      {
        if (isSync)
          throw new NotSupportedException("isSync == true");
        var client = GetClient();
        var request = GetBaseCriteriaRequest();
        request.TypeName = AssemblyNameTranslator.GetAssemblyQualifiedName(objectType.AssemblyQualifiedName);
        if (!(criteria is IMobileObject))
        {
          criteria = new PrimitiveCriteria(criteria);
        }
        request.CriteriaData = MobileFormatter.Serialize(criteria);
        request = ConvertRequest(request);

        var serialized = MobileFormatter.Serialize(request);

        serialized = await CallDataPortalServer(client, serialized, "create", GetRoutingToken(objectType));

        var response = (Csla.Server.Hosts.HttpChannel.HttpResponse)MobileFormatter.Deserialize(serialized);
        response = ConvertResponse(response);
        var globalContext = (ContextDictionary)MobileFormatter.Deserialize(response.GlobalContext);
        if (response != null && response.ErrorData == null)
        {
          var obj = MobileFormatter.Deserialize(response.ObjectData);
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
#pragma warning disable 1998
    public async Task<DataPortalResult> Fetch(Type objectType, object criteria, DataPortalContext context, bool isSync)
#pragma warning restore 1998
    {
      DataPortalResult result = null;
      try
      {
        if (isSync)
          throw new NotSupportedException("isSync == true");
        var client = GetClient();
        var request = GetBaseCriteriaRequest();
        request.TypeName = AssemblyNameTranslator.GetAssemblyQualifiedName(objectType.AssemblyQualifiedName);
        if (!(criteria is IMobileObject))
        {
          criteria = new PrimitiveCriteria(criteria);
        }
        request.CriteriaData = MobileFormatter.Serialize(criteria);
        request = ConvertRequest(request);

        var serialized = MobileFormatter.Serialize(request);

        serialized = await CallDataPortalServer(client, serialized, "fetch", GetRoutingToken(objectType));

        var response = (Csla.Server.Hosts.HttpChannel.HttpResponse)MobileFormatter.Deserialize(serialized);
        response = ConvertResponse(response);
        var globalContext = (ContextDictionary)MobileFormatter.Deserialize(response.GlobalContext);
        if (response != null && response.ErrorData == null)
        {
          var obj = MobileFormatter.Deserialize(response.ObjectData);
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
#pragma warning disable 1998
    public async Task<DataPortalResult> Update(object obj, DataPortalContext context, bool isSync)
#pragma warning restore 1998
    {
      DataPortalResult result = null;
      try
      {
        if (isSync)
          throw new NotSupportedException("isSync == true");
        var client = GetClient();
        var request = GetBaseUpdateCriteriaRequest();
        request.ObjectData = MobileFormatter.Serialize(obj);
        request = ConvertRequest(request);

        var serialized = MobileFormatter.Serialize(request);

        serialized = await CallDataPortalServer(client, serialized, "update", GetRoutingToken(obj.GetType()));

        var response = (Csla.Server.Hosts.HttpChannel.HttpResponse)MobileFormatter.Deserialize(serialized);
        response = ConvertResponse(response);
        var globalContext = (ContextDictionary)MobileFormatter.Deserialize(response.GlobalContext);
        if (response != null && response.ErrorData == null)
        {
          var newobj = MobileFormatter.Deserialize(response.ObjectData);
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
#pragma warning disable 1998
    public async Task<DataPortalResult> Delete(Type objectType, object criteria, DataPortalContext context, bool isSync)
#pragma warning restore 1998
    {
      DataPortalResult result = null;
      try
      {
        if (isSync)
          throw new NotSupportedException("isSync == true");
        var client = GetClient();
        var request = GetBaseCriteriaRequest();
        request.TypeName = AssemblyNameTranslator.GetAssemblyQualifiedName(objectType.AssemblyQualifiedName);
        if (!(criteria is IMobileObject))
        {
          criteria = new PrimitiveCriteria(criteria);
        }
        request.CriteriaData = MobileFormatter.Serialize(criteria);
        request = ConvertRequest(request);

        var serialized = MobileFormatter.Serialize(request);

        serialized = await CallDataPortalServer(client, serialized, "delete", GetRoutingToken(objectType));

        var response = (Csla.Server.Hosts.HttpChannel.HttpResponse)MobileFormatter.Deserialize(serialized);
        response = ConvertResponse(response);
        var globalContext = (ContextDictionary)MobileFormatter.Deserialize(response.GlobalContext);
        if (response != null && response.ErrorData == null)
        {
          result = new DataPortalResult(null, null, globalContext);
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

    private async Task<byte[]> CallDataPortalServer(HttpClient client, byte[] serialized, string operation, string routingToken)
    {
      HttpRequestMessage httpRequest = null;
      httpRequest = new HttpRequestMessage(
        HttpMethod.Post, 
        $"{DataPortalUrl}?operation={CreateOperationTag(operation, ApplicationContext.VersionRoutingTag, routingToken)}");
      if (UseTextSerialization)
        httpRequest.Content = new StringContent(System.Convert.ToBase64String(serialized));
      else
        httpRequest.Content = new ByteArrayContent(serialized);
      var httpResponse = await client.SendAsync(httpRequest);
      httpResponse.EnsureSuccessStatusCode();
      if (UseTextSerialization)
        serialized = System.Convert.FromBase64String(await httpResponse.Content.ReadAsStringAsync());
      else
        serialized = await httpResponse.Content.ReadAsByteArrayAsync();
      return serialized;
    }

    private string CreateOperationTag(string operatation, string versionToken, string routingToken)
    {
      if (!string.IsNullOrWhiteSpace(versionToken) || !string.IsNullOrWhiteSpace(routingToken))
        return $"{operatation}/{routingToken}-{versionToken}";
      else
        return operatation;
    }

    private string GetRoutingToken(Type objectType)
    {
      string result = null;
      var list = objectType.GetCustomAttributes(typeof(DataPortalServerRoutingTagAttribute), false);
      if (list.Count() > 0)
        result = ((DataPortalServerRoutingTagAttribute)list[0]).RoutingTag;
      return result;
    }

#region Extension Method for Requests

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
  }
}
#endif