Imports System.ServiceModel

Public Class Form1

  Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    Dim svc As PTWcfService.PTServiceClient = New PTWcfClient.PTWcfService.PTServiceClient()
    Dim list As PTWcfService.ProjectData()
    Try
      list = svc.GetProjectList()
    Finally
      svc.Close()
    End Try
    Me.ProjectDataBindingSource.DataSource = list

    'Dim list As PTWcfService.ProjectData()
    'Dim svc As PTWcfService.PTServiceClient = New PTWcfClient.PTWcfService.PTServiceClient()
    'Try
    '  svc.ClientCredentials.UserName.UserName = "pm"
    '  svc.ClientCredentials.UserName.Password = "pm"
    '  list = svc.GetProjectList()
    'Finally
    '  svc.Close()
    'End Try
    'Me.ProjectDataBindingSource.DataSource = list

    'Dim list As PTWcfService.ProjectData()
    'Dim factory As New ChannelFactory(Of PTWcfService.IPTService)("WSHttpBinding_IPTService")
    'Try
    '  factory.Credentials.UserName.UserName = "pm"
    '  factory.Credentials.UserName.Password = "pm"
    '  Dim proxy As PTWcfService.IPTService = factory.CreateChannel
    '  list = proxy.GetProjectList()
    'Finally
    '  factory.Close()
    'End Try
    'Me.ProjectDataBindingSource.DataSource = list

  End Sub

End Class
