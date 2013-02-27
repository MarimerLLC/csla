using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Csla.Server.Hosts.WcfChannel;

namespace ExtendableWcfPortalForDotNet
{
  [ServiceContract(Namespace = "http://ws.lhotka.net/ExtendableWcfPortalForDotNet")]
  public interface IExtendableWcfPortalForDotNet
  {
    [OperationContract]
    [UseNetDataContract]
    WcfResponse Create(CriteriaRequest request);

    [OperationContract]
    [UseNetDataContract]
    WcfResponse Fetch(CriteriaRequest request);

    [OperationContract]
    [UseNetDataContract]
    WcfResponse Update(UpdateRequest request);

    [OperationContract]
    [UseNetDataContract]
    WcfResponse Delete(CriteriaRequest request);
  }
}
