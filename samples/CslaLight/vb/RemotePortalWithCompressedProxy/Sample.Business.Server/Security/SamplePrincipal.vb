'INSTANT C# NOTE: Formerly VB.NET project-level imports:

Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Linq

Imports System.Text
Imports Csla
Imports Csla.Security
Imports Csla.Core
Imports Csla.Serialization
Imports Csla.DataPortalClient
Imports System.ComponentModel
Imports System.Security.Principal

#If (Not SILVERLIGHT) Then
Imports System.Data.SqlClient
#End If

Namespace Sample.Business
  <Serializable()> _
  Public Class SamplePrincipal
	  Inherits BusinessPrincipalBase

	Private Sub New(ByVal identity As IIdentity)
		MyBase.New(identity)
	End Sub


#If SILVERLIGHT Then
	  Public Sub New()
	  End Sub
#Else
	Private Sub New()
	End Sub
#End If


#If SILVERLIGHT Then



	  Public Shared Event SignalLogin As EventHandler(Of EventArgs)

	  Public Shared Sub Login(ByVal userName As String, ByVal password As String, ByVal completed As EventHandler(Of EventArgs))
		AddHandler SamplePrincipal.SignalLogin, completed
		SampleIdentity.GetIdentity(userName, password, AddressOf HandleLogin)
	  End Sub

	  Private Shared Sub HandleLogin(ByVal sender As Object, ByVal e As DataPortalResult(Of SampleIdentity))
		If e.Object IsNot Nothing AndAlso e.Error Is Nothing Then
		  SetPrincipal(e.Object)
		Else
		  SetPrincipal(SampleIdentity.UnauthenticatedIdentity())
		End If
		RaiseEvent SignalLogin(e.Object, EventArgs.Empty)
	  End Sub

#End If

	Private Shared Sub SetPrincipal(ByVal identity As Csla.Security.CslaIdentity)
	  Dim principal As New SamplePrincipal(identity)
	  Csla.ApplicationContext.User = principal
	End Sub

	Public Shared Sub Logout()
	  Dim identity As Csla.Security.CslaIdentity = SampleIdentity.UnauthenticatedIdentity()
	  Dim principal As New SamplePrincipal(identity)
	  Csla.ApplicationContext.User = principal
	End Sub

	Public Overrides Function IsInRole(ByVal role As String) As Boolean
	  Return (CType(MyBase.Identity, ICheckRoles)).IsInRole(role)
	End Function

  End Class

End Namespace 'end of root namespace