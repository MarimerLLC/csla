Module Module1
  Class Program

    Shared WaitHandle As New AutoResetEvent(False)

    Shared Sub Main()
      Using workflowRuntime As New WorkflowRuntime()
        AddHandler workflowRuntime.WorkflowCompleted, AddressOf OnWorkflowCompleted
        AddHandler workflowRuntime.WorkflowTerminated, AddressOf OnWorkflowTerminated

        Dim workflowInstance As WorkflowInstance
        Dim params As New Dictionary(Of String, Object)
        params.Add("ProjectId", New Guid("750a346c-6c08-48c8-9329-e306ce8c7299"))
        workflowInstance = _
          workflowRuntime.CreateWorkflow(GetType(ProjectWorkflow), params)
        workflowInstance.Start()
        WaitHandle.WaitOne()
      End Using
    End Sub

    Shared Sub OnWorkflowCompleted(ByVal sender As Object, ByVal e As WorkflowCompletedEventArgs)
      WaitHandle.Set()
    End Sub

    Shared Sub OnWorkflowTerminated(ByVal sender As Object, ByVal e As WorkflowTerminatedEventArgs)
      Console.WriteLine(e.Exception.Message)
      WaitHandle.Set()
    End Sub

  End Class
End Module

