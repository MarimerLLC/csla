Imports System.Web
Imports System.Web.SessionState
Imports System.Threading

Public Class Global
  Inherits System.Web.HttpApplication

#Region " Component Designer Generated Code "

  Public Sub New()
    MyBase.New()

    'This call is required by the Component Designer.
    InitializeComponent()

    'Add any initialization after the InitializeComponent() call

  End Sub

  'Required by the Component Designer
  Private components As System.ComponentModel.IContainer

  'NOTE: The following procedure is required by the Component Designer
  'It can be modified using the Component Designer.
  'Do not modify it using the code editor.
  <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
    components = New System.ComponentModel.Container
  End Sub

#End Region

  Private Sub Global_AcquireRequestState(ByVal sender As Object, _
      ByVal e As System.EventArgs) Handles MyBase.AcquireRequestState

    ' set the security principal to our BusinessPrincipal
    If Not Session("CSLA-Principal") Is Nothing Then
      Thread.CurrentPrincipal = Session("CSLA-Principal")
      HttpContext.Current.User = Session("CSLA-Principal")

    Else
      If Thread.CurrentPrincipal.Identity.IsAuthenticated Then
        Web.Security.FormsAuthentication.SignOut()
        Server.Transfer("Login.aspx")
      End If
    End If

  End Sub

End Class
