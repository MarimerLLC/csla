Imports ProjectTracker.Library

''' <summary>
''' Interaction logic for ResourceEdit.xaml
''' </summary>
Partial Public Class ResourceEdit
  Inherits EditForm

  Private _resourceId As Integer

  Public Sub New()

    InitializeComponent()

    Dim dp As Csla.Wpf.CslaDataProvider = TryCast(Me.FindResource("Resource"), Csla.Wpf.CslaDataProvider)
    AddHandler dp.DataChanged, AddressOf DataChanged

  End Sub

  Public Sub New(ByVal resourceId As Integer)
    Me.New()
    _resourceId = resourceId
  End Sub

  Private Sub ResourceEdit_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded

    Dim dp As Csla.Wpf.CslaDataProvider = TryCast(Me.FindResource("Resource"), Csla.Wpf.CslaDataProvider)
    Using dp.DeferRefresh()
      dp.FactoryParameters.Clear()
      If _resourceId = 0 Then
        dp.FactoryMethod = "NewResource"

      Else
        dp.FactoryMethod = "GetResource"
        dp.FactoryParameters.Add(_resourceId)
      End If
    End Using

    If Not dp.Data Is Nothing Then
      SetTitle(CType(dp.Data, Resource))

    Else
      MainForm.ShowControl(Nothing)
    End If

  End Sub

  Private Sub SetTitle(ByVal resource As Resource)
    If resource.IsNew Then
      Me.Title = String.Format("Resource: {0}", "<new>")
    Else
      Me.Title = String.Format("Resource: {0}", resource.FullName)
    End If
  End Sub

  Private Sub ShowProject(ByVal sender As Object, ByVal e As EventArgs)
    Dim item As ProjectTracker.Library.ResourceAssignment = CType(ProjectListBox.SelectedItem, ProjectTracker.Library.ResourceAssignment)

    If Not item Is Nothing Then
      Dim frm As ProjectEdit = New ProjectEdit(item.ProjectId)
      MainForm.ShowControl(frm)
    End If
  End Sub

  Protected Overrides Sub ApplyAuthorization()
    Me.AuthPanel.Refresh()
    If Resource.CanEditObject() Then
      Me.ProjectListBox.ItemTemplate = CType(Me.MainGrid.Resources("lbTemplate"), DataTemplate)
      Me.AssignButton.IsEnabled = True
    Else
      Me.ProjectListBox.ItemTemplate = CType(Me.MainGrid.Resources("lbroTemplate"), DataTemplate)
      CType(Me.FindResource("Resource"), Csla.Wpf.CslaDataProvider).Cancel()
      Me.AssignButton.IsEnabled = False
    End If
  End Sub

  Private Sub Assign(ByVal sender As Object, ByVal e As EventArgs)
    Dim dlg As ProjectSelect = New ProjectSelect()
    If CBool(dlg.ShowDialog()) Then
      Dim id As Guid = dlg.ProjectId
      Dim resource As Resource = CType((CType(Me.FindResource("Resource"), Csla.Wpf.CslaDataProvider)).Data, Resource)
      Try
        resource.Assignments.AssignTo(id)
      Catch ex As Exception
        MessageBox.Show(ex.Message, "Assignment error", MessageBoxButton.OK, MessageBoxImage.Information)
      End Try
    End If
  End Sub

  Private Sub Unassign(ByVal sender As Object, ByVal e As EventArgs)
    Dim btn As Button = CType(sender, Button)
    Dim id As Guid = CType(btn.Tag, Guid)
    Dim resource As Resource = CType((CType(Me.FindResource("Resource"), Csla.Wpf.CslaDataProvider)).Data, Resource)
    resource.Assignments.Remove(id)
  End Sub

End Class