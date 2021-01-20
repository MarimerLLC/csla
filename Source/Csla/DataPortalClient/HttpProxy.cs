//-----------------------------------------------------------------------
// <copyright file="HttpProxy.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Implements a data portal proxy to relay data portal</summary>
//-----------------------------------------------------------------------
using Csla.Core;
using Csla.Serialization;
using Csla.Serialization.Mobile;
using Csla.Server;
using System;
using System.Linq;
using System.Net;
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
    /// <summary>
    /// Gets or sets the HttpClient timeout
    /// in milliseconds (0 uses default HttpClient/WebClient timeout).
    /// </summary>
    public int Timeout { get; set; }

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
    /// Creates an instance of the object, initializing
    /// it to use the supplied HttpClient object.
    /// </summary>
    /// <param name="httpClient">HttpClient instance</param>
    public HttpProxy(HttpClient httpClient)
    {
      this.DataPortalUrl = HttpProxy.DefaultUrl;
      _httpClient = httpClient;
    }

    /// <summary>
    /// Creates an instance of the object, initializing
    /// it to use the supplied HttpClient object and URL.
    /// </summary>
    /// <param name="httpClient">HttpClient instance</param>
    /// <param name="dataPortalUrl">Server endpoint URL</param>
    public HttpProxy(HttpClient httpClient, string dataPortalUrl)
    {
      _httpClient = httpClient;
      DataPortalUrl = dataPortalUrl;
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

    private static HttpClient _httpClient;
    
    /// <summary>
    /// Gets an HttpClient object for use in
    /// communication with the server.
    /// </summary>
    protected virtual HttpClient GetHttpClient()
    {
      if (_httpClient == null) {
        HttpClientHandler handler;
        if (UseTextSerialization)
        {
          handler = new HttpClientHandler();
        }
        else
        {
          handler = new HttpClientHandler()
          {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
          };
        }
        _httpClient = new HttpClient(handler);
        if (this.Timeout > 0) {
          _httpClient.Timeout = TimeSpan.FromMilliseconds(this.Timeout);
        }
      }

      return _httpClient;
    }

    /// <summary>
    /// Gets an WebClient object for use in
    /// communication with the server.
    /// </summary>
    protected virtual WebClient GetWebClient()
    {
      return new DefaultWebClient(this.Timeout);
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
      request.ClientContext = SerializationFormatterFactory.GetFormatter().Serialize(ApplicationContext.ClientContext);
#pragma warning disable CS0618 // Type or member is obsolete
      request.GlobalContext = SerializationFormatterFactory.GetFormatter().Serialize(ApplicationContext.GlobalContext);
#pragma warning restore CS0618 // Type or member is obsolete
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
      var request = new Csla.Server.Hosts.HttpChannel.UpdateRequest();
      request.ObjectData = null;
      request.ClientContext = SerializationFormatterFactory.GetFormatter().Serialize(ApplicationContext.ClientContext);
#pragma warning disable CS0618 // Type or member is obsolete
      request.GlobalContext = SerializationFormatterFactory.GetFormatter().Serialize(ApplicationContext.GlobalContext);
#pragma warning restore CS0618 // Type or member is obsolete
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

        serialized = await CallDataPortalServer(serialized, "create", GetRoutingToken(objectType), isSync);

        var response = (Csla.Server.Hosts.HttpChannel.HttpResponse)SerializationFormatterFactory.GetFormatter().Deserialize(serialized);
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
#pragma warning disable 1998
    public async Task<DataPortalResult> Fetch(Type objectType, object criteria, DataPortalContext context, bool isSync)
#pragma warning restore 1998
    {
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

        serialized = await CallDataPortalServer(serialized, "fetch", GetRoutingToken(objectType), isSync);

        var response = (Csla.Server.Hosts.HttpChannel.HttpResponse)SerializationFormatterFactory.GetFormatter().Deserialize(serialized);
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
#pragma warning disable 1998
    public async Task<DataPortalResult> Update(object obj, DataPortalContext context, bool isSync)
#pragma warning restore 1998
    {
      DataPortalResult result;
      try
      {
        var request = GetBaseUpdateCriteriaRequest();
        request.ObjectData = SerializationFormatterFactory.GetFormatter().Serialize(obj);
        request = ConvertRequest(request);

        var serialized = SerializationFormatterFactory.GetFormatter().Serialize(request);

        serialized = await CallDataPortalServer(serialized, "update", GetRoutingToken(obj.GetType()), isSync);

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
#pragma warning disable 1998
    public async Task<DataPortalResult> Delete(Type objectType, object criteria, DataPortalContext context, bool isSync)
#pragma warning restore 1998
    {
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

        serialized = await CallDataPortalServer(serialized, "delete", GetRoutingToken(objectType), isSync);

        var response = (Csla.Server.Hosts.HttpChannel.HttpResponse)SerializationFormatterFactory.GetFormatter().Deserialize(serialized);
        response = ConvertResponse(response);
        var globalContext = (ContextDictionary)SerializationFormatterFactory.GetFormatter().Deserialize(response.GlobalContext);
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

    private async Task<byte[]> CallDataPortalServer(byte[] serialized, string operation, string routingToken, bool isSync)
    {
      if (isSync)
        serialized = CallViaWebClient(serialized, operation, routingToken);
      else
        serialized = await CallViaHttpClient(serialized, operation, routingToken);
      return serialized;
    }

    private async Task<byte[]> CallViaHttpClient(byte[] serialized, string operation, string routingToken)
    {
      HttpClient client = GetHttpClient();
      HttpRequestMessage httpRequest;
      httpRequest = new HttpRequestMessage(
        HttpMethod.Post, 
        $"{DataPortalUrl}?operation={CreateOperationTag(operation, ApplicationContext.VersionRoutingTag, routingToken)}");
      httpRequest.Headers.Add("Accept", "*/*");
      httpRequest.Headers.Add("Accept-Encoding", "gzip,deflate,*");
      if (UseTextSerialization)
        httpRequest.Content = new StringContent(System.Convert.ToBase64String(serialized));
      else
        httpRequest.Content = new ByteArrayContent(serialized);
      var httpResponse = await client.SendAsync(httpRequest);
      await VerifyResponseSuccess(httpResponse);
      if (UseTextSerialization)
        serialized = Convert.FromBase64String(await httpResponse.Content.ReadAsStringAsync());
      else
        serialized = await httpResponse.Content.ReadAsByteArrayAsync();
      return serialized;
    }

    private byte[] CallViaWebClient(byte[] serialized, string operation, string routingToken)
    {
      WebClient client = GetWebClient();
      client.Headers.Set("Accept", "*/*");
      client.Headers.Set("Accept-Encoding", "gzip,deflate,*");
      var url = $"{DataPortalUrl}?operation={CreateOperationTag(operation, ApplicationContext.VersionRoutingTag, routingToken)}";
      if (UseTextSerialization)
      {
        var result = client.UploadString(url, System.Convert.ToBase64String(serialized));
        serialized = System.Convert.FromBase64String(result);
      }
      else
      {
        var result = client.UploadData(url, serialized);
        serialized = result;
      }
      return serialized;
    }

    private static async Task VerifyResponseSuccess(HttpResponseMessage httpResponse)
    {
      if (!httpResponse.IsSuccessStatusCode)
      {
        var message = new StringBuilder();
        message.Append((int)httpResponse.StatusCode);
        message.Append(": ");
        message.Append(httpResponse.ReasonPhrase);
        var content = await httpResponse.Content.ReadAsStringAsync();
        if (!string.IsNullOrWhiteSpace(content))
        {
          message.AppendLine();
          message.Append(content);
        }
        throw new HttpRequestException(message.ToString());
      }
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

    private class DefaultWebClient : WebClient
    {
      private int Timeout { get; set; }

      public DefaultWebClient(int timeout)
      {
        Timeout = timeout;
      }

      protected override WebRequest GetWebRequest(Uri address)
      {
        var req = base.GetWebRequest(address) as HttpWebRequest;
        req.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
        if (Timeout > 0)
          req.Timeout = Timeout;
        return req;
      }
    }
  }
}