Public Class Form1

  Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

    Dim svc As PTWcfService.PTServiceClient = New PTWcfClient.PTWcfService.PTServiceClient()
    Dim list As PTWcfService.ProjectData() = svc.GetProjectList()
    Me.ProjectDataBindingSource.DataSource = list
    svc.Close()

  End Sub

End Class
