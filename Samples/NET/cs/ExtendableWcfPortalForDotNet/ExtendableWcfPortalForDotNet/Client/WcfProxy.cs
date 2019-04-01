using System;
using System.ServiceModel;
using System.Threading.Tasks;
using Csla.Server;
using Csla.Serialization;
using Csla.Core;

namespace ExtendableWcfPortalForDotNet.Client
{
  public class WcfProxy : Csla.DataPortalClient.IDataPortalProxy
  {
    #region IDataPortalProxy Members

    /// <summary>
    /// Gets a value indicating whether the data portal
    /// is hosted on a remote server.
    /// </summary>
    public bool IsServerRemote
    {
      get { return true; }
    }

    #endregion

    #region IDataPortalServer Members

    private string _endPoint = "WcfDataPortal";

    private Type _objectType;
    private object _criteria;
    private DataPortalContext _context;
    private object _obj;

    /// <summary>
    /// Gets or sets the WCF endpoint used
    /// to contact the server.
    /// </summary>
    /// <remarks>
    /// The default value is WcfDataPortal.
    /// </remarks>
    protected string EndPoint
    {
      get { return _endPoint; }
      set { _endPoint = value; }
    }

    /// <summary>
    /// Returns an instance of the channel factory
    /// used by GetProxy() to create the WCF proxy
    /// object.
    /// </summary>
    protected virtual ChannelFactory<IExtendableWcfPortalForDotNet> GetChannelFactory()
    {
      return new ChannelFactory<IExtendableWcfPortalForDotNet>(_endPoint);
    }

    /// <summary>
    /// Returns the WCF proxy object used for
    /// communication with the data portal
    /// server.
    /// </summary>
    /// <param name="cf">
    /// The ChannelFactory created by GetChannelFactory().
    /// </param>
    protected virtual IExtendableWcfPortalForDotNet GetProxy(ChannelFactory<IExtendableWcfPortalForDotNet> cf)
    {
      return cf.CreateChannel();
    }

    /// <summary>
    /// Called by <see cref="DataPortal" /> to create a
    /// new business object.
    /// </summary>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context"><see cref="DataPortalContext" /> object passed to the server.</param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    /// <returns></returns>
    public Task<DataPortalResult> Create(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      _objectType = objectType;
      _criteria = criteria;
      _context = context;

      return new Task<DataPortalResult>(Create);
    }

    private DataPortalResult Create()
    {
      var cf = GetChannelFactory();
      var svr = GetProxy(cf);
      WcfResponse response;
      var formatter = SerializationFormatterFactory.GetFormatter();
      try
      {
        response = svr.Create(GetRequest(formatter, _objectType, _criteria, _context));
        if (cf != null)
          cf.Close();
      }
      catch
      {
        cf.Abort();
        throw;
      }
      var result = GetResult(formatter, response);
      var error = (Exception) formatter.Deserialize(response.Error);
      if (error != null)
      {
        throw error;
      }

      return result;
    }

    /// <summary>
    /// Called by <see cref="DataPortal" /> to load an
    /// existing business object.
    /// </summary>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context"><see cref="DataPortalContext" /> object passed to the server.</param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    /// <returns></returns>
    public Task<DataPortalResult> Fetch(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      _objectType = objectType;
      _criteria = criteria;
      _context = context;

      return new Task<DataPortalResult>(Fetch);
    }

    private DataPortalResult Fetch()
    {
      var cf = GetChannelFactory();
      var svr = GetProxy(cf);
      WcfResponse response;
      var formatter = SerializationFormatterFactory.GetFormatter();
      try
      {
        response = svr.Fetch(GetRequest(formatter, _objectType, _criteria, _context));
        if (cf != null)
          cf.Close();
      }
      catch
      {
        cf.Abort();
        throw;
      }
      var result = GetResult(formatter, response);
      var error = (Exception) formatter.Deserialize(response.Error);
      if (error != null)
      {
        throw error;
      }
      return result;
    }

    /// <summary>
    /// Called by <see cref="DataPortal" /> to update a
    /// business object.
    /// </summary>
    /// <param name="obj">The business object to update.</param>
    /// <param name="context"><see cref="DataPortalContext" /> object passed to the server.</param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    /// <returns></returns>
    public Task<DataPortalResult> Update(object obj, DataPortalContext context, bool isSync)
    {
      _obj = obj;
      _context = context;

      return new Task<DataPortalResult>(Update);
    }

    private DataPortalResult Update()
    {
      var cf = GetChannelFactory();
      var svr = GetProxy(cf);
      WcfResponse response;
      var formatter = SerializationFormatterFactory.GetFormatter();
      try
      {
        response = svr.Update(GetRequest(formatter, _obj, _context));
        if (cf != null)
          cf.Close();
      }
      catch
      {
        cf.Abort();
        throw;
      }

      var result = GetResult(formatter, response);
      var error = (Exception) formatter.Deserialize(response.Error);
      if (error != null)
      {
        throw error;
      }
      return result;
    }

    /// <summary>
    /// Called by <see cref="DataPortal" /> to delete a
    /// business object.
    /// </summary>
    /// <param name="objectType">Type of business object to create.</param>
    /// <param name="criteria">Criteria object describing business object.</param>
    /// <param name="context"><see cref="DataPortalContext" /> object passed to the server.</param>
    /// <param name="isSync">True if the client-side proxy should synchronously invoke the server.</param>
    /// <returns></returns>
    public Task<DataPortalResult> Delete(Type objectType, object criteria, DataPortalContext context, bool isSync)
    {
      _objectType = objectType;
      _criteria = criteria;
      _context = context;

      return new Task<DataPortalResult>(Delete);
    }

    private DataPortalResult Delete()
    {
      var cf = GetChannelFactory();
      var svr = GetProxy(cf);
      WcfResponse response;
      var formatter = SerializationFormatterFactory.GetFormatter();
      try
      {
        response = svr.Delete(GetRequest(formatter, _objectType, _criteria, _context));
        if (cf != null)
          cf.Close();
      }
      catch
      {
        cf.Abort();
        throw;
      }
      var result = GetResult(formatter, response);
      var error = (Exception) formatter.Deserialize(response.Error);
      if (error != null)
      {
        throw error;
      }
      return result;
    }

    #endregion

    private CriteriaRequest GetRequest(ISerializationFormatter formatter, Type objectType, object criteria,
      DataPortalContext context)
    {
      var request = new CriteriaRequest();
      request.ClientContext = formatter.Serialize(Csla.ApplicationContext.ClientContext);
      request.ClientCulture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
      request.ClientUICulture = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
      request.CriteriaData = formatter.Serialize(criteria);
      request.GlobalContext = formatter.Serialize(Csla.ApplicationContext.GlobalContext);
      request.Principal = formatter.Serialize(Csla.ApplicationContext.User);
      request.TypeName = objectType.AssemblyQualifiedName;
      request = ConvertRequest(request);
      return request;
    }

    private UpdateRequest GetRequest(ISerializationFormatter formatter, object obj, DataPortalContext context)
    {
      var request = new UpdateRequest();
      request.ClientContext = formatter.Serialize(Csla.ApplicationContext.ClientContext);
      request.ClientCulture = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
      request.ClientUICulture = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
      request.GlobalContext = formatter.Serialize(Csla.ApplicationContext.GlobalContext);
      request.Principal = formatter.Serialize(Csla.ApplicationContext.User);
      request.ObjectData = formatter.Serialize(obj);
      request = ConvertRequest(request);
      return request;
    }

    private DataPortalResult GetResult(ISerializationFormatter formatter, WcfResponse response)
    {
      response = ConvertResponse(response);
      object result = formatter.Deserialize(response.ObjectData);
      ContextDictionary globalContext = (ContextDictionary) formatter.Deserialize(response.GlobalContext);
      DataPortalResult returnValue = new DataPortalResult(result, globalContext);
      return returnValue;
    }

    protected virtual WcfResponse ConvertResponse(WcfResponse response)
    {
      return response;
    }

    protected virtual CriteriaRequest ConvertRequest(CriteriaRequest request)
    {
      return request;
    }

    protected virtual UpdateRequest ConvertRequest(UpdateRequest request)
    {
      return request;
    }
  }
}