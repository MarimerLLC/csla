Imports System.Configuration
Imports System.Security.Principal
Imports CSLA.Security
Imports System.Threading
Imports CSLA.BatchQueue

Public Class Main
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
  Friend WithEvents MainMenu1 As System.Windows.Forms.MainMenu
  Friend WithEvents MenuItem1 As System.Windows.Forms.MenuItem
  Friend WithEvents mnuFileLogin As System.Windows.Forms.MenuItem
  Friend WithEvents MenuItem3 As System.Windows.Forms.MenuItem
  Friend WithEvents mnuFileExit As System.Windows.Forms.MenuItem
  Friend WithEvents mnuAction As System.Windows.Forms.MenuItem
  Friend WithEvents MenuItem6 As System.Windows.Forms.MenuItem
  Friend WithEvents mnuProjectNew As System.Windows.Forms.MenuItem
  Friend WithEvents mnuProjectEdit As System.Windows.Forms.MenuItem
  Friend WithEvents mnuProjectRemove As System.Windows.Forms.MenuItem
  Friend WithEvents MenuItem10 As System.Windows.Forms.MenuItem
  Friend WithEvents mnuResourceNew As System.Windows.Forms.MenuItem
  Friend WithEvents mnuResourceEdit As System.Windows.Forms.MenuItem
  Friend WithEvents mnuResourceRemove As System.Windows.Forms.MenuItem
  Friend WithEvents StatusBar1 As System.Windows.Forms.StatusBar
  Friend WithEvents pnlStatus As System.Windows.Forms.StatusBarPanel
  Friend WithEvents pnlUser As System.Windows.Forms.StatusBarPanel
  Friend WithEvents mnuProjectList As System.Windows.Forms.MenuItem
  Friend WithEvents mnuReport As System.Windows.Forms.MenuItem
  Friend WithEvents mnuBatch As System.Windows.Forms.MenuItem
  Friend WithEvents mnuProjectUpdate As System.Windows.Forms.MenuItem
  <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
    Me.MainMenu1 = New System.Windows.Forms.MainMenu
    Me.MenuItem1 = New System.Windows.Forms.MenuItem
    Me.mnuFileLogin = New System.Windows.Forms.MenuItem
    Me.MenuItem3 = New System.Windows.Forms.MenuItem
    Me.mnuFileExit = New System.Windows.Forms.MenuItem
    Me.mnuAction = New System.Windows.Forms.MenuItem
    Me.MenuItem6 = New System.Windows.Forms.MenuItem
    Me.mnuProjectNew = New System.Windows.Forms.MenuItem
    Me.mnuProjectEdit = New System.Windows.Forms.MenuItem
    Me.mnuProjectRemove = New System.Windows.Forms.MenuItem
    Me.MenuItem10 = New System.Windows.Forms.MenuItem
    Me.mnuResourceNew = New System.Windows.Forms.MenuItem
    Me.mnuResourceEdit = New System.Windows.Forms.MenuItem
    Me.mnuResourceRemove = New System.Windows.Forms.MenuItem
    Me.mnuReport = New System.Windows.Forms.MenuItem
    Me.mnuProjectList = New System.Windows.Forms.MenuItem
    Me.mnuBatch = New System.Windows.Forms.MenuItem
    Me.mnuProjectUpdate = New System.Windows.Forms.MenuItem
    Me.StatusBar1 = New System.Windows.Forms.StatusBar
    Me.pnlStatus = New System.Windows.Forms.StatusBarPanel
    Me.pnlUser = New System.Windows.Forms.StatusBarPanel
    CType(Me.pnlStatus, System.ComponentModel.ISupportInitialize).BeginInit()
    CType(Me.pnlUser, System.ComponentModel.ISupportInitialize).BeginInit()
    Me.SuspendLayout()
    '
    'MainMenu1
    '
    Me.MainMenu1.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.MenuItem1, Me.mnuAction, Me.mnuReport, Me.mnuBatch})
    '
    'MenuItem1
    '
    Me.MenuItem1.Index = 0
    Me.MenuItem1.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuFileLogin, Me.MenuItem3, Me.mnuFileExit})
    Me.MenuItem1.Text = "&File"
    '
    'mnuFileLogin
    '
    Me.mnuFileLogin.Index = 0
    Me.mnuFileLogin.Text = "&Login"
    '
    'MenuItem3
    '
    Me.MenuItem3.Index = 1
    Me.MenuItem3.Text = "-"
    '
    'mnuFileExit
    '
    Me.mnuFileExit.Index = 2
    Me.mnuFileExit.Text = "E&xit"
    '
    'mnuAction
    '
    Me.mnuAction.Enabled = False
    Me.mnuAction.Index = 1
    Me.mnuAction.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.MenuItem6, Me.MenuItem10})
    Me.mnuAction.Text = "&Action"
    '
    'MenuItem6
    '
    Me.MenuItem6.Index = 0
    Me.MenuItem6.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuProjectNew, Me.mnuProjectEdit, Me.mnuProjectRemove})
    Me.MenuItem6.Text = "&Project"
    '
    'mnuProjectNew
    '
    Me.mnuProjectNew.Index = 0
    Me.mnuProjectNew.Text = "&New"
    '
    'mnuProjectEdit
    '
    Me.mnuProjectEdit.Index = 1
    Me.mnuProjectEdit.Text = "&Edit"
    '
    'mnuProjectRemove
    '
    Me.mnuProjectRemove.Index = 2
    Me.mnuProjectRemove.Text = "&Remove"
    '
    'MenuItem10
    '
    Me.MenuItem10.Index = 1
    Me.MenuItem10.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuResourceNew, Me.mnuResourceEdit, Me.mnuResourceRemove})
    Me.MenuItem10.Text = "&Resource"
    '
    'mnuResourceNew
    '
    Me.mnuResourceNew.Index = 0
    Me.mnuResourceNew.Text = "&New"
    '
    'mnuResourceEdit
    '
    Me.mnuResourceEdit.Index = 1
    Me.mnuResourceEdit.Text = "&Edit"
    '
    'mnuResourceRemove
    '
    Me.mnuResourceRemove.Index = 2
    Me.mnuResourceRemove.Text = "&Remove"
    '
    'mnuReport
    '
    Me.mnuReport.Enabled = False
    Me.mnuReport.Index = 2
    Me.mnuReport.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuProjectList})
    Me.mnuReport.Text = "&Report"
    '
    'mnuProjectList
    '
    Me.mnuProjectList.Index = 0
    Me.mnuProjectList.Text = "&Project list"
    '
    'mnuBatch
    '
    Me.mnuBatch.Enabled = False
    Me.mnuBatch.Index = 3
    Me.mnuBatch.MenuItems.AddRange(New System.Windows.Forms.MenuItem() {Me.mnuProjectUpdate})
    Me.mnuBatch.Text = "&Batch"
    '
    'mnuProjectUpdate
    '
    Me.mnuProjectUpdate.Index = 0
    Me.mnuProjectUpdate.Text = "&Project update"
    '
    'StatusBar1
    '
    Me.StatusBar1.Location = New System.Drawing.Point(0, 384)
    Me.StatusBar1.Name = "StatusBar1"
    Me.StatusBar1.Panels.AddRange(New System.Windows.Forms.StatusBarPanel() {Me.pnlStatus, Me.pnlUser})
    Me.StatusBar1.ShowPanels = True
    Me.StatusBar1.Size = New System.Drawing.Size(720, 22)
    Me.StatusBar1.TabIndex = 0
    Me.StatusBar1.Text = "StatusBar1"
    '
    'pnlStatus
    '
    Me.pnlStatus.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring
    Me.pnlStatus.Width = 604
    '
    'Main
    '
    Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
    Me.ClientSize = New System.Drawing.Size(720, 406)
    Me.Controls.Add(Me.StatusBar1)
    Me.IsMdiContainer = True
    Me.Menu = Me.MainMenu1
    Me.Name = "Main"
    Me.Text = "Project Tracker"
    Me.WindowState = System.Windows.Forms.FormWindowState.Maximized
    CType(Me.pnlStatus, System.ComponentModel.ISupportInitialize).EndInit()
    CType(Me.pnlUser, System.ComponentModel.ISupportInitialize).EndInit()
    Me.ResumeLayout(False)

  End Sub

#End Region

#Region " Load and Exit "

  Private Sub mnuFileExit_Click(ByVal sender As System.Object, _
      ByVal e As System.EventArgs) Handles mnuFileExit.Click

    Close()

  End Sub

  Private Sub Main_Load(ByVal sender As System.Object, _
      ByVal e As System.EventArgs) Handles MyBase.Load

    mMain = Me

    If ConfigurationSettings.AppSettings("Authentication") = "Windows" Then
      mnuFileLogin.Visible = False
      AppDomain.CurrentDomain.SetPrincipalPolicy( _
                  PrincipalPolicy.WindowsPrincipal)
      EnableMenus()

    Else
      DoLogin()
    End If

  End Sub

#End Region

#Region " Login/Logout/Authorization "

  Private Sub mnuFileLogin_Click(ByVal sender As System.Object, _
      ByVal e As System.EventArgs) Handles mnuFileLogin.Click

    DoLogin()

  End Sub

  Private Sub DoLogin()

    Dim dlg As New Login

    dlg.ShowDialog(Me)
    If dlg.Login Then
      Cursor = Cursors.WaitCursor
      Status("Verifying user...")
      BusinessPrincipal.Login(dlg.Username, dlg.Password)
      Status("")
      Cursor = Cursors.Default

      If Thread.CurrentPrincipal.Identity.IsAuthenticated Then
        pnlUser.Text = Thread.CurrentPrincipal.Identity.Name
        EnableMenus()

      Else
        DoLogout()
        MsgBox("The username and password were not valid", MsgBoxStyle.Exclamation)
      End If

    Else
      DoLogout()
    End If

  End Sub

  Private Sub DoLogout()

    ' reset to an unauthorized principal
    Thread.CurrentPrincipal = New GenericPrincipal(New GenericIdentity(""), New Object())
    pnlUser.Text = ""
    mnuAction.Enabled = False
    mnuReport.Enabled = False
    mnuBatch.Enabled = False

  End Sub

  Private Sub EnableMenus()
    Dim user As Security.Principal.IPrincipal
    user = Thread.CurrentPrincipal

    mnuAction.Enabled = True
    mnuReport.Enabled = True
    mnuBatch.Enabled = True

    mnuProjectNew.Enabled = _
      user.IsInRole("ProjectManager")

    mnuProjectRemove.Enabled = _
      user.IsInRole("ProjectManager") OrElse _
      user.IsInRole("Administrator")

    mnuResourceNew.Enabled = _
      user.IsInRole("ProjectManager") OrElse _
      user.IsInRole("Supervisor")

    mnuResourceRemove.Enabled = _
      user.IsInRole("ProjectManager") OrElse _
      user.IsInRole("Supervisor") OrElse _
      user.IsInRole("Administrator")

  End Sub

#End Region

#Region " Status "

  Private Shared mMain As Main

  Public Shared Sub Status(ByVal Text As String)

    mMain.pnlStatus.Text = Text

  End Sub


#End Region

#Region " Projects "

  Private Sub mnuProjectEdit_Click(ByVal sender As System.Object, _
      ByVal e As System.EventArgs) Handles mnuProjectEdit.Click

    Dim dlg As New ProjectSelect
    dlg.Text = "Edit Project"
    dlg.ShowDialog(Me)

    Dim Result As String = dlg.Result
    If Len(Result) > 0 Then
      Try
        Cursor.Current = Cursors.WaitCursor
        Dim ID As Guid = New Guid(Result)
        Dim obj As Project = Project.GetProject(ID)

        Dim frm As New ProjectEdit
        frm.MdiParent = Me
        frm.Project = obj
        Cursor.Current = Cursors.Default
        frm.Show()

      Catch ex As Exception
        Cursor.Current = Cursors.Default
        MsgBox("Error loading project " & vbCrLf & ex.ToString)
      End Try
    End If

  End Sub

  Private Sub mnuProjectRemove_Click(ByVal sender As System.Object, _
      ByVal e As System.EventArgs) Handles mnuProjectRemove.Click

    Dim dlg As New ProjectSelect
    dlg.Text = "Remove Project"
    dlg.ShowDialog(Me)

    Dim Result As String = dlg.Result
    If Len(Result) > 0 Then
      If MsgBox("Remove project " & Result, _
          MsgBoxStyle.YesNo, "Remove project") = MsgBoxResult.Yes Then
        Try
          Cursor.Current = Cursors.WaitCursor
          pnlStatus.Text = "Deleting project..."

          Dim ID As Guid = New Guid(Result)
          Project.DeleteProject(ID)

          Cursor.Current = Cursors.Default
          MsgBox("Project deleted")

        Catch ex As Exception
          Cursor.Current = Cursors.Default
          MsgBox("Error deleting project" & vbCrLf & ex.ToString)

        Finally
          pnlStatus.Text = ""
        End Try
      End If
    End If

  End Sub

  Private Sub mnuProjectNew_Click(ByVal sender As System.Object, _
      ByVal e As System.EventArgs) Handles mnuProjectNew.Click

    Cursor.Current = Cursors.WaitCursor
    Dim frm As New ProjectEdit
    frm.MdiParent = Me
    frm.Project = Project.NewProject
    Cursor.Current = Cursors.Default
    frm.Show()

  End Sub

#End Region

#Region " Resources "

  Private Sub mnuResourceNew_Click(ByVal sender As System.Object, _
      ByVal e As System.EventArgs) Handles mnuResourceNew.Click

    Dim ID As String = InputBox("Resource ID", "New resource")
    If Len(ID) > 0 Then
      Cursor.Current = Cursors.WaitCursor
      Dim obj As Resource = Resource.NewResource(ID)
      Dim frm As New ResourceEdit
      frm.MdiParent = Me
      frm.Resource = obj
      Cursor.Current = Cursors.Default
      frm.Show()
    End If

  End Sub

  Private Sub mnuResourceEdit_Click(ByVal sender As System.Object, _
      ByVal e As System.EventArgs) Handles mnuResourceEdit.Click

    Dim dlg As New ResourceSelect
    dlg.Text = "Edit Resource"
    dlg.ShowDialog(Me)

    Dim Result As String = dlg.Result
    If Len(Result) > 0 Then
      Try
        Cursor.Current = Cursors.WaitCursor
        Dim obj As Resource = Resource.GetResource(Result)
        Dim frm As New ResourceEdit
        frm.MdiParent = Me
        frm.Resource = obj
        Cursor.Current = Cursors.Default
        frm.Show()

      Catch ex As Exception
        Cursor.Current = Cursors.Default
        MsgBox("Error loading resource" & vbCrLf & ex.ToString)
      End Try
    End If

  End Sub

  Private Sub mnuResourceRemove_Click(ByVal sender As System.Object, _
      ByVal e As System.EventArgs) Handles mnuResourceRemove.Click

    Dim dlg As New ResourceSelect
    dlg.Text = "Remove Resource"
    dlg.ShowDialog(Me)

    Dim Result As String = dlg.Result
    If Len(Result) > 0 Then
      If MsgBox("Remove resource " & Result, _
          MsgBoxStyle.YesNo, "Remove resource") = MsgBoxResult.Yes Then
        Try
          Cursor.Current = Cursors.WaitCursor
          pnlStatus.Text = "Removing resource..."

          Resource.DeleteResource(Result)
          Cursor.Current = Cursors.Default
          MsgBox("Resource deleted")

        Catch ex As Exception
          Cursor.Current = Cursors.Default
          MsgBox("Error deleting resource" & vbCrLf & ex.ToString)

        Finally
          pnlStatus.Text = ""
        End Try
      End If
    End If

  End Sub

#End Region

#Region " Reports "

  Private Sub mnuProjectList_Click(ByVal sender As System.Object, _
      ByVal e As System.EventArgs) Handles mnuProjectList.Click

    Dim dlg As New ProjectSelect
    dlg.Text = "Project List"
    dlg.ShowDialog(Me)

    Dim Result As String = dlg.Result
    If Len(Result) > 0 Then
      Try
        Cursor.Current = Cursors.WaitCursor
        Dim ID As Guid = New Guid(Result)
        Dim obj As Project = Project.GetProject(ID)

        Dim frm As New ProjectList
        frm.MdiParent = Me
        frm.WindowState = FormWindowState.Maximized
        frm.Project = obj
        Cursor.Current = Cursors.Default
        frm.Show()

      Catch ex As Exception
        Cursor.Current = Cursors.Default
        MsgBox("Error loading project " & vbCrLf & ex.ToString)
      End Try
    End If

  End Sub

  Private Sub mnuProjectReport_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)

  End Sub

#End Region

#Region " Batch Submissions "

  Private Sub mnuProjectUpdate_Click(ByVal sender As System.Object, _
      ByVal e As System.EventArgs) Handles mnuProjectUpdate.Click

    Dim batch As New BatchQueue

    batch.Submit(New BatchJobRequest("PTBatch.ProjectJob", "PTBatch"))

  End Sub

#End Region

End Class
