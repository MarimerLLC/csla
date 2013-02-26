using System;
using Csla.Server.Hosts.WcfBfChannel;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace Csla.Server.Hosts
{
  /// <summary>
  /// Exposes server-side DataPortal functionality
  /// through WCF using the BinaryFormatter.
  /// </summary>
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
  [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
  public class WcfBfPortal : IWcfBfPortal
  {
    #region IWcfPortal Members

    /// <summary>
    /// Create a new business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    public byte[] Create(byte[] req)
    {
      var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
      Csla.Server.Hosts.WcfBfChannel.CreateRequest request;
      using (var buffer = new System.IO.MemoryStream(req))
      {
        request = (Csla.Server.Hosts.WcfBfChannel.CreateRequest)formatter.Deserialize(buffer);
      }
      Csla.Server.DataPortal portal = new Csla.Server.DataPortal();
      object result;
      try
      {
        result = portal.Create(request.ObjectType, request.Criteria, request.Context);
      }
      catch (Exception ex)
      {
        result = ex;
      }
      var response = new WcfResponse(result);
      using (var buffer = new System.IO.MemoryStream())
      {
        formatter.Serialize(buffer, response);
        return buffer.ToArray();
      }
    }

    /// <summary>
    /// Get an existing business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    public byte[] Fetch(byte[] req)
    {
      var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
      Csla.Server.Hosts.WcfBfChannel.FetchRequest request;
      using (var buffer = new System.IO.MemoryStream(req))
      {
        request = (Csla.Server.Hosts.WcfBfChannel.FetchRequest)formatter.Deserialize(buffer);
      }
      Csla.Server.DataPortal portal = new Csla.Server.DataPortal();
      object result;
      try
      {
        result = portal.Fetch(request.ObjectType, request.Criteria, request.Context);
      }
      catch (Exception ex)
      {
        result = ex;
      }
      var response = new WcfResponse(result);
      using (var buffer = new System.IO.MemoryStream())
      {
        formatter.Serialize(buffer, response);
        return buffer.ToArray();
      }
    }

    /// <summary>
    /// Update a business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    public byte[] Update(byte[] req)
    {
      var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
      Csla.Server.Hosts.WcfBfChannel.UpdateRequest request;
      using (var buffer = new System.IO.MemoryStream(req))
      {
        request = (Csla.Server.Hosts.WcfBfChannel.UpdateRequest)formatter.Deserialize(buffer);
      }
      Csla.Server.DataPortal portal = new Csla.Server.DataPortal();
      object result;
      try
      {
        result = portal.Update(request.Object, request.Context);
      }
      catch (Exception ex)
      {
        result = ex;
      }
      var response = new WcfResponse(result);
      using (var buffer = new System.IO.MemoryStream())
      {
        formatter.Serialize(buffer, response);
        return buffer.ToArray();
      }
    }

    /// <summary>
    /// Delete a business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    public byte[] Delete(byte[] req)
    {
      var formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
      Csla.Server.Hosts.WcfBfChannel.DeleteRequest request;
      using (var buffer = new System.IO.MemoryStream(req))
      {
        request = (Csla.Server.Hosts.WcfBfChannel.DeleteRequest)formatter.Deserialize(buffer);
      }
      Csla.Server.DataPortal portal = new Csla.Server.DataPortal();
      object result;
      try
      {
        result = portal.Delete(request.ObjectType, request.Criteria, request.Context);
      }
      catch (Exception ex)
      {
        result = ex;
      }
      var response = new WcfResponse(result);
      using (var buffer = new System.IO.MemoryStream())
      {
        formatter.Serialize(buffer, response);
        return buffer.ToArray();
      }
    }

    #endregion
  }
}
