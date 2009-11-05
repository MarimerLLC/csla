Imports System.ServiceModel

<ServiceContract()> _
Public Interface IBenchSvc

  <OperationContract()> _
  Function GetConsultantList(ByVal benchOnly As Boolean) As ConsultantData()
  <OperationContract()> _
  Function GetConsultant(ByVal id As Integer) As ConsultantData
  <OperationContract()> _
  Function UpdateConsultant(ByVal consultant As ConsultantUpdateData) As ConsultantData
  <OperationContract()> _
  Function GetProjectList(ByVal consultantId As Integer) As ProjectData()
  <OperationContract()> _
  Sub UpdateProjects(ByVal consultantId As Integer, ByVal projects As ProjectUpdateData())

End Interface
