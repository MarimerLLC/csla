Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Net
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Shapes
Imports Rolodex.Business.Security

Namespace Rolodex
  Partial Public Class LoginControl
    Inherits UserControl
    Public Sub New()
      InitializeComponent()
    End Sub

    Public Event LoginSuccessfull As EventHandler

    Private Sub LogInButton_Click(ByVal sender As Object, ByVal e As RoutedEventArgs)
      LogInButton.IsEnabled = False
      Status.Text = "Validating credentials..."
      animation.Visibility = Visibility.Visible
      Status.Visibility = Visibility.Visible
      animation.IsRunning = True
      RolodexPrincipal.Login(UserIdBox.Text.Trim(), UserPwdBox.Text.Trim(), AddressOf EndLogin)
    End Sub

    Private Sub EndLogin(ByVal sender As Object, ByVal e As EventArgs)
      animation.IsRunning = False
      If Csla.ApplicationContext.User.Identity.IsAuthenticated Then
        Status.Text = "Login Successfull."
        If LoginSuccessfullEvent IsNot Nothing Then
          LoginSuccessfullEvent.Invoke(Me, EventArgs.Empty)
        End If
      Else
        Status.Text = "Invalid login. Try again."
        LogInButton.IsEnabled = True
      End If
    End Sub
  End Class
End Namespace
