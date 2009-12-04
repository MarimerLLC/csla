Imports System.ServiceModel

<ServiceContract()> _
Public Interface IPTService

  <OperationContract()> _
  Function GetProjectList() As ProjectData()

End Interface
