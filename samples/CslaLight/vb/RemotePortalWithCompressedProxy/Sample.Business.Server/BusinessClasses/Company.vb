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
Imports Csla.Validation

#If (Not SILVERLIGHT) Then
Imports System.Data.SqlClient
#End If

Namespace Sample.Business
  <Serializable()> _
  Public Class Company
	  Inherits BusinessBase(Of Company)


#If SILVERLIGHT Then
	Public Sub New()
	End Sub
#Else
	Private Sub New()
	End Sub
#End If

	Private Shared CompanyIdProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(New PropertyInfo(Of Integer)("CompanyId", "Company Id", 0))
	Public ReadOnly Property CompanyId() As Integer
	  Get
		Return GetProperty(CompanyIdProperty)
	  End Get
	End Property

	Private Shared CompanyNameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(New PropertyInfo(Of String)("CompanyName", "Company Name", String.Empty))
	Public Property CompanyName() As String
	  Get
		Return GetProperty(CompanyNameProperty)
	  End Get
	  Set(ByVal value As String)
		SetProperty(CompanyNameProperty, value)
	  End Set
	End Property

	Private Shared DateAddedProperty As PropertyInfo(Of SmartDate) = RegisterProperty(Of SmartDate)(New PropertyInfo(Of SmartDate)("DateAdded", "Date Added"))
	Public Property DateAdded() As String
	  Get
		Return GetProperty(DateAddedProperty).Text
	  End Get
	  Set(ByVal value As String)
		Dim test As New SmartDate()
		If SmartDate.TryParse(value, test) Then
		  SetProperty(DateAddedProperty, test)
		End If
	  End Set
	End Property

	Friend ReadOnly Property DateAddedValue() As Object
	  Get
		Return GetProperty(DateAddedProperty).DBValue
	  End Get
	End Property

	Protected Overrides Sub AddAuthorizationRules()
	  MyBase.AddAuthorizationRules()
	  Dim canWrite() As String = { "AdminUser", "RegularUser" }
	  Dim canRead() As String = { "AdminUser", "RegularUser", "ReadOnlyUser" }
	  Dim admin() As String = { "AdminUser" }
	  AuthorizationRules.AllowCreate(GetType(Company), admin)
	  AuthorizationRules.AllowDelete(GetType(Company), admin)
	  AuthorizationRules.AllowEdit(GetType(Company), canWrite)
	  AuthorizationRules.AllowGet(GetType(Company), canRead)
	  AuthorizationRules.AllowWrite(CompanyNameProperty, canWrite)
	  AuthorizationRules.AllowWrite(DateAddedProperty, canWrite)
	  AuthorizationRules.AllowRead(CompanyNameProperty, canRead)
	  AuthorizationRules.AllowRead(CompanyIdProperty, canRead)
	  AuthorizationRules.AllowRead(DateAddedProperty, canRead)
	End Sub

	Protected Overrides Sub AddBusinessRules()
	  MyBase.AddBusinessRules()
      ValidationRules.AddRule(AddressOf CommonRules.StringRequired, New RuleArgs(CompanyNameProperty))
      ValidationRules.AddRule(AddressOf CommonRules.StringMaxLength, New CommonRules.MaxLengthRuleArgs(CompanyNameProperty, 50))
	  ValidationRules.AddRule(Of Company)(AddressOf IsDateValid, DateAddedProperty)

	End Sub

	Private Shared Function IsDateValid(ByVal target As Company, ByVal e As RuleArgs) As Boolean
	  Dim dateAdded As SmartDate = target.GetProperty(DateAddedProperty)
	  If (Not dateAdded.IsEmpty) Then
		If dateAdded.Date < New System.DateTime(2000, 1, 1) Then
		  e.Description = "Date must be greater that 1/1/2000!"
		  Return False
		ElseIf dateAdded.Date > DateTime.Today Then
		  e.Description = "Date cannot be greater than today!"
		  Return False
		End If
	  Else
		e.Description = "Date added is required!"
		Return False
	  End If
	  Return True
	End Function

	Public Shared Sub GetCompany(ByVal companyId As Integer, ByVal handler As EventHandler(Of DataPortalResult(Of Company)))
	  Dim dp As DataPortal(Of Company) = New DataPortal(Of Company)()
	  AddHandler dp.FetchCompleted, handler
	  dp.BeginFetch(New SingleCriteria(Of Company, Integer)(companyId))
	End Sub

	Public Shared Sub CreateCompany(ByVal handler As EventHandler(Of DataPortalResult(Of Company)))
	  Dim dp As DataPortal(Of Company) = New DataPortal(Of Company)()
	  AddHandler dp.CreateCompleted, handler
	  dp.BeginCreate()
	End Sub



#If (Not SILVERLIGHT) Then

	Protected Sub DataPortal_Fetch(ByVal criteria As SingleCriteria(Of Company, Integer))
	  Using connection As New SqlConnection(DataConnection.ConnectionString)
		connection.Open()
		Using command As New SqlCommand("GetCompany", connection)
		  command.CommandType = System.Data.CommandType.StoredProcedure
		  command.Parameters.Add(New SqlParameter("@companyID", criteria.Value))
		  Using reader As New Csla.Data.SafeDataReader(command.ExecuteReader())
			If reader.Read() Then
			  LoadProperty(Of Integer)(CompanyIdProperty, reader.GetInt32("CompanyID"))
			  LoadProperty(Of String)(CompanyNameProperty, reader.GetString("CompanyName"))
			  LoadProperty(Of SmartDate)(DateAddedProperty, reader.GetSmartDate("DateAdded"))
			End If
		  End Using
		End Using
		connection.Close()
	  End Using
	End Sub

	Protected Sub DataPortal_Create()
	  ValidationRules.CheckRules()
	End Sub


	<Transactional(TransactionalTypes.TransactionScope)> _
	Private Sub DataPortal_DeleteSelf()
	  Using connection As New SqlConnection(DataConnection.ConnectionString)
		connection.Open()
		Using command As New SqlCommand("CompaniesDelete", connection)
		  command.CommandType = System.Data.CommandType.StoredProcedure
		  command.Parameters.Add(New SqlParameter("@companyID", ReadProperty(CompanyIdProperty)))
		  command.ExecuteNonQuery()
		End Using
		connection.Close()
	  End Using
	End Sub

	<Transactional(TransactionalTypes.TransactionScope)> _
	Private Sub DataPortal_Insert()
	  Using connection As New SqlConnection(DataConnection.ConnectionString)
		connection.Open()
		Using command As New SqlCommand("CompaniesInsert", connection)
		  command.CommandType = System.Data.CommandType.StoredProcedure
		  command.Parameters.Add(New SqlParameter("@companyID", ReadProperty(CompanyIdProperty)))
		  command.Parameters("@companyID").Direction = System.Data.ParameterDirection.Output
		  command.Parameters.Add(New SqlParameter("@companyName", ReadProperty(CompanyNameProperty)))
		  command.Parameters.Add(New SqlParameter("@dateAdded", ReadProperty(DateAddedProperty).DBValue))
		  command.ExecuteNonQuery()
		  LoadProperty(CompanyIdProperty, command.Parameters("@companyID").Value)
		End Using
		connection.Close()
	  End Using
	End Sub

	<Transactional(TransactionalTypes.TransactionScope)> _
	Private Sub DataPortal_Update()
	  Using connection As New SqlConnection(DataConnection.ConnectionString)
		connection.Open()
		Using command As New SqlCommand("CompaniesUpdate", connection)
		  command.CommandType = System.Data.CommandType.StoredProcedure
		  command.Parameters.Add(New SqlParameter("@companyID", ReadProperty(CompanyIdProperty)))
		  command.Parameters.Add(New SqlParameter("@companyName", ReadProperty(CompanyNameProperty)))
		  command.Parameters.Add(New SqlParameter("@dateAdded", ReadProperty(DateAddedProperty).DBValue))
		  command.ExecuteNonQuery()
		End Using
		connection.Close()
	  End Using
	End Sub
#End If

  End Class

End Namespace 'end of root namespace