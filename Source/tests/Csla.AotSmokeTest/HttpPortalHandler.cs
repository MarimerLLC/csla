//-----------------------------------------------------------------------
// <copyright file="HttpPortalHandler.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Hands HttpProxy requests to an in-process HttpPortal</summary>
//-----------------------------------------------------------------------

using System.Net;
using Csla.Serialization;
using Csla.Server.Hosts;
using Csla.Server.Hosts.DataPortalChannel;
using Microsoft.Extensions.DependencyInjection;

namespace Csla.AotSmokeTest
{
  /// <summary>
  /// Hands HttpProxy requests to an HttpPortal in the same process, the way
  /// HttpPortalController does in an ASP.NET Core data portal host. The portal
  /// resolves the business type names the client sends.
  /// </summary>
  internal sealed class HttpPortalHandler(IServiceProvider serverServices) : HttpMessageHandler
  {
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
      var operation = request.RequestUri!.Query.TrimStart('?')
        .Split('&')
        .Select(p => p.Split('=', 2))
        .Single(p => p[0] == "operation")[1];
      var body = await request.Content!.ReadAsByteArrayAsync(cancellationToken);

      using var scope = serverServices.CreateScope();
      var applicationContext = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
      var formatter = applicationContext.GetRequiredService<ISerializationFormatter>();
      var portal = applicationContext.CreateInstanceDI<HttpPortal>();
      var portalRequest = formatter.Deserialize(body)!;
      var response = operation switch
      {
        "create" => await portal.Create((CriteriaRequest)portalRequest),
        "fetch" => await portal.Fetch((CriteriaRequest)portalRequest),
        "update" => await portal.Update((UpdateRequest)portalRequest),
        "delete" => await portal.Delete((CriteriaRequest)portalRequest),
        _ => throw new InvalidOperationException(operation),
      };

      return new HttpResponseMessage(HttpStatusCode.OK)
      {
        Content = new ByteArrayContent(formatter.Serialize(response))
      };
    }
  }
}
