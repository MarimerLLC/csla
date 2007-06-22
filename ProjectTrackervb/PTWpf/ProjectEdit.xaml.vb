Imports System.Windows
Imports ProjectTracker.Library

''' <summary>
''' Interaction logic for ProjectEdit.xaml
''' </summary>
Partial Public Class ProjectEdit
  Inherits EditForm

  Private mProjectId As Guid

  Public Sub New()

    InitializeComponent()

    Dim dp As Csla.Wpf.CslaDataProvider = TryCast(Me.FindResource("Project"), Csla.Wpf.CslaDataProvider)
    AddHandler dp.DataChanged, AddressOf DataChanged

  End Sub

  Public Sub New(ByVal id As Guid)
    Me.New()
    mProjectId = id
  End Sub

  Private Sub ProjectEdit_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded

    Dim dp As Csla.Wpf.CslaDataProvider = TryCast(Me.FindResource("Project"), Csla.Wpf.CslaDataProvider)
    Using dp.DeferRefresh()
      dp.FactoryParameters.Clear()
      If mProjectId.Equals(Guid.Empty) Then
        dp.FactoryMethod = "NewProject"

      Else
        dp.FactoryMethod = "GetProject"
        dp.FactoryParameters.Add(mProjectId)
      End If
    End Using

    If Not dp.Data Is Nothing Then
      SetTitle(CType(dp.Data, Project))

    Else
      MainForm.ShowControl(Nothing)
    End If

  End Sub

  Private Sub SetTitle(ByVal project As Project)
    If project.IsNew Then
      Me.Title = String.Format("Project: {0}", "<new>")
    Else
      Me.Title = String.Format("Project: {0}", project.Name)
    End If
  End Sub

  Private Sub ShowResource(ByVal sender As Object, ByVal e As EventArgs)
    Dim item As ProjectTracker.Library.ProjectResource = CType(ResourceListBox.SelectedItem, ProjectTracker.Library.ProjectResource)

    If Not item Is Nothing Then
      Dim frm As ResourceEdit = New ResourceEdit(item.ResourceId)
      MainForm.ShowControl(frm)
    End If
  End Sub

  Protected Overrides Sub ApplyAuthorization()
    Me.AuthPanel.Refresh()
    If Project.CanEditObject() Then
      Me.ResourceListBox.ItemTemplate = CType(Me.MainGrid.Resources("lbTemplate"), DataTemplate)
      Me.AssignButton.IsEnabled = True
    Else
      Me.ResourceListBox.ItemTemplate = CType(Me.MainGrid.Resources("lbroTemplate"), DataTemplate)
      CType(Me.FindResource("Project"), Csla.Wpf.CslaDataProvider).Cancel()
      Me.AssignButton.IsEnabled = False
    End If
  End Sub

  Private Sub Assign(ByVal sender As Object, ByVal e As EventArgs)
    Dim dlg As ResourceSelect = New ResourceSelect()
    If CBool(dlg.ShowDialog()) Then
      Dim id As Integer = dlg.ResourceId
      Dim project As Project = CType((CType(Me.FindResource("Project"), Csla.Wpf.CslaDataProvider)).Data, Project)
      Try
        project.Resources.Assign(id)
      Catch ex As Exception
        MessageBox.Show(ex.Message, "Assignment error", MessageBoxButton.OK, MessageBoxImage.Information)
      End Try
    End If
  End Sub

  Private Sub Unassign(ByVal sender As Object, ByVal e As EventArgs)
    Dim btn As Button = CType(sender, Button)
    Dim id As Integer = CInt(Fix(btn.Tag))
    Dim project As Project = CType((CType(Me.FindResource("Project"), Csla.Wpf.CslaDataProvider)).Data, Project)
    project.Resources.Remove(id)
  End Sub

End Class