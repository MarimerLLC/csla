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

#If(Not SILVERLIGHT) Then
Imports Csla.Data
Imports System.Data.SqlClient
Imports Rolodex.Business.Data
#End If

Namespace Rolodex.Business.BusinessClasses
  <Serializable> _
  Public Class ReadOnlyCompanyList
	  Inherits ReadOnlyListBase(Of ReadOnlyCompanyList,ReadOnlyCompany)
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

#If (Not SILVERLIGHT) Then

	Private Sub DataPortal_Fetch()
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
