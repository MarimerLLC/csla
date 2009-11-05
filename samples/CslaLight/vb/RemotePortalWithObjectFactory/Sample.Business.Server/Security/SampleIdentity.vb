Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports Csla
Imports Csla.Security
Imports Csla.Core
Imports Csla.Serialization
Imports Csla.DataPortalClient
Imports System.ComponentModel

#If Not SILVERLIGHT = 1 Then
Imports System.Data.SqlClient
#End If


<Serializable(), Csla.Server.ObjectFactory("Sample.Business.SampleIdentityFactory, Sample.Business", "", "GetSampleIdentity", "", "")> _
Public Class SampleIdentity
  Inherits CslaIdentity

#If Silverlight = 1 Then

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
