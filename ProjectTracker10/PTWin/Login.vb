Public Class Login
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
  Friend WithEvents Label1 As System.Windows.Forms.Label
  Friend WithEvents txtUsername As System.Windows.Forms.TextBox
  Friend WithEvents Label2 As System.Windows.Forms.Label
  Friend WithEvents txtPassword As System.Windows.Forms.TextBox
  Friend WithEvents btnLogin As System.Windows.Forms.Button
  Friend WithEvents btnCancel As System.Windows.Forms.Button
  Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
  <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
    Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(Login))
    Me.Label1 = New System.Windows.Forms.Label()
    Me.txtUsername = New System.Windows.Forms.TextBox()
    Me.Label2 = New System.Windows.Forms.Label()
    Me.txtPassword = New System.Windows.Forms.TextBox()
    Me.btnLogin = New System.Windows.Forms.Button()
    Me.btnCancel = New System.Windows.Forms.Button()
    Me.PictureBox1 = New System.Windows.Forms.PictureBox()
    Me.SuspendLayout()
    '
    'Label1
    '
    Me.Label1.Location = New System.Drawing.Point(280, 40)
    Me.Label1.Name = "Label1"
    Me.Label1.TabIndex = 0
    Me.Label1.Text = "Username"
    '
    'txtUsername
    '
    Me.txtUsername.Anchor = ((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right)
    Me.txtUsername.Location = New System.Drawing.Point(352, 40)
    Me.txtUsername.Name = "txtUsername"
    Me.txtUsername.Size = New System.Drawing.Size(168, 20)
    Me.txtUsername.TabIndex = 1
    Me.txtUsername.Text = ""
    '
    'Label2
    '
    Me.Label2.Location = New System.Drawing.Point(280, 72)
    Me.Label2.Name = "Label2"
    Me.Label2.TabIndex = 2
    Me.Label2.Text = "Password"
    '
    'txtPassword
    '
    Me.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
                Or System.Windows.Forms.AnchorStyles.Right)
    Me.txtPassword.Location = New System.Drawing.Point(352, 72)
    Me.txtPassword.Name = "txtPassword"
    Me.txtPassword.PasswordChar = Microsoft.VisualBasic.ChrW(42)
    Me.txtPassword.Size = New System.Drawing.Size(168, 20)
    Me.txtPassword.TabIndex = 3
    Me.txtPassword.Text = ""
    '
    'btnLogin
    '
    Me.btnLogin.Anchor = (System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right)
    Me.btnLogin.Enabled = False
    Me.btnLogin.Location = New System.Drawing.Point(352, 112)
    Me.btnLogin.Name = "btnLogin"
    Me.btnLogin.TabIndex = 4
    Me.btnLogin.Text = "Login"
    '
    'btnCancel
    '
    Me.btnCancel.Anchor = (System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right)
    Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
    Me.btnCancel.Location = New System.Drawing.Point(440, 112)
    Me.btnCancel.Name = "btnCancel"
    Me.btnCancel.TabIndex = 5
    Me.btnCancel.Text = "Cancel"
    '
    'PictureBox1
    '
    Me.PictureBox1.Dock = System.Windows.Forms.DockStyle.Left
    Me.PictureBox1.Image = CType(resources.GetObject("PictureBox1.Image"), System.Drawing.Bitmap)
    Me.PictureBox1.Name = "PictureBox1"
    Me.PictureBox1.Size = New System.Drawing.Size(264, 176)
    Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage
    Me.PictureBox1.TabIndex = 6
    Me.PictureBox1.TabStop = False
    '
    'Login
    '
    Me.AcceptButton = Me.btnLogin
    Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
    Me.CancelButton = Me.btnCancel
    Me.ClientSize = New System.Drawing.Size(538, 176)
    Me.ControlBox = False
    Me.Controls.AddRange(New System.Windows.Forms.Control() {Me.PictureBox1, Me.btnCancel, Me.btnLogin, Me.txtPassword, Me.txtUsername, Me.Label2, Me.Label1})
    Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
    Me.MaximizeBox = False
    Me.MinimizeBox = False
    Me.Name = "Login"
    Me.ShowInTaskbar = False
    Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
    Me.Text = "Login"
    Me.ResumeLayout(False)

  End Sub

#End Region

  Private mUsername As String
  Private mPassword As String
  Private mLogin As Boolean

  Private Sub btnLogin_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnLogin.Click
    mUsername = txtUsername.Text
    mPassword = txtPassword.Text
    mLogin = True
    Hide()
  End Sub

  Private Sub btnCancel_Click(ByVal sender As System.Object, _
      ByVal e As System.EventArgs) Handles btnCancel.Click

    mUsername = ""
    mPassword = ""
    mLogin = False
    Hide()

  End Sub

  Public ReadOnly Property Username() As String
    Get
      Return mUsername
    End Get
  End Property

  Public ReadOnly Property Password() As String
    Get
      Return mPassword
    End Get
  End Property

  Public ReadOnly Property Login() As Boolean
    Get
      Return mLogin
    End Get
  End Property

  Private Sub txtUsername_TextChanged(ByVal sender As System.Object, _
      ByVal e As System.EventArgs) Handles txtUsername.TextChanged

    btnLogin.Enabled = (Len(txtUsername.Text) > 0)

  End Sub

End Class
