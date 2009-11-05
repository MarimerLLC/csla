Partial Public Class MainForm

  Private Sub ClientListButton_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles ClientListButton.Click

    Dim frm As New ClientList
    frm.ShowDialog()

  End Sub

  Private Sub EditConsultantButton_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles EditConsultantButton.Click

    Dim frm As New EditConsultants
    frm.ShowDialog()

  End Sub

  Public Shared Property StatusText() As String
    Get
      Return _mainForm.StatusTextBlock.Text
    End Get
    Set(ByVal value As String)
      _mainForm.StatusTextBlock.Text = value
    End Set
  End Property

  Public Shared Property CurrentCursor() As Cursor
    Get
      Return _mainForm.Cursor
    End Get
    Set(ByVal value As Cursor)
      _mainForm.Cursor = value
    End Set
  End Property

  Private Shared _mainForm As MainForm

  Private Sub MainForm_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded
    _mainForm = Me
  End Sub

  Private Sub EditClientProjectsButton_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles EditClientProjectsButton.Click

    Dim frm As New EditClientProjects
    frm.ShowDialog()

  End Sub

  Private Sub ManageBenchButton_Click(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles ManageBenchButton.Click

    Dim frm As New ManageBench
    frm.ShowDialog()

  End Sub

End Class
