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
  <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
    Me.Button1 = New System.Windows.Forms.Button
    Me.SuspendLayout()
    '
    'Button1
    '
    Me.Button1.Location = New System.Drawing.Point(96, 72)
    Me.Button1.Name = "Button1"
    Me.Button1.TabIndex = 0
    Me.Button1.Text = "Button1"
    '
    'Form1
    '
    Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
    Me.ClientSize = New System.Drawing.Size(292, 266)
    Me.Controls.Add(Me.Button1)
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

End Class
