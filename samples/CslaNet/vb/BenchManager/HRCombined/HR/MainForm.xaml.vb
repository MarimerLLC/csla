Partial Public Class MainForm

  Private WithEvents _engine As New Workflow.Runtime.WorkflowRuntime

  Private Sub SaveButton_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles SaveButton.Click

    SaveButton.IsEnabled = False

    Dim args As New Dictionary(Of String, Object)
    args.Add("EmployeeData", CType(MainPanel.DataContext, EmployeeWorkflows.NewEmployeeData))
    Dim wf As Workflow.Runtime.WorkflowInstance = _engine.CreateWorkflow(GetType(EmployeeWorkflows.NewEmployee), args)
    wf.Start()

  End Sub

  Private Delegate Sub WFComplete(ByVal e As System.Workflow.Runtime.WorkflowCompletedEventArgs)

  ' executed on background thread
  Private Sub _engine_WorkflowCompleted(ByVal sender As Object, ByVal e As System.Workflow.Runtime.WorkflowCompletedEventArgs) Handles _engine.WorkflowCompleted

    SaveButton.Dispatcher.BeginInvoke( _
                                      Windows.Threading.DispatcherPriority.Normal, _
                                      New WFComplete(AddressOf WorkflowCompleted), e)

  End Sub

  ' executed on UI thread
  Private Sub WorkflowCompleted(ByVal e As System.Workflow.Runtime.WorkflowCompletedEventArgs)

    Dim errorText As String = TryCast(e.OutputParameters("ErrorText"), String)

    SaveButton.IsEnabled = True
    If String.IsNullOrEmpty(errorText) Then
      Dim emp As EmployeeWorkflows.NewEmployeeData = CType(e.OutputParameters("EmployeeData"), EmployeeWorkflows.NewEmployeeData)
      MessageBox.Show(String.Format("Employee {0} was added and configured.", emp.Name))

    Else
      MessageBox.Show(errorText, "Update error", MessageBoxButton.OK, MessageBoxImage.Exclamation)
    End If

  End Sub

End Class
