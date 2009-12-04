'INSTANT C# NOTE: Formerly VB.NET project-level imports:

Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Diagnostics
Imports System.Linq

Imports System.Text
Imports Csla
Imports Csla.Core
Imports Csla.Serialization

Namespace Sample.Business
	<Serializable()> _
	Public Class CredentialsCriteria
		Inherits CriteriaBase

	  Public Sub New()
	  End Sub

	  Private _userName As String = String.Empty
	  Private _password As String = String.Empty

	  Private Shared _passwordProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(GetType(CredentialsCriteria), New PropertyInfo(Of String)("Password", "Password"))

	  Public ReadOnly Property Password() As String
		Get
		  Return ReadProperty(_passwordProperty)
		End Get
	  End Property


	  Private Shared _userNameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(GetType(CredentialsCriteria), New PropertyInfo(Of String)("UserName", "User Name"))
	  Public ReadOnly Property UserName() As String
		Get
		  Return ReadProperty(_userNameProperty)
		End Get
	  End Property

	  Public Sub New(ByVal userName As String, ByVal password As String)
		LoadProperty(_userNameProperty, userName)
		LoadProperty(_passwordProperty, password)
	  End Sub

	End Class

End Namespace 'end of root namespace