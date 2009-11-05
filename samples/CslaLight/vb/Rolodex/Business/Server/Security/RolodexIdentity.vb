Imports Microsoft.VisualBasic
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

#If(Not SILVERLIGHT) Then
Imports System.Data.SqlClient
Imports Rolodex.Business.Data
#End If

Namespace Rolodex.Business.Security
  <Serializable()> _
  Public Class RolodexIdentity
	  Inherits CslaIdentity

    Private Shared UserIdProperty As PropertyInfo(Of Integer) = _
      RegisterProperty(Of Integer)(GetType(RolodexIdentity), New PropertyInfo(Of Integer)("UserId", "User Id", 0))
	Public ReadOnly Property UserId() As Integer
	  Get
		Return GetProperty(Of Integer)(UserIdProperty)
	  End Get
	End Property

#If SILVERLIGHT Then

	Public Sub New()
	End Sub

	Public Shared Sub GetIdentity(ByVal username As String, ByVal password As String, ByVal completed As EventHandler(Of DataPortalResult(Of RolodexIdentity)))
      GetCslaIdentity(Of RolodexIdentity)(completed, New CredentialsCriteria(username, password))
	End Sub

#Else
	Public Shared Sub GetIdentity(ByVal username As String, ByVal password As String, ByVal roles As String)
	  GetCslaIdentity(Of RolodexIdentity)(New CredentialsCriteria(username, password))
	End Sub

	Private Sub DataPortal_Fetch(ByVal criteria As CredentialsCriteria)
	  Using connection As New SqlConnection(DataConnection.ConnectionString)
		connection.Open()
		Using command As New SqlCommand("GetUser", connection)
		  command.CommandType = System.Data.CommandType.StoredProcedure
		  command.Parameters.Add(New SqlParameter("@userName",criteria.Username))
		  Using reader As New Csla.Data.SafeDataReader(command.ExecuteReader())
			If reader.Read() Then
			  If criteria.Password = reader.GetString("Password") Then
				LoadProperty(Of Integer)(UserIdProperty, reader.GetInt32(0))
				Name = reader.GetString(1)
				Roles = New MobileList(Of String)(New String() { reader.GetString("Role") })
				IsAuthenticated = True
			  End If
			End If
		  End Using
		End Using
		connection.Close()
	  End Using


	End Sub


#End If
  End Class
End Namespace
