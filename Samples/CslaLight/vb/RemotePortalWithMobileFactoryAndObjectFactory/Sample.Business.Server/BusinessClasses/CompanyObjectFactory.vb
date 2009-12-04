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
Imports Csla.Validation
Imports Csla.Server

#If Not SILVERLIGHT = 1 Then
Imports System.Data.SqlClient
#End If

Public Class CompanyObjectFactory
  Inherits ObjectFactory

  Private Function GetCompany(ByVal criteria As SingleCriteria(Of CompanyObjectFactoryTarget, Integer)) As CompanyObjectFactoryTarget
    Dim returnValue As CompanyObjectFactoryTarget = Nothing
    Using connection As New SqlConnection(DataConnection.ConnectionString)
      connection.Open()
      Using command As New SqlCommand("GetCompany", connection)
        command.CommandType = System.Data.CommandType.StoredProcedure
        command.Parameters.Add(New SqlParameter("@companyID", criteria.Value))
        Using reader As New Csla.Data.SafeDataReader(command.ExecuteReader())
          If reader.Read() Then
            returnValue = CompanyObjectFactoryTarget.LoadCompany(reader.GetInt32("CompanyID"), reader.GetString("CompanyName"), reader.GetSmartDate("DateAdded"))
            MarkOld(returnValue)
          End If
        End Using
      End Using
      connection.Close()
    End Using
    Return returnValue

  End Function

  Private Function CreateCompany() As CompanyObjectFactoryTarget
    Return CompanyObjectFactoryTarget.NewCompany()
  End Function

  Private Function SaveCompany(ByVal target As CompanyObjectFactoryTarget) As CompanyObjectFactoryTarget
    'insert DB code here
    If target.IsDeleted Then
      'DB code to delete company
      Using connection As New SqlConnection(DataConnection.ConnectionString)
        connection.Open()
        Using command As New SqlCommand("CompaniesDelete", connection)
          command.CommandType = System.Data.CommandType.StoredProcedure
          command.Parameters.Add(New SqlParameter("@companyID", target.CompanyId))
          command.ExecuteNonQuery()
        End Using
        connection.Close()
      End Using
    ElseIf target.IsNew Then
      'DB code to insert company
      Using connection As New SqlConnection(DataConnection.ConnectionString)
        connection.Open()
        Using command As New SqlCommand("CompaniesInsert", connection)
          command.CommandType = System.Data.CommandType.StoredProcedure
          command.Parameters.Add(New SqlParameter("@companyID", target.CompanyId))
          command.Parameters("@companyID").Direction = System.Data.ParameterDirection.Output
          command.Parameters.Add(New SqlParameter("@companyName", target.CompanyName))
          command.Parameters.Add(New SqlParameter("@dateAdded", target.DateAddedValue))
          command.ExecuteNonQuery()
          target.SetID(CInt(command.Parameters("@companyID").Value))
        End Using
        connection.Close()
      End Using
    Else
      Using connection As New SqlConnection(DataConnection.ConnectionString)
        connection.Open()
        Using command As New SqlCommand("CompaniesUpdate", connection)
          command.CommandType = System.Data.CommandType.StoredProcedure
          command.Parameters.Add(New SqlParameter("@companyID", target.CompanyId))
          command.Parameters.Add(New SqlParameter("@companyName", target.CompanyName))
          command.Parameters.Add(New SqlParameter("@dateAdded", target.DateAddedValue))
          command.ExecuteNonQuery()
        End Using
        connection.Close()
      End Using
    End If
    MarkOld(target)
    Return target
  End Function

End Class
