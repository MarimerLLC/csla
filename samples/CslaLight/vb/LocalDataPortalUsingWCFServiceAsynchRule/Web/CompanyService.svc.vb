Imports System.Data.SqlClient

' NOTE: If you change the class name "CompanyService" here, you must also update the reference to "CompanyService" in Web.config and in the associated .svc file.
Public Class CompanyService
  Implements ICompanyService

  Public Sub DeleteCompany(ByVal companyId As Integer) Implements ICompanyService.DeleteCompany
    Using connection As New SqlConnection(DataConnection.ConnectionString)
      connection.Open()
      Using command As New SqlCommand("CompaniesDelete", connection)
        command.CommandType = System.Data.CommandType.StoredProcedure
        command.Parameters.Add(New SqlParameter("@companyID", companyId))
        command.ExecuteNonQuery()
      End Using
      connection.Close()
    End Using
  End Sub

  Public Function GetCompany(ByVal companyId As Integer) As CompanyInfo Implements ICompanyService.GetCompany
    Dim returnValue As CompanyInfo = Nothing
    Using connection As New SqlConnection(DataConnection.ConnectionString)
      connection.Open()
      Using command As New SqlCommand("GetCompany", connection)
        command.CommandType = System.Data.CommandType.StoredProcedure
        command.Parameters.Add(New SqlParameter("@companyID", companyId))
        Using reader As New Csla.Data.SafeDataReader(command.ExecuteReader())
          If reader.Read() Then
            returnValue = New CompanyInfo(reader.GetInt32("CompanyID"), reader.GetString("CompanyName"), reader.GetSmartDate("DateAdded").Text)
          End If
        End Using
      End Using
      connection.Close()
    End Using
    Return returnValue
  End Function

  Public Function InsertCompany(ByVal company As CompanyInfo) As Integer Implements ICompanyService.InsertCompany
    Dim returnValue As Integer
    Using connection As New SqlConnection(DataConnection.ConnectionString)
      connection.Open()
      Using command As New SqlCommand("CompaniesInsert", connection)
        command.CommandType = System.Data.CommandType.StoredProcedure
        command.Parameters.Add(New SqlParameter("@companyID", company.CompanyId))
        command.Parameters("@companyID").Direction = System.Data.ParameterDirection.Output
        command.Parameters.Add(New SqlParameter("@companyName", company.CompanyName))
        command.Parameters.Add(New SqlParameter("@dateAdded", company.DateAddedValue))
        command.ExecuteNonQuery()
        returnValue = CInt(command.Parameters("@companyID").Value)
      End Using
      connection.Close()
    End Using
    Return returnValue
  End Function

  Public Sub UpdateCompany(ByVal company As CompanyInfo) Implements ICompanyService.UpdateCompany
    Using connection As New SqlConnection(DataConnection.ConnectionString)
      connection.Open()
      Using command As New SqlCommand("CompaniesUpdate", connection)
        command.CommandType = System.Data.CommandType.StoredProcedure
        command.Parameters.Add(New SqlParameter("@companyID", company.CompanyId))
        command.Parameters.Add(New SqlParameter("@companyName", company.CompanyName))
        command.Parameters.Add(New SqlParameter("@dateAdded", company.DateAddedValue))
        command.ExecuteNonQuery()
      End Using
      connection.Close()
    End Using
  End Sub

  Public Function GetUser(ByVal userName As String, ByVal password As String) As UserInfo Implements ICompanyService.GetUser
    Dim returnValue As New UserInfo("", "", False)
    Using connection As New SqlConnection(DataConnection.ConnectionString)
      connection.Open()
      Using command As New SqlCommand("GetUser", connection)
        command.CommandType = System.Data.CommandType.StoredProcedure
        command.Parameters.Add(New SqlParameter("@userName", userName))
        Using reader As Csla.Data.SafeDataReader = New Csla.Data.SafeDataReader(command.ExecuteReader())
          If reader.Read() Then
            If password = reader.GetString("Password") Then
              returnValue = New UserInfo(reader.GetString(1), reader.GetString("Role"), True)
            End If
          End If
        End Using
      End Using
    End Using
    Return returnValue
  End Function

  Public Function IsDuplicateNameCompany(ByVal companyId As Integer, ByVal companyName As String) As Boolean Implements ICompanyService.IsDuplicateNameCompany
    Dim returnValue As Boolean = False
    Using connection As New SqlConnection(DataConnection.ConnectionString)
      connection.Open()
      Using command As New SqlCommand("IsDuplicateCompany", connection)
        command.CommandType = System.Data.CommandType.StoredProcedure
        command.Parameters.Add(New SqlParameter("@companyID", companyId))
        command.Parameters.Add(New SqlParameter("@companyName", companyName))
        Dim isDuplicateParameter As SqlParameter = New SqlParameter("@isDuplicate", False)
        isDuplicateParameter.Direction = System.Data.ParameterDirection.Output
        command.Parameters.Add(isDuplicateParameter)
        command.ExecuteNonQuery()
        returnValue = CBool(isDuplicateParameter.Value)
      End Using
      connection.Close()
    End Using
    Return returnValue
  End Function
End Class
