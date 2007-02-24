Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.ServiceModel
Imports Csla.Server.Hosts.WcfChannel

Namespace Server.Hosts
  ''' <summary>
  ''' Defines the service contract for the WCF data
  ''' portal.
  ''' </summary>
  <ServiceContract(Namespace:="http://ws.lhotka.net/WcfDataPortal")> _
  Public Interface IWcfPortal
    ''' <summary>
    ''' Create a new business object.
    ''' </summary>
    ''' <param name="request">The request parameter object.</param>
    <OperationContract(), UseNetDataContract()> _
    Function Create(ByVal request As CreateRequest) As WcfResponse
    ''' <summary>
    ''' Get an existing business object.
    ''' </summary>
    ''' <param name="request">The request parameter object.</param>
    <OperationContract(), UseNetDataContract()> _
    Function Fetch(ByVal request As FetchRequest) As WcfResponse
    ''' <summary>
    ''' Update a business object.
    ''' </summary>
    ''' <param name="request">The request parameter object.</param>
    <OperationContract(), UseNetDataContract()> _
    Function Update(ByVal request As UpdateRequest) As WcfResponse
    ''' <summary>
    ''' Delete a business object.
    ''' </summary>
    ''' <param name="request">The request parameter object.</param>
    <OperationContract(), UseNetDataContract()> _
    Function Delete(ByVal request As DeleteRequest) As WcfResponse
  End Interface
End Namespace
