Public Class Form1
    Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

    Public Sub New()
        MyBase.New()

        'This call is required by the Windows Form Designer.
        InitializeComponent()

        'Add any initialization after the InitializeComponent() call

    End Sub

    'Form overrides dispose to clean up the component list.
    Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing Then
            If Not (components Is Nothing) Then
                components.Dispose()
            End If
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
  Friend WithEvents Button1 As System.Windows.Forms.Button
  Friend WithEvents Button2 As System.Windows.Forms.Button
  Friend WithEvents Button3 As System.Windows.Forms.Button
  Friend WithEvents Button4 As System.Windows.Forms.Button
  Friend WithEvents Button5 As System.Windows.Forms.Button
  Friend WithEvents Button6 As System.Windows.Forms.Button
  Friend WithEvents Button7 As System.Windows.Forms.Button
  <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
    Me.Button1 = New System.Windows.Forms.Button()
    Me.Button2 = New System.Windows.Forms.Button()
    Me.Button3 = New System.Windows.Forms.Button()
    Me.Button4 = New System.Windows.Forms.Button()
    Me.Button5 = New System.Windows.Forms.Button()
    Me.Button6 = New System.Windows.Forms.Button()
    Me.Button7 = New System.Windows.Forms.Button()
    Me.SuspendLayout()
    '
    'Button1
    '
    Me.Button1.Location = New System.Drawing.Point(32, 72)
    Me.Button1.Name = "Button1"
    Me.Button1.TabIndex = 0
    Me.Button1.Text = "Button1"
    '
    'Button2
    '
    Me.Button2.Location = New System.Drawing.Point(32, 112)
    Me.Button2.Name = "Button2"
    Me.Button2.TabIndex = 1
    Me.Button2.Text = "Button2"
    '
    'Button3
    '
    Me.Button3.Location = New System.Drawing.Point(32, 152)
    Me.Button3.Name = "Button3"
    Me.Button3.TabIndex = 2
    Me.Button3.Text = "Button3"
    '
    'Button4
    '
    Me.Button4.Location = New System.Drawing.Point(136, 72)
    Me.Button4.Name = "Button4"
    Me.Button4.TabIndex = 3
    Me.Button4.Text = "Button4"
    '
    'Button5
    '
    Me.Button5.Location = New System.Drawing.Point(136, 112)
    Me.Button5.Name = "Button5"
    Me.Button5.TabIndex = 4
    Me.Button5.Text = "Button5"
    '
    'Button6
    '
    Me.Button6.Location = New System.Drawing.Point(136, 152)
    Me.Button6.Name = "Button6"
    Me.Button6.TabIndex = 5
    Me.Button6.Text = "Button6"
    '
    'Button7
    '
    Me.Button7.Location = New System.Drawing.Point(96, 200)
    Me.Button7.Name = "Button7"
    Me.Button7.TabIndex = 6
    Me.Button7.Text = "Button7"
    '
    'Form1
    '
    Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
    Me.ClientSize = New System.Drawing.Size(292, 266)
    Me.Controls.AddRange(New System.Windows.Forms.Control() {Me.Button7, Me.Button6, Me.Button5, Me.Button4, Me.Button3, Me.Button2, Me.Button1})
    Me.Name = "Form1"
    Me.Text = "Form1"
    Me.ResumeLayout(False)

  End Sub

#End Region

  Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click

    Dim proj() As PTService.ProjectInfo
    proj = WebService.GetProjectList
    MsgBox(UBound(proj))

  End Sub

#Region " Security "

  Private mService As PTService.ProjectTracker

  Private Function WebService() As PTService.ProjectTracker

    If mService Is Nothing Then

      ' Create the web service proxy
      mService = New PTService.ProjectTracker

      ' Create the security credentials
      Dim cred As New PTService.CSLACredentials
      cred.Username = "rocky"
      cred.Password = "lhotka"

      ' Provide the credentials to the service proxy
      mService.CSLACredentialsValue = cred

      ' Return the service proxy for use
      Return mService
    Else

      ' We had a cached proxy - return it
      Return mService
    End If

  End Function

#End Region

  Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click

    Dim proj As PTService.ProjectInfo

    proj = WebService.GetProject("AF53CD7A-B1B0-4A10-8B9E-1BC3697BEB8F")
    MsgBox(proj.Name)
    MsgBox(proj.Resources.Length)

  End Sub

  Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click

    Dim proj As New PTService.ProjectInfo()

    With proj
      .Name = "WS Test"
      .Description = "Added with a web service"
      .Started = Now
    End With

    Dim newID As String = WebService.UpdateProject(proj)

    MsgBox("Added: " & newID)

    proj = WebService.GetProject(newID)
    MsgBox("Got: " & proj.Name)

    proj.Name = "Updated by WS"

    WebService.UpdateProject(proj)

    proj = WebService.GetProject(newID)
    MsgBox("Got updated: " & proj.Name)


  End Sub

  Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click

    Dim res As PTService.ResourceInfo

    res = WebService.GetResource("810")

    MsgBox(res.Name)

  End Sub

  Private Sub Button5_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button5.Click

    Dim res As New PTService.ResourceInfo()

    res.Name = "Johnson,Reginald"
    res.ID = "951"

    WebService.UpdateResource(res)

    res = WebService.GetResource(res.ID)

    MsgBox(res.Name)

  End Sub

  Private Sub Button6_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button6.Click

    WebService.DeleteResource("951")

  End Sub

  Private Sub Button7_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button7.Click

    Dim proj As New PTService.ProjectInfo()

    With proj
      .Name = "WS Test"
      .Description = "Added with a web service"
      .Started = Now
    End With

    Dim newID As String = WebService.UpdateProject(proj)

    Dim res As New PTService.ResourceInfo()

    res.Name = "Johnson,Reginald"
    res.ID = "951"

    WebService.UpdateResource(res)

    proj = WebService.GetProject(newID)

    ReDim proj.Resources(0)
    proj.Resources(0) = New PTService.ProjectResourceInfo()
    With proj.Resources(0)
      .ResourceID = res.ID
      .Assigned = Now
      .Role = "Project Manager"
    End With

    WebService.UpdateProject(proj)

  End Sub

End Class
