Imports CSLA.Security
Imports System.Threading

Public Class Login
  Inherits System.Web.UI.Page

#Region " Web Form Designer Generated Code "

  'This call is required by the Web Form Designer.
  <System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()

  End Sub
  Protected WithEvents btnLogin As System.Web.UI.WebControls.Button
  Protected WithEvents RequiredFieldValidator1 As System.Web.UI.WebControls.RequiredFieldValidator
  Protected WithEvents txtPassword As System.Web.UI.WebControls.TextBox
  Protected WithEvents RequiredFieldValidator2 As System.Web.UI.WebControls.RequiredFieldValidator
  Protected WithEvents txtUsername As System.Web.UI.WebControls.TextBox

  'NOTE: The following placeholder declaration is required by the Web Form Designer.
  'Do not delete or move it.
  Private designerPlaceholderDeclaration As System.Object

  Private Sub Page_Init(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Init
    'CODEGEN: This method call is required by the Web Form Designer
    'Do not modify it using the code editor.
    InitializeComponent()
  End Sub

#End Region

  Private Sub Page_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
    'Put user code to initialize the page here
  End Sub

  Private Sub btnLogin_Click(ByVal sender As System.Object, _
      ByVal e As System.EventArgs) Handles btnLogin.Click

    Dim UserName As String = txtUsername.Text
    Dim Password As String = txtPassword.Text

    ' if we're logging in, clear the current session
    Session.Clear()

    ' log into the system
    BusinessPrincipal.Login(UserName, Password)

    ' see if we logged in successfully 
    If Thread.CurrentPrincipal.Identity.IsAuthenticated Then
      Session("CSLA-Principal") = Threading.Thread.CurrentPrincipal
      HttpContext.Current.User = Session("CSLA-Principal")

      ' redirect to the page the user requested
      Web.Security.FormsAuthentication.RedirectFromLoginPage(UserName, False)
    End If

  End Sub

End Class
