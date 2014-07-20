using Csla.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Csla.Server.Hosts
{
  /// <summary>
  /// Data portal to be invoked by the brokered 
  /// data portal proxy/host implementation.
  /// </summary>
  public class BrokeredPortal
  {
    /// <summary>
    /// Create and initialize a business object.
    /// </summary>
    /// <param name="objectTypeName">Type of business object to create.</param>
    /// <param name="criteriaData">Criteria object describing business object.</param>
    /// <param name="contextData">
    /// Server.DataPortalContext object passed to the server.
    /// </param>
    public async Task<byte[]> Create(string objectTypeName, byte[] criteriaData, byte[] contextData)
    {
      DataPortalResult result;
      try
      {
        var objectType = Csla.Reflection.MethodCaller.GetType(objectTypeName);
        var criteria = Csla.Serialization.Mobile.MobileFormatter.Deserialize(criteriaData);
        var context = (DataPortalContext)Csla.Serialization.Mobile.MobileFormatter.Deserialize(contextData);
        var portal = new Csla.Server.DataPortal();
        result = await portal.Create(objectType, criteria, context, false);
      }
      catch (Exception ex)
      {
        result = new DataPortalResult(null, ex, null);
      }
      var response = GetDataPortalResult(result);
      var resultData = Csla.Serialization.Mobile.MobileFormatter.Serialize(response);
      return resultData;
    }

    /// <summary>
    /// Get an existing business object.
    /// </summary>
    /// <param name="objectTypeName">Type of business object to retrieve.</param>
    /// <param name="criteriaData">Criteria object describing business object.</param>
    /// <param name="contextData">
    /// Server.DataPortalContext object passed to the server.
    /// </param>
    public async Task<byte[]> Fetch(string objectTypeName, byte[] criteriaData, byte[] contextData)
    {
      DataPortalResult result;
      try
      {
        var objectType = Csla.Reflection.MethodCaller.GetType(objectTypeName);
        var criteria = Csla.Serialization.Mobile.MobileFormatter.Deserialize(criteriaData);
        var context = (DataPortalContext)Csla.Serialization.Mobile.MobileFormatter.Deserialize(contextData);
        var portal = new Csla.Server.DataPortal();
        result = await portal.Fetch(objectType, criteria, context, false);
      }
      catch (Exception ex)
      {
        result = new DataPortalResult(null, ex, null);
      }
      var response = GetDataPortalResult(result);
      var resultData = Csla.Serialization.Mobile.MobileFormatter.Serialize(response);
      return resultData;
    }

    /// <summary>
    /// Update a business object.
    /// </summary>
    /// <param name="objectData">Business object to update.</param>
    /// <param name="contextData">
    /// Server.DataPortalContext object passed to the server.
    /// </param>
    public async Task<byte[]> Update(byte[] objectData, byte[] contextData)
    {
      DataPortalResult result;
      try
      {
        var obj = Csla.Serialization.Mobile.MobileFormatter.Deserialize(objectData);
        var context = (DataPortalContext)Csla.Serialization.Mobile.MobileFormatter.Deserialize(contextData);
        var portal = new Csla.Server.DataPortal();
        result = await portal.Update(obj, context, false);
      }
      catch (Exception ex)
      {
        result = new DataPortalResult(null, ex, null);
      }
      var response = GetDataPortalResult(result);
      var resultData = Csla.Serialization.Mobile.MobileFormatter.Serialize(response);
      return resultData;
    }

    /// <summary>
    /// Delete a business object.
    /// </summary>
    /// <param name="objectTypeName">Type of business object to create.</param>
    /// <param name="criteriaData">Criteria object describing business object.</param>
    /// <param name="contextData">
    /// Server.DataPortalContext object passed to the server.
    /// </param>
    public async Task<byte[]> Delete(string objectTypeName, byte[] criteriaData, byte[] contextData)
    {
      DataPortalResult result;
      try
      {
        var objectType = Csla.Reflection.MethodCaller.GetType(objectTypeName);
        var criteria = Csla.Serialization.Mobile.MobileFormatter.Deserialize(criteriaData);
        var context = (DataPortalContext)Csla.Serialization.Mobile.MobileFormatter.Deserialize(contextData);
        var portal = new Csla.Server.DataPortal();
        result = await portal.Delete(objectType, criteria, context, false);
      }
      catch (Exception ex)
      {
        result = new DataPortalResult(null, ex, null);
      }
      var response = GetDataPortalResult(result);
      var resultData = Csla.Serialization.Mobile.MobileFormatter.Serialize(response);
      return resultData;
    }

    private Csla.Server.Hosts.HttpChannel.HttpResponse GetDataPortalResult(DataPortalResult dataportalResult)
    {
      var result = new Csla.Server.Hosts.HttpChannel.HttpResponse();
      if (dataportalResult.Error != null)
        result.ErrorData = new HttpChannel.HttpErrorInfo(dataportalResult.Error);
      if (dataportalResult.GlobalContext != null)
        result.GlobalContext = Csla.Serialization.Mobile.MobileFormatter.Serialize(dataportalResult.GlobalContext);
      if (dataportalResult.ReturnObject != null)
        result.ObjectData = Csla.Serialization.Mobile.MobileFormatter.Serialize(dataportalResult.ReturnObject);
      return result;
    }
  }
}
