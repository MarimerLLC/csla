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

#If Not SILVERLIGHT = 1 Then
Imports System.Data.SqlClient
#End If

<Serializable(), Csla.Server.ObjectFactory("Sample.Business.CompanyFactory, Sample.Business", "CreateCompany", "GetCompany", "SaveCompany", "")> _
Public Class Company
  Inherits BusinessBase(Of Company)


#If Silverlight = 1 Then
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
      Dim test As SmartDate = New SmartDate()
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
    Dim canWrite() As String = New String() {"AdminUser", "RegularUser"}
    Dim canRead() As String = New String() {"AdminUser", "RegularUser", "ReadOnlyUser"}
    Dim admin() As String = New String() {"AdminUser"}
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
    If Not dateAdded.IsEmpty Then
      If (dateAdded.Date < New DateTime(2000, 1, 1)) Then
        e.Description = "Date must be greater that 1/1/2000!"
        Return False
      ElseIf (dateAdded.Date > DateTime.Today) Then
        e.Description = "Date cannot be greater than today!"
        Return False
      End If
    Else
      e.Description = "Date added is required!"
      Return False
    End If
    Return True
  End Function

  Friend Sub SetID(ByVal companyId As Integer)
    LoadProperty(CompanyIdProperty, companyId)
  End Sub

  Public Shared Sub GetCompany(ByVal companyId As Integer, ByVal handler As EventHandler(Of DataPortalResult(Of Company)))
    Dim dp As New DataPortal(Of Company)()
    AddHandler dp.FetchCompleted, handler
    dp.BeginFetch(New SingleCriteria(Of Company, Integer)(companyId))
  End Sub

  Public Shared Sub CreateCompany(ByVal handler As EventHandler(Of DataPortalResult(Of Company)))
    Dim dp As New DataPortal(Of Company)()
    AddHandler dp.CreateCompleted, handler
    dp.BeginCreate()
  End Sub

  Friend Shared Function LoadCompany(ByVal companyId As Integer, ByVal companyName As String, ByVal dateAdded As SmartDate) As Company
    Dim newCompany As New Company
    newCompany.LoadProperty(CompanyIdProperty, companyId)
    newCompany.LoadProperty(CompanyNameProperty, companyName)
    newCompany.LoadProperty(DateAddedProperty, dateAdded)
    Return newCompany
  End Function

  Friend Shared Function NewCompany() As Company
    Dim aCompany As New Company
    aCompany.ValidationRules.CheckRules()
    Return aCompany
  End Function


End Class
