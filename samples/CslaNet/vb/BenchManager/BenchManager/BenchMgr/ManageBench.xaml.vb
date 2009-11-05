Partial Public Class ManageBench

  Public Sub SetConsultantId(ByVal consultantId As Integer)

    For Each item In BenchListBox.Items
      If item.Key = consultantId Then
        BenchListBox.SelectedItem = item
        Exit Sub
      End If
    Next

  End Sub

  Private Sub BenchListBox_SelectionChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles BenchListBox.SelectionChanged

    Dim consultantId As Integer = CType(BenchListBox.SelectedItem, Library.ConsultantNVList.NameValuePair).Key
    Dim projects As Library.ConsultantProjectList = Library.ConsultantProjectList.GetList(consultantId)
    projects.BeginEdit()
    Me.DetailGrid.DataContext = projects

    UnAssignButton.IsEnabled = False
    If ProjectListBox.SelectedItem IsNot Nothing Then
      AssignButton.IsEnabled = True
    End If

  End Sub

  Private Sub AssignButton_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles AssignButton.Click

    Dim newProject As Library.ClientProjectNVList.NameValuePair = _
      CType(ProjectListBox.SelectedItem, Library.ClientProjectNVList.NameValuePair)
    Dim targetList As Library.ConsultantProjectList = _
      CType(DetailGrid.DataContext, Library.ConsultantProjectList)

    targetList.Add(Library.ConsultantProject.NewConsultantProject(newProject.Key))

  End Sub

  Private Sub UnAssignButton_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles UnAssignButton.Click

    Dim oldProject As Library.ConsultantProject = _
      CType(PossibleProjectsListBox.SelectedItem, Library.ConsultantProject)
    Dim targetList As Library.ConsultantProjectList = _
      CType(DetailGrid.DataContext, Library.ConsultantProjectList)

    targetList.Remove(oldProject)

  End Sub

  Private Sub ProjectListBox_SelectionChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles ProjectListBox.SelectionChanged

    If DetailGrid.DataContext IsNot Nothing Then
      AssignButton.IsEnabled = True
    End If

  End Sub

  Private Sub PossibleProjectsListBox_SelectionChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles PossibleProjectsListBox.SelectionChanged

    UnAssignButton.IsEnabled = True

  End Sub

  Private Sub SaveButton_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles SaveButton.Click

    Dim projects As Library.ConsultantProjectList = CType(Me.DetailGrid.DataContext, Library.ConsultantProjectList)
    projects = SaveObject(Of Library.ConsultantProjectList)(projects)

    ' rebind
    Me.DetailGrid.DataContext = Nothing
    Me.DetailGrid.DataContext = projects

  End Sub

  Private Sub CancelButton_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles CancelButton.Click

    Dim projects As Library.ConsultantProjectList = CType(Me.DetailGrid.DataContext, Library.ConsultantProjectList)
    projects.CancelEdit()
    projects.BeginEdit()

  End Sub

End Class
