#if !NETSTANDARD2_0 && !NET5_0
#pragma warning disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csla.Server.Hosts.WcfChannel;
using System.ServiceModel;

namespace Csla.DataPortalClient
{
  [System.ServiceModel.ServiceContractAttribute(Namespace = "http://ws.lhotka.net/WcfDataPortal", ConfigurationName = "WcfPortal.IWcfPortal")]
  public interface IWcfPortal
  {
    [System.ServiceModel.OperationContractAttribute(Action = "http://ws.lhotka.net/WcfDataPortal/IWcfPortal/Create", ReplyAction = "http://ws.lhotka.net/WcfDataPortal/IWcfPortal/CreateResponse")]
    [UseNetDataContract]
    WcfResponse Create(CreateRequest request);

    [System.ServiceModel.OperationContractAttribute(Action = "http://ws.lhotka.net/WcfDataPortal/IWcfPortal/Create", ReplyAction = "http://ws.lhotka.net/WcfDataPortal/IWcfPortal/CreateResponse")]
    System.Threading.Tasks.Task<WcfResponse> CreateAsync(CreateRequest request);

    [System.ServiceModel.OperationContractAttribute(Action = "http://ws.lhotka.net/WcfDataPortal/IWcfPortal/Fetch", ReplyAction = "http://ws.lhotka.net/WcfDataPortal/IWcfPortal/FetchResponse")]
    [UseNetDataContract]
    WcfResponse Fetch(FetchRequest request);

    [System.ServiceModel.OperationContractAttribute(Action = "http://ws.lhotka.net/WcfDataPortal/IWcfPortal/Fetch", ReplyAction = "http://ws.lhotka.net/WcfDataPortal/IWcfPortal/FetchResponse")]
    System.Threading.Tasks.Task<WcfResponse> FetchAsync(FetchRequest request);

    [System.ServiceModel.OperationContractAttribute(Action = "http://ws.lhotka.net/WcfDataPortal/IWcfPortal/Update", ReplyAction = "http://ws.lhotka.net/WcfDataPortal/IWcfPortal/UpdateResponse")]
    [UseNetDataContract]
    WcfResponse Update(UpdateRequest request);

    [System.ServiceModel.OperationContractAttribute(Action = "http://ws.lhotka.net/WcfDataPortal/IWcfPortal/Update", ReplyAction = "http://ws.lhotka.net/WcfDataPortal/IWcfPortal/UpdateResponse")]
    System.Threading.Tasks.Task<WcfResponse> UpdateAsync(UpdateRequest request);

    [System.ServiceModel.OperationContractAttribute(Action = "http://ws.lhotka.net/WcfDataPortal/IWcfPortal/Delete", ReplyAction = "http://ws.lhotka.net/WcfDataPortal/IWcfPortal/DeleteResponse")]
    [UseNetDataContract]
    WcfResponse Delete(DeleteRequest request);

    [System.ServiceModel.OperationContractAttribute(Action = "http://ws.lhotka.net/WcfDataPortal/IWcfPortal/Delete", ReplyAction = "http://ws.lhotka.net/WcfDataPortal/IWcfPortal/DeleteResponse")]
    System.Threading.Tasks.Task<WcfResponse> DeleteAsync(DeleteRequest request);
  }
}
#endif