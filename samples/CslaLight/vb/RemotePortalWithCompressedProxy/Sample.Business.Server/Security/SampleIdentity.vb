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

#If (Not SILVERLIGHT) Then
Imports System.Data.SqlClient
#End If


Namespace Sample.Business
  <Serializable(), Csla.Server.ObjectFactory("Sample.Business.SampleIdentityFactory, Sample.Business", "", "GetSampleIdentity", "", "")> _
  Public Class SampleIdentity
	  Inherits CslaIdentity

#If SILVERLIGHT Then

	Public Shared Sub GetIdentity(ByVal username As String, ByVal password As String, ByVal completed As EventHandler(Of DataPortalResult(Of SampleIdentity)))
	  GetCslaIdentity(Of SampleIdentity)(completed, New CredentialsCriteria(username, password))
	End Sub

#Else

	  Friend Sub LoadData(ByVal userName As String, ByVal rolesList As MobileList(Of String))
		AuthenticationType = "Csla"
		Roles = rolesList
		Name = userName
		IsAuthenticated = True
	  End Sub

#End If

  End Class

End Namespace 'end of root namespace