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

namespace Csla.Server.Hosts
{
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
}