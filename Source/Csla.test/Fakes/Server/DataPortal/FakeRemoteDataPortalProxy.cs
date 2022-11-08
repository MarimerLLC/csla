using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csla.Channels.Local;
using Csla.DataPortalClient;
using Csla.Server;

namespace Csla.Test.Fakes.Server.DataPortal
{
  /// <summary>
  /// Fake proxy implementation that can be used to represent a remote proxy
  /// for testing, without the need to step outside of the AppDomain
  /// </summary>
  internal class FakeRemoteDataPortalProxy : IDataPortalProxy
  {
    private readonly LocalProxy _implementingProxy;

    public FakeRemoteDataPortalProxy(LocalProxy implementingProxy)
    {
      _implementingProxy = implementingProxy;
    }

    public bool IsServerRemote { get => true; }

    public Task<DataPortalResult> Create(Type objectType, object criteria, DataPortalContext context, bool isSync) 
      => _implementingProxy.Create(objectType, criteria, context, isSync);
    
    public Task<DataPortalResult> Delete(Type objectType, object criteria, DataPortalContext context, bool isSync)
      => _implementingProxy.Delete(objectType, criteria, context, isSync);

    public Task<DataPortalResult> Fetch(Type objectType, object criteria, DataPortalContext context, bool isSync)
      => _implementingProxy.Fetch(objectType, criteria, context, isSync);

    public Task<DataPortalResult> Update(object obj, DataPortalContext context, bool isSync)
      => _implementingProxy.Update(obj, context, isSync);
  }
}
