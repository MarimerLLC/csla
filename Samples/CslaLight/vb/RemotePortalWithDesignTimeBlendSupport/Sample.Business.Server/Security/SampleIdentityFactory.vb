'INSTANT C# NOTE: Formerly VB.NET project-level imports:

Imports Microsoft.VisualBasic
Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.Data
Imports System.Diagnostics
Imports System.Linq
Imports System.Xml.Linq

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
  Public Class SampleIdentityFactory

	Public Function GetSampleIdentity(ByVal criteria As CredentialsCriteria) As SampleIdentity
	  Dim returnValue As New SampleIdentity()
	  Using connection As New SqlConnection(DataConnection.ConnectionString)
		connection.Open()
		Using command As New SqlCommand("GetUser", connection)
		  command.CommandType = System.Data.CommandType.StoredProcedure
		  command.Parameters.Add(New SqlParameter("@userName", criteria.UserName))
		  Using reader As New Csla.Data.SafeDataReader(command.ExecuteReader())
			If reader.Read() Then
			  If criteria.Password = reader.GetString("Password") Then
				returnValue.LoadData(reader.GetString(1), New MobileList(Of String)(New String() { reader.GetString("Role") }))

			  End If
			End If
		  End Using
		End Using
	  End Using
	  Return returnValue
	End Function


  End Class

End Namespace 'end of root namespace