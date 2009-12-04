Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports Csla
Imports Csla.Security
Imports Csla.Core
Imports Csla.Serialization
Imports System.Security.Principal

#If (Not SILVERLIGHT) Then
Imports System.Data.SqlClient
Imports Rolodex.Business.Data
#End If


Namespace Rolodex.Business.Security
  <Serializable()> _
  Public Class RolodexPrincipal
    Inherits BusinessPrincipalBase
    Private Sub New(ByVal identity As IIdentity)
      MyBase.New(identity)
    End Sub

    Public Sub New()
      MyBase.New()
    End Sub

#If SILVERLIGHT Then

    Private Shared _handler As EventHandler(Of EventArgs)

    Public Shared Sub Login(ByVal username As String, ByVal password As String, ByVal completed As EventHandler(Of EventArgs))
      _handler = completed
      RolodexIdentity.GetIdentity(username, password, AddressOf EndGetIdentity)
    End Sub

    Private Shared Sub EndGetIdentity(ByVal sender As Object, ByVal e As DataPortalResult(Of RolodexIdentity))
      If e.Object Is Nothing Then
        SetPrincipal(RolodexIdentity.UnauthenticatedIdentity())
      Else
        SetPrincipal(e.Object)
      End If
      _handler(Nothing, EventArgs.Empty)
    End Sub
#Else
	Public Shared Sub Login(ByVal username As String, ByVal password As String, ByVal roles As String)
	  RolodexIdentity.GetIdentity(username, password, roles)
	End Sub
#End If

    Private Shared Sub SetPrincipal(ByVal identity As Csla.Security.CslaIdentity)
      Dim principal As New RolodexPrincipal(identity)
      Csla.ApplicationContext.User = principal
    End Sub

    Public Shared Sub Logout()
      Dim identity As Csla.Security.CslaIdentity = RolodexIdentity.UnauthenticatedIdentity()
      Dim principal As New RolodexPrincipal(identity)
      Csla.ApplicationContext.User = principal
    End Sub

    Public Overrides Function IsInRole(ByVal role As String) As Boolean
      Return (CType(MyBase.Identity, ICheckRoles)).IsInRole(role)
    End Function
  End Class
End Namespace
