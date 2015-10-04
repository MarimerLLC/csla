#if !NETFX_CORE && !(ANDROID || IOS)
//-----------------------------------------------------------------------
// <copyright file="WcfPortal.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Exposes server-side DataPortal functionality</summary>
//-----------------------------------------------------------------------
using System;
using Csla.Server.Hosts.WcfChannel;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Threading.Tasks;

namespace Csla.Server.Hosts
{
#if NET40
  /// <summary>
  /// Exposes server-side DataPortal functionality
  /// through WCF.
  /// </summary>
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
  [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
  public class WcfPortal : IWcfPortal
  {
    /// <summary>
    /// Create a new business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
    public WcfResponse Create(CreateRequest request)
    {
      Csla.Server.DataPortal portal = new Csla.Server.DataPortal();
      object result;
      try
      {
        result = portal.Create(request.ObjectType, request.Criteria, request.Context, true).Result;
      }
      catch (Exception ex)
      {
        result = ex;
      }
      return new WcfResponse(result);
    }

    /// <summary>
    /// Get an existing business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
    public WcfResponse Fetch(FetchRequest request)
    {
      Csla.Server.DataPortal portal = new Csla.Server.DataPortal();
      object result;
      try
      {
        result = portal.Fetch(request.ObjectType, request.Criteria, request.Context, true).Result;
      }
      catch (Exception ex)
      {
        result = ex;
      }
      return new WcfResponse(result);
    }

    /// <summary>
    /// Update a business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
    public WcfResponse Update(UpdateRequest request)
    {
      Csla.Server.DataPortal portal = new Csla.Server.DataPortal();
      object result;
      try
      {
        result = portal.Update(request.Object, request.Context, true).Result;
      }
      catch (Exception ex)
      {
        result = ex;
      }
      return new WcfResponse(result);
    }

    /// <summary>
    /// Delete a business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
    public WcfResponse Delete(DeleteRequest request)
    {
      Csla.Server.DataPortal portal = new Csla.Server.DataPortal();
      object result;
      try
      {
        result = portal.Delete(request.ObjectType, request.Criteria, request.Context, true).Result;
      }
      catch (Exception ex)
      {
        result = ex;
      }
      return new WcfResponse(result);
    }
  }
#else
  /// <summary>
  /// Exposes server-side DataPortal functionality
  /// through WCF.
  /// </summary>
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
  [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
  public class WcfPortal : IWcfPortal
  {
    /// <summary>
    /// Create a new business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
    public async Task<WcfResponse> Create(CreateRequest request)
    {
      Csla.Server.DataPortal portal = new Csla.Server.DataPortal();
      object result;
      try
      {
        result = await portal.Create(request.ObjectType, request.Criteria, request.Context, true);
      }
      catch (Exception ex)
      {
        result = ex;
      }
      return new WcfResponse(result);
    }

    /// <summary>
    /// Get an existing business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
    public async Task<WcfResponse> Fetch(FetchRequest request)
    {
      Csla.Server.DataPortal portal = new Csla.Server.DataPortal();
      object result;
      try
      {
        result = await portal.Fetch(request.ObjectType, request.Criteria, request.Context, true);
      }
      catch (Exception ex)
      {
        result = ex;
      }
      return new WcfResponse(result);
    }

    /// <summary>
    /// Update a business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
    public async Task<WcfResponse> Update(UpdateRequest request)
    {
      Csla.Server.DataPortal portal = new Csla.Server.DataPortal();
      object result;
      try
      {
        result = await portal.Update(request.Object, request.Context, true);
      }
      catch (Exception ex)
      {
        result = ex;
      }
      return new WcfResponse(result);
    }

    /// <summary>
    /// Delete a business object.
    /// </summary>
    /// <param name="request">The request parameter object.</param>
    [OperationBehavior(Impersonation = ImpersonationOption.Allowed)]
    public async Task<WcfResponse> Delete(DeleteRequest request)
    {
      Csla.Server.DataPortal portal = new Csla.Server.DataPortal();
      object result;
      try
      {
        result = await portal.Delete(request.ObjectType, request.Criteria, request.Context, true);
      }
      catch (Exception ex)
      {
        result = ex;
      }
      return new WcfResponse(result);
    }
  }
#endif
}
#endif