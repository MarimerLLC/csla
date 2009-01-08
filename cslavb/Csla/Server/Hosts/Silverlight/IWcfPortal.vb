Imports System
Imports System.ServiceModel

Namespace Server.Hosts.Silverlight

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
    <OperationContract()> _
    Function Create(ByVal request As CriteriaRequest) As WcfResponse
    ''' <summary>
    ''' Get an existing business object.
    ''' </summary>
    ''' <param name="request">The request parameter object.</param>
    <OperationContract()> _
    Function Fetch(ByVal request As CriteriaRequest) As WcfResponse
    ''' <summary>
    ''' Update a business object.
    ''' </summary>
    ''' <param name="request">The request parameter object.</param>
    <OperationContract()> _
    Function Update(ByVal request As UpdateRequest) As WcfResponse
    ''' <summary>
    ''' Delete a business object.
    ''' </summary>
    ''' <param name="request">The request parameter object.</param>
    <OperationContract()> _
    Function Delete(ByVal request As CriteriaRequest) As WcfResponse

  End Interface
End Namespace

