Partial Public Class EditClientProjects

  Private Sub ClientComboBox_SelectionChanged(ByVal sender As Object, ByVal e As System.Windows.Controls.SelectionChangedEventArgs) Handles ClientComboBox.SelectionChanged

    Using New StatusBusy("Loading data...")
      Dim id As Integer = CType(ClientComboBox.SelectedItem, Library.ClientNVList.NameValuePair).Key
      Dim list As Library.ClientProjectList = BenchMgr.Library.ClientProjectList.GetList(id)

      list.BeginEdit()

      Me.DataContext = list

      Me.AddItemButton.IsEnabled = True

      ApplyAuthorization()

    End Using

  End Sub

  Private Sub ApplyAuthorization()

  End Sub

  Private Sub RemoveItem(ByVal sender As Object, ByVal e As EventArgs)

    Dim btn As Button = CType(sender, Button)
    Dim id As Integer = CInt(btn.Tag)
    Dim list As Library.ClientProjectList = CType(Me.DataContext, Library.ClientProjectList)
    For Each item As Library.ClientProjectEdit In list
      If item.Id = id Then
        list.Remove(item)
        Exit For
      End If
    Next

  End Sub

  Private Sub SaveButton_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles SaveButton.Click

    Dim list As Library.ClientProjectList = CType(Me.DataContext, Library.ClientProjectList)
    list = SaveObject(list)
    Me.DataContext = Nothing
    Me.DataContext = list

  End Sub

  Private Sub CancelButton_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles CancelButton.Click

    Dim list As Library.ClientProjectList = CType(Me.DataContext, Library.ClientProjectList)
    list.CancelEdit()
    list.BeginEdit()

  End Sub

  Private Sub AddItemButton_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles AddItemButton.Click

    CType(Me.DataContext, Library.ClientProjectList).AddNew()

  End Sub

End Class
