Imports Csla

Partial Public Class EditConsultants

  Private Sub EditConsultants_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded

    Using New StatusBusy("Loading data...")
      Dim list As Library.ConsultantList = Library.ConsultantList.GetList

      ' start editing all items so undo can work
      For Each item As Library.ConsultantEdit In list
        item.BeginEdit()
      Next

      Me.DataContext = list

      ApplyAuthorization()
    End Using

  End Sub

  Private Sub ApplyAuthorization()

  End Sub

  Private Sub RemoveItem(ByVal sender As Object, ByVal e As EventArgs)

    Dim id As Integer = CInt(CType(sender, Button).Tag)
    Dim list As Library.ConsultantList = CType(Me.DataContext, Library.ConsultantList)
    list.Remove(FindSelectedItem(id))

  End Sub

  Private Sub CancelItem(ByVal sender As Object, ByVal e As EventArgs)

    Dim id As Integer = CInt(CType(sender, Button).Tag)
    Dim item As Library.ConsultantEdit = FindSelectedItem(id)
    item.CancelEdit()
    item.BeginEdit()

  End Sub

  Private Sub SaveItem(ByVal sender As Object, ByVal e As EventArgs)

    DoSave(sender)

  End Sub

  Private Sub ShowProjectCandidates(ByVal sender As Object, ByVal e As EventArgs)

    If DoSave(sender) Then
      Dim id As Integer = CInt(CType(sender, Button).Tag)
      Dim frm As New ManageBench
      frm.SetConsultantId(id)
      frm.ShowDialog()
    End If

  End Sub

  Private Function DoSave(ByVal sender As Object) As Boolean

    Using New StatusBusy("Saving data...")
      Dim result As Boolean
      Dim id As Integer = CInt(CType(sender, Button).Tag)
      Dim item As Library.ConsultantEdit = FindSelectedItem(id)
      Dim list As Library.ConsultantList = CType(Me.DataContext, Library.ConsultantList)

      item.ApplyEdit()
      Dim index As Integer = list.IndexOf(item)
      Try
        list.SaveItem(index)
        result = True

      Catch ex As DataPortalException
        MessageBox.Show(ex.Message, "Data error")
      Catch ex As Csla.Validation.ValidationException
        MessageBox.Show(ex.Message, "Data validation error")
      Catch ex As System.Security.SecurityException
        MessageBox.Show(ex.Message, "Security error")
      Catch ex As Exception
        MessageBox.Show(ex.ToString, "Unexpected error")

      Finally
        ' saving replaces the item in the list, so
        ' call BeginEdit on new item in list
        item = list(index)
        item.BeginEdit()
      End Try

      Return result
    End Using

  End Function

  Private Function FindSelectedItem(ByVal id As Integer) As Library.ConsultantEdit

    Dim list As Library.ConsultantList = CType(Me.DataContext, Library.ConsultantList)
    For Each item As Library.ConsultantEdit In list
      If item.Id = id Then
        Return item
        Exit For
      End If
    Next
    Return Nothing

  End Function

  Private Sub AddItemButton_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles AddItemButton.Click

    Dim item As Library.ConsultantEdit = Library.ConsultantEdit.NewConsultant
    item.BeginEdit()
    CType(Me.DataContext, Library.ConsultantList).Add(item)

  End Sub

  Private Sub SaveButton_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles SaveButton.Click

    Dim list As Library.ConsultantList = CType(Me.DataContext, Library.ConsultantList)
    list.SaveAll()

  End Sub

End Class
