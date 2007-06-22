Imports System.Windows.Controls

''' <summary>
''' Interaction logic for Login.xaml
''' </summary>
Partial Public Class Login
  Inherits Window

  Private mResult As Boolean

  Private Sub Login_Loaded(ByVal sender As Object, ByVal e As System.Windows.RoutedEventArgs) Handles Me.Loaded

    UsernameTextBox.Focus()

  End Sub

  Public Property Result() As Boolean
    Get
      Return mResult
    End Get
    Set(ByVal value As Boolean)
      mResult = value
    End Set
  End Property

  Private Sub LoginButton(ByVal sender As Object, ByVal e As EventArgs)
    mResult = True
    Me.Close()
  End Sub

  Private Sub CancelButton(ByVal sender As Object, ByVal e As EventArgs)
    mResult = False
    Me.Close()
  End Sub

End Class