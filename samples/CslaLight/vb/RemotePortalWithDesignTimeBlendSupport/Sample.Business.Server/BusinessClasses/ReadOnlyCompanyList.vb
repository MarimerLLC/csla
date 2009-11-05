Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports Csla
Imports Csla.Security
Imports Csla.Core
Imports Csla.Serialization
Imports Csla.Silverlight
Imports Csla.Validation
Imports Sample.Business

#If (Not SILVERLIGHT) Then
Imports Csla.Data
Imports System.Data.SqlClient
#End If

Namespace Sample.Business
  <Serializable()> _
  Public Class ReadOnlyCompanyList
    Inherits ReadOnlyListBase(Of ReadOnlyCompanyList, ReadOnlyCompany)
#If SILVERLIGHT Then
    Public Sub New()
    End Sub
#Else
    Private Sub New()
    End Sub
#End If

    Public Shared Sub GetCompanyList(ByVal handler As EventHandler(Of DataPortalResult(Of ReadOnlyCompanyList)))
      Dim dp As DataPortal(Of ReadOnlyCompanyList) = New DataPortal(Of ReadOnlyCompanyList)()
      AddHandler dp.FetchCompleted, handler
      dp.BeginFetch()
    End Sub

    <ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
    Public Function DesignTime_Create() As ReadOnlyCompanyList
      Dim returnValue As New ReadOnlyCompanyList
      returnValue.IsReadOnly = False
      Dim company As ReadOnlyCompany
      company = ReadOnlyCompany.DesignTime_Create("Sample Company 1")
      returnValue.Add(company)
      company = ReadOnlyCompany.DesignTime_Create("Sample Company 2")
      returnValue.Add(company)
      returnValue.IsReadOnly = True
      Return returnValue
    End Function

#If (Not Silverlight) Then

    Private Shadows Sub DataPortal_Fetch()
      RaiseListChangedEvents = False
      IsReadOnly = False
      Using connection As New SqlConnection(DataConnection.ConnectionString)
        connection.Open()
        Using command As New SqlCommand("GetCompanies", connection)
          command.CommandType = System.Data.CommandType.StoredProcedure
          Using reader As New Csla.Data.SafeDataReader(command.ExecuteReader())
            Do While reader.Read()
              Add(ReadOnlyCompany.GetReadOnlyCompany(reader))
            Loop
          End Using
        End Using
        connection.Close()
      End Using
      IsReadOnly = True
      RaiseListChangedEvents = True
    End Sub

#End If
  End Class
End Namespace
