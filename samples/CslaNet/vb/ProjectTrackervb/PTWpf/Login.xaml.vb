Imports System.Windows.Controls

''' <summary>
''' Interaction logic for Login.xaml
''' </summary>
Partial Public Class Login
  Inherits Window

  Private _result As Boolean

  Private Sub Login_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded

    UsernameTextBox.Focus()

  End Sub

  Public Property Result() As Boolean
    Get
      Return _result
    End Get
    Set(ByVal value As Boolean)
      _result = value
    End Set
  End Property

  Private Sub LoginButton(ByVal sender As Object, ByVal e As EventArgs)
    _result = True
    Me.Close()
  End Sub

  Private Sub CancelButton(ByVal sender As Object, ByVal e As EventArgs)
    _result = False
    Me.Close()
  End Sub

End Class