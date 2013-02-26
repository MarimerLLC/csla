Class Window1 

  Private Sub Window_Loaded(ByVal sender As System.Object, ByVal e As System.Windows.RoutedEventArgs)

    Csla.ApplicationContext.GlobalContext.Add("Test", "Test context")
    Dim dp = New Csla.DataPortal(Of ThreadCheck)
    AddHandler dp.CreateCompleted, AddressOf dp_CreateCompleted
    'dp.BeginCreate()
    dp.BeginCreate(New Csla.SingleCriteria(Of ThreadCheck, String)("Hi there"))

  End Sub

  Private Sub dp_CreateCompleted(ByVal sender As Object, ByVal e As Csla.DataPortalResult(Of ThreadCheck))

    If e.Error IsNot Nothing Then
      MessageBox.Show(e.Error.ToString)
    Else
      Me.Thread.Text = System.Threading.Thread.CurrentThread.ManagedThreadId.ToString 'e.Object.Thread.ToString
      Me.CreatedThread.Text = e.Object.CreateThread.ToString
      Me.Data.Text = e.Object.Data
      Me.Context.Text = Csla.ApplicationContext.GlobalContext("Test").ToString
      Me.ReturnedContext.Text = CType(sender, Csla.DataPortal(Of ThreadCheck)).GlobalContext("Test").ToString
    End If

  End Sub

End Class
