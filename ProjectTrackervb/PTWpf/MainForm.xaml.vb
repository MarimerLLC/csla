''' <summary>
''' Interaction logic for MainForm.xaml
''' </summary>
Partial Public Class MainForm
  Inherits Window

#Region "Navigation and Plumbing"

  Private Shared mPrincipal As ProjectTracker.Library.Security.PTPrincipal
  Private Shared mMainForm As MainForm

  Private mCurrentControl As UserControl

  Public Sub New()

    InitializeComponent()

    mMainForm = Me

    AddHandler Csla.DataPortal.DataPortalInitInvoke, AddressOf DataPortal_DataPortalInitInvoke

  End Sub

  Private Sub MainForm_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded

    ProjectTracker.Library.Security.PTPrincipal.Logout()
    mPrincipal = CType(Csla.ApplicationContext.User, ProjectTracker.Library.Security.PTPrincipal)

    Me.Title = "Project Tracker"

  End Sub

  ''' <summary>
  ''' This method ensures that the thread about to do
  ''' data access has a valid PTPrincipal object, and is
  ''' needed because of the way WPF doesn't move the 
  ''' main thread's principal object to other threads
  ''' automatically.
  ''' </summary>
  ''' <param name="obj"></param>
  Private Sub DataPortal_DataPortalInitInvoke(ByVal obj As Object)
    If Not ReferenceEquals(Csla.ApplicationContext.User, mPrincipal) Then
      Csla.ApplicationContext.User = mPrincipal
    End If
  End Sub


  Public Shared Sub ShowControl(ByVal control As UserControl)
    mMainForm.ShowUserControl(control)
  End Sub

  Private Sub ShowUserControl(ByVal control As UserControl)

    UnHookTitleEvent(mCurrentControl)

    contentArea.Children.Clear()
    If Not control Is Nothing Then
      contentArea.Children.Add(control)
    End If
    mCurrentControl = control

    HookTitleEvent(mCurrentControl)

  End Sub

  Private Sub UnHookTitleEvent(ByVal control As UserControl)

    Dim form As EditForm = TryCast(control, EditForm)
    If Not form Is Nothing Then
      RemoveHandler form.TitleChanged, AddressOf SetTitle
    End If

  End Sub

  Private Sub HookTitleEvent(ByVal control As UserControl)

    SetTitle(control, EventArgs.Empty)
    Dim form As EditForm = TryCast(control, EditForm)
    If Not form Is Nothing Then
      AddHandler form.TitleChanged, AddressOf SetTitle
    End If

  End Sub

  Private Sub SetTitle(ByVal sender As Object, ByVal e As EventArgs)

    Dim form As EditForm = TryCast(sender, EditForm)
    If Not form Is Nothing AndAlso (Not String.IsNullOrEmpty(form.Title)) Then
      mMainForm.Title = String.Format("Project Tracker - {0}", (CType(sender, EditForm)).Title)
    Else
      mMainForm.Title = String.Format("Project Tracker")
    End If

  End Sub

#End Region

#Region "Menu items"

  Private Sub NewProject(ByVal sender As Object, ByVal e As EventArgs)
    Try
      Dim frm As ProjectEdit = New ProjectEdit(Guid.Empty)
      ShowControl(frm)
    Catch ex As System.Security.SecurityException
      MessageBox.Show(ex.ToString())
    End Try
  End Sub

  Private Sub ShowProjectList(ByVal sender As Object, ByVal e As EventArgs)
    Dim frm As ProjectList = New ProjectList()
    ShowControl(frm)
  End Sub

  Private Sub ShowResourceList(ByVal sender As Object, ByVal e As EventArgs)
    Dim frm As ResourceList = New ResourceList()
    ShowControl(frm)
  End Sub

  Private Sub NewResource(ByVal sender As Object, ByVal e As EventArgs)
    Dim frm As ResourceEdit = New ResourceEdit(0)
    ShowControl(frm)
  End Sub

  Private Sub ShowRolesEdit(ByVal sender As Object, ByVal e As EventArgs)
    Dim frm As RolesEdit = New RolesEdit()
    ShowControl(frm)
  End Sub

#End Region

#Region "Login/Logout"

  Private Sub LogInOut(ByVal sender As Object, ByVal e As EventArgs)
    If Csla.ApplicationContext.User.Identity.IsAuthenticated Then
      ProjectTracker.Library.Security.PTPrincipal.Logout()
      CurrentUser.Text = "Not logged in"
      LoginButtonText.Text = "Log in"
    Else
      Dim frm As Login = New Login()
      frm.ShowDialog()
      If frm.Result Then
        Dim username As String = frm.UsernameTextBox.Text
        Dim password As String = frm.PasswordTextBox.Password
        ProjectTracker.Library.Security.PTPrincipal.Login(username, password)
      End If
      If (Not Csla.ApplicationContext.User.Identity.IsAuthenticated) Then
        ProjectTracker.Library.Security.PTPrincipal.Logout()
        CurrentUser.Text = "Not logged in"
        LoginButtonText.Text = "Log in"
      Else
        CurrentUser.Text = String.Format("Logged in as {0}", Csla.ApplicationContext.User.Identity.Name)
        LoginButtonText.Text = "Log out"
      End If
    End If
    mPrincipal = _
      CType(Csla.ApplicationContext.User, ProjectTracker.Library.Security.PTPrincipal)
    Dim p As IRefresh = TryCast(mCurrentControl, IRefresh)
    If Not p Is Nothing Then
      p.Refresh()
    End If
  End Sub

#End Region

End Class