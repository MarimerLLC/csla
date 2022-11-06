using Csla.Channels.Local;
using Csla.DataPortalClient;
using Csla.Test.Fakes.Server.DataPortal;
using Microsoft.Extensions.DependencyInjection;

namespace Csla.Configuration
{
  /// <summary>
  /// Extension methods for the DataPortalClientOptions to make FakeRemoteDataPortalProxy easier to use
  /// </summary>
  internal static class FakeDataPortalProxyExtensions
  {
    /// <summary>
    /// Configure data portal client to use fake remote data portal proxy.
    /// </summary>
    /// <param name="config">CslaDataPortalConfiguration object being extended</param>
    public static DataPortalClientOptions UseFakeRemoteDataPortalProxy(this DataPortalClientOptions config)
    {
      config.Services.AddTransient(typeof(IDataPortalProxy),
        sp =>
        {
          var implementingProxy = sp.GetRequiredService<LocalProxy>();
          return new FakeRemoteDataPortalProxy(implementingProxy);
        });
      return config;
    }
  }
}
