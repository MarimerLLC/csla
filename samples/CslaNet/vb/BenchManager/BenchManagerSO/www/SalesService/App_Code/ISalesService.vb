Imports Microsoft.VisualBasic
Imports System.ServiceModel

<ServiceContract()> _
Public Interface ISalesService
  <OperationContract()> _
  Function Test() As String
  <OperationContract()> _
  Function GetClientList() As ClientData()
  <OperationContract()> _
  Sub UpdateClients(ByVal clientList As ClientDataUpdate())
  <OperationContract()> _
  Sub DeleteClient(ByVal clientId As Integer)

  <OperationContract()> _
  Function GetProjectList(ByVal clientId As Integer) As ProjectData()
  <OperationContract()> _
  Function GetProject(ByVal projectId As Integer) As ProjectData
  <OperationContract()> _
  Function GetFullProjectList() As ProjectSummaryData()
  <OperationContract()> _
  Sub UpdateProjects(ByVal projectList As ProjectDataUpdate())
  <OperationContract()> _
  Sub DeleteProject(ByVal projectId As Integer)
End Interface
