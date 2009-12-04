Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports Csla
Imports Csla.Core
Imports Csla.Serialization

Namespace Rolodex.Business.Security
  <Serializable()> _
  Public Class CredentialsCriteria
	  Inherits CriteriaBase

	Public Sub New()
	End Sub

	Private _username As String
	Private _password As String

	Public ReadOnly Property Username() As String
	  Get
		Return _username
	  End Get
	End Property

	Public ReadOnly Property Password() As String
	  Get
		Return _password
	  End Get
	End Property

	Public Sub New(ByVal username As String, ByVal password As String)
		MyBase.New(GetType(CredentialsCriteria))
	  _username = username
	  _password = password
	End Sub

	Protected Overrides Sub OnGetState(ByVal info As Csla.Serialization.Mobile.SerializationInfo, ByVal mode As StateMode)
	  info.AddValue("_username", _username)
	  info.AddValue("_password", _password)
	  MyBase.OnGetState(info, mode)
	End Sub

	Protected Overrides Sub OnSetState(ByVal info As Csla.Serialization.Mobile.SerializationInfo, ByVal mode As StateMode)
	  _username = CStr(info.Values("_username").Value)
	  _password = CStr(info.Values("_password").Value)
	  MyBase.OnSetState(info, mode)
	End Sub
  End Class
End Namespace
