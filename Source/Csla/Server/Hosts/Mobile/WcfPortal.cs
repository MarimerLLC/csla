#if !NETSTANDARD2_0 && !NET5_0
//-----------------------------------------------------------------------
// <copyright file="WcfPortal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Exposes server-side DataPortal functionality</summary>
//-----------------------------------------------------------------------
using System;
using Csla.Serialization.Mobile;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Csla.Core;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Csla.Server.Hosts.Mobile
{
  /// <summary>
  /// Exposes server-side DataPortal functionality
  /// through WCF.
  /// </summary>
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
  [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
  public class WcfPortal : IWcfPortal
  {

#region IWcfPortal Members

    /// <summary>
    /// Create a new business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
    public async Task<WcfResponse> Create(CriteriaRequest request)
    {
      var result = new WcfResponse();
      try
      {
        request = ConvertRequest(request);

        // unpack criteria data into object
        object criteria = GetCriteria(request.CriteriaData);
        if (criteria is Csla.DataPortalClient.PrimitiveCriteria)
        {
          criteria = ((Csla.DataPortalClient.PrimitiveCriteria)criteria).Value;
        }

        var processor = new MobileRequestProcessor();
        var createRequest = new MobileCriteriaRequest(
          request.TypeName,
          criteria,
          (IPrincipal)MobileFormatter.Deserialize(request.Principal),
          (ContextDictionary)MobileFormatter.Deserialize(request.GlobalContext),
          (ContextDictionary)MobileFormatter.Deserialize(request.ClientContext),
          request.ClientCulture,
          request.ClientUICulture);

        var createResponse = await processor.Create(createRequest).ConfigureAwait(false);
        if (createResponse.Error != null)
        {
          result.ErrorData = new WcfErrorInfo(createResponse.Error);
        }
        result.GlobalContext = MobileFormatter.Serialize(createResponse.GlobalContext);
        result.ObjectData = MobileFormatter.Serialize(createResponse.Object);
      }
      catch (Exception ex)
      {
        result.ErrorData = new WcfErrorInfo(ex);
      }
      finally
      {
        MobileRequestProcessor.ClearContext();
      }
      return ConvertResponse(result);

    }

    /// <summary>
    /// Get an existing business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
    public async Task<WcfResponse> Fetch(CriteriaRequest request)
    {
      var result = new WcfResponse();
      try
      {
        request = ConvertRequest(request);
        // unpack criteria data into object
        object criteria = GetCriteria(request.CriteriaData);
        if (criteria is Csla.DataPortalClient.PrimitiveCriteria)
        {
          criteria = ((Csla.DataPortalClient.PrimitiveCriteria)criteria).Value;
        }
        var processor = new MobileRequestProcessor();
        var fetchRequest = new MobileCriteriaRequest(
          request.TypeName,
          criteria,
          (IPrincipal)MobileFormatter.Deserialize(request.Principal),
          (ContextDictionary)MobileFormatter.Deserialize(request.GlobalContext),
          (ContextDictionary)MobileFormatter.Deserialize(request.ClientContext),
          request.ClientCulture,
          request.ClientUICulture);

        var fetchResponse = await processor.Fetch(fetchRequest).ConfigureAwait(false);
        if (fetchResponse.Error != null)
        {
          result.ErrorData = new WcfErrorInfo(fetchResponse.Error);
        }
        result.GlobalContext = MobileFormatter.Serialize(fetchResponse.GlobalContext);
        result.ObjectData = MobileFormatter.Serialize(fetchResponse.Object);
      }
      catch (Exception ex)
      {
        result.ErrorData = new WcfErrorInfo(ex);
      }
      finally
      {
        MobileRequestProcessor.ClearContext();
      }
      return ConvertResponse(result);
    }

    /// <summary>
    /// Update a business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
    public async Task<WcfResponse> Update(UpdateRequest request)
    {
      var result = new WcfResponse();
      try
      {
        request = ConvertRequest(request);
        // unpack object
        object obj = GetCriteria(request.ObjectData);

        var processor = new MobileRequestProcessor();
        var updateRequest = new MobileUpdateRequest(
          obj,
          (IPrincipal)MobileFormatter.Deserialize(request.Principal),
          (ContextDictionary)MobileFormatter.Deserialize(request.GlobalContext),
          (ContextDictionary)MobileFormatter.Deserialize(request.ClientContext),
          request.ClientCulture,
          request.ClientUICulture);

        var updateResponse = await processor.Update(updateRequest).ConfigureAwait(false);
        if (updateResponse.Error != null)
        {
          result.ErrorData = new WcfErrorInfo(updateResponse.Error);
        }
        result.GlobalContext = MobileFormatter.Serialize(updateResponse.GlobalContext);
        result.ObjectData = MobileFormatter.Serialize(updateResponse.Object);
      }
      catch (Exception ex)
      {
        result.ErrorData = new WcfErrorInfo(ex);
      }
      finally
      {
        MobileRequestProcessor.ClearContext();
      }
      return ConvertResponse(result);
    }

    /// <summary>
    /// Delete a business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
    public async Task<WcfResponse> Delete(CriteriaRequest request)
    {
      var result = new WcfResponse();
      try
      {
        request = ConvertRequest(request);
        // unpack criteria data into object
        object criteria = GetCriteria(request.CriteriaData);
        if (criteria is Csla.DataPortalClient.PrimitiveCriteria)
        {
          criteria = ((Csla.DataPortalClient.PrimitiveCriteria)criteria).Value;
        }

        var processor = new MobileRequestProcessor();
        var deleteRequest = new MobileCriteriaRequest(
          request.TypeName,
          criteria,
          (IPrincipal)MobileFormatter.Deserialize(request.Principal),
          (ContextDictionary)MobileFormatter.Deserialize(request.GlobalContext),
          (ContextDictionary)MobileFormatter.Deserialize(request.ClientContext),
          request.ClientCulture,
          request.ClientUICulture);

        var deleteResponse = await processor.Delete(deleteRequest).ConfigureAwait(false);
        if (deleteResponse.Error != null)
        {
          result.ErrorData = new WcfErrorInfo(deleteResponse.Error);
        }
        result.GlobalContext = MobileFormatter.Serialize(deleteResponse.GlobalContext);
        result.ObjectData = MobileFormatter.Serialize(deleteResponse.Object);
      }
      catch (Exception ex)
      {
        result.ErrorData = new WcfErrorInfo(ex);
      }
      finally
      {
        MobileRequestProcessor.ClearContext();
      }
      return ConvertResponse(result);
    }

#endregion

#region Criteria

    private static object GetCriteria(byte[] criteriaData)
    {
      object criteria = null;
      if (criteriaData != null)
        criteria = MobileFormatter.Deserialize(criteriaData);
      return criteria;
    }

#endregion

#region Extention Method for Requests

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
    protected virtual WcfResponse ConvertResponse(WcfResponse response)
    {
      return response;
    }

#endregion
  }
}
#endif