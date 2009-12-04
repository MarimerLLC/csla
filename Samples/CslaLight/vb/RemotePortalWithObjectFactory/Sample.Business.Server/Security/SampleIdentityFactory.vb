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

Public Class SampleIdentityFactory

  Public Function GetSampleIdentity(ByVal criteria As CredentialsCriteria) As SampleIdentity
    Dim returnValue As New SampleIdentity()
    Using connection As New SqlConnection(DataConnection.ConnectionString)
      connection.Open()
      Using command As New SqlCommand("GetUser", connection)
        command.CommandType = System.Data.CommandType.StoredProcedure
        command.Parameters.Add(New SqlParameter("@userName", criteria.UserName))
        Using reader As Csla.Data.SafeDataReader = New Csla.Data.SafeDataReader(command.ExecuteReader())
          If reader.Read() Then
            If criteria.Password = reader.GetString("Password") Then
              returnValue.LoadData(reader.GetString(1), New MobileList(Of String)(New String() {reader.GetString("Role")}))

            End If
          End If
        End Using
      End Using
    End Using
    Return returnValue
  End Function


End Class
