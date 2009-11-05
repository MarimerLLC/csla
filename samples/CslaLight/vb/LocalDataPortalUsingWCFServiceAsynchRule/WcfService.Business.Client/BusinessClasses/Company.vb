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

<Serializable()> _
Public Class Company
  Inherits BusinessBase(Of Company)


#If Silverlight = 1 Then
  Public Sub New()
  End Sub
#Else
  Private Sub New()
  End Sub
#End If

  Public Shared Function GetServiceUri() As Uri
    Return New Uri("CompanyService.svc", UriKind.Relative)
  End Function


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
    ValidationRules.AddRule(AddressOf IsDuplicateName, New AsyncRuleArgs(CompanyNameProperty, CompanyIdProperty))

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


  Private _context As AsyncValidationRuleContext

  Private Shared Sub IsDuplicateName(ByVal context As AsyncValidationRuleContext)
    Dim client = Csla.Data.ServiceClientManager(Of CompanyServiceReference.CompanyServiceClient, CompanyServiceReference.ICompanyService).GetManager(Constants.ClientName).Client
    RemoveHandler client.IsDuplicateNameCompanyCompleted, AddressOf DuplicateCheckCompleted
    AddHandler client.IsDuplicateNameCompanyCompleted, AddressOf DuplicateCheckCompleted
    client.IsDuplicateNameCompanyAsync(CInt(context.PropertyValues("CompanyId")), CStr(context.PropertyValues("CompanyName")), context)
  End Sub

  Private Shared Sub DuplicateCheckCompleted(ByVal sender As Object, ByVal e As CompanyServiceReference.IsDuplicateNameCompanyCompletedEventArgs)
    Dim context As AsyncValidationRuleContext = CType(e.UserState, AsyncValidationRuleContext)
    If e.Result Then
      context.OutArgs.Result = False
      context.OutArgs.Description = "Duplicate company name."
      context.OutArgs.Severity = RuleSeverity.Error
    Else
      context.OutArgs.Result = True
    End If
    context.Complete()
  End Sub


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

#If Silverlight = 1 Then

  Private _handler As LocalProxy(Of Company).CompletedHandler
  Private _criteria As SingleCriteria(Of Company, Integer)

  <EditorBrowsable(EditorBrowsableState.Never)> _
  Public Sub DataPortal_Fetch(ByVal criteria As SingleCriteria(Of Company, Integer), ByVal handler As LocalProxy(Of Company).CompletedHandler)
    Try
      _handler = handler
      _criteria = criteria
      Dim client = Csla.Data.ServiceClientManager(Of CompanyServiceReference.CompanyServiceClient, CompanyServiceReference.ICompanyService).GetManager(Constants.ClientName).Client
      AddHandler client.GetCompanyCompleted, AddressOf EndFetch
      client.GetCompanyAsync(criteria.Value)
    Catch ex As Exception
      _handler(Nothing, ex)
    End Try

  End Sub

  Private Sub EndFetch(ByVal sender As Object, ByVal e As CompanyServiceReference.GetCompanyCompletedEventArgs)
    Try
      Dim company As CompanyServiceReference.CompanyInfo = e.Result
      If company IsNot Nothing Then
        LoadProperty(CompanyIdProperty, company.CompanyId)
        LoadProperty(CompanyNameProperty, company.CompanyName)
        LoadProperty(DateAddedProperty, New SmartDate(company.DateAdded))
        _handler(Me, Nothing)
      Else
        _handler(Nothing, New ArgumentException("Company with Id of " & _criteria.Value.ToString() & " is not found."))
      End If
    Catch ex As Exception
      _handler(Nothing, ex)
    Finally
      Dim client = Csla.Data.ServiceClientManager(Of CompanyServiceReference.CompanyServiceClient, CompanyServiceReference.ICompanyService).GetManager(Constants.ClientName).Client
      RemoveHandler client.GetCompanyCompleted, AddressOf EndFetch
    End Try

  End Sub

  <EditorBrowsable(EditorBrowsableState.Never)> _
  Public Shadows Sub DataPortal_DeleteSelf(ByVal handler As LocalProxy(Of Company).CompletedHandler)
    If Not IsNew Then
      _handler = handler
      Dim client = Csla.Data.ServiceClientManager(Of CompanyServiceReference.CompanyServiceClient, CompanyServiceReference.ICompanyService).GetManager(Constants.ClientName).Client
      AddHandler client.DeleteCompanyCompleted, AddressOf EndDelete
      client.DeleteCompanyAsync(ReadProperty(CompanyIdProperty))
    Else
      _handler(Me, Nothing)
    End If

  End Sub

  Private Sub EndDelete(ByVal sender As Object, ByVal e As System.ComponentModel.AsyncCompletedEventArgs)
    Try
      If e.Error Is Nothing Then
        _handler(Me, Nothing)
      Else
        _handler(Me, e.Error)
      End If

    Catch ex As Exception
      _handler(Nothing, ex)
    Finally
      Dim client = Csla.Data.ServiceClientManager(Of CompanyServiceReference.CompanyServiceClient, CompanyServiceReference.ICompanyService).GetManager(Constants.ClientName).Client
      RemoveHandler client.DeleteCompanyCompleted, AddressOf EndDelete
    End Try

  End Sub

  <EditorBrowsable(EditorBrowsableState.Never)> _
  Public Shadows Sub DataPortal_Insert(ByVal handler As LocalProxy(Of Company).CompletedHandler)
    Try
      _handler = handler
      Dim client = Csla.Data.ServiceClientManager(Of CompanyServiceReference.CompanyServiceClient, CompanyServiceReference.ICompanyService).GetManager(Constants.ClientName).Client
      Dim newCompany As CompanyServiceReference.CompanyInfo = New CompanyServiceReference.CompanyInfo()
      With newCompany
        .CompanyId = ReadProperty(CompanyIdProperty)
        .CompanyName = ReadProperty(CompanyNameProperty)
        .DateAdded = ReadProperty(DateAddedProperty).Text
      End With
      AddHandler client.InsertCompanyCompleted, AddressOf EndInsert
      client.InsertCompanyAsync(newCompany)
    Catch ex As Exception
      _handler(Me, ex)
    End Try

  End Sub

  Private Sub EndInsert(ByVal sender As Object, ByVal e As CompanyServiceReference.InsertCompanyCompletedEventArgs)
    Try
      Dim client = Csla.Data.ServiceClientManager(Of CompanyServiceReference.CompanyServiceClient, CompanyServiceReference.ICompanyService).GetManager(Constants.ClientName).Client
      If e.Error Is Nothing Then
        Dim newId As Integer = e.Result
        If newId > 0 Then
          LoadProperty(CompanyIdProperty, newId)
          MarkOld()
          _handler(Me, Nothing)
        Else
          _handler(Me, New ArgumentException("Cannot insert company."))
        End If
      Else
        _handler(Me, e.Error)
      End If

    Catch ex As Exception
      _handler(Nothing, ex)
    Finally
      Dim client = Csla.Data.ServiceClientManager(Of CompanyServiceReference.CompanyServiceClient, CompanyServiceReference.ICompanyService).GetManager(Constants.ClientName).Client
      RemoveHandler client.InsertCompanyCompleted, AddressOf EndInsert
    End Try

  End Sub

  <EditorBrowsable(EditorBrowsableState.Never)> _
  Public Shadows Sub DataPortal_Update(ByVal handler As LocalProxy(Of Company).CompletedHandler)

    Try
      _handler = handler
      Dim client = Csla.Data.ServiceClientManager(Of CompanyServiceReference.CompanyServiceClient, CompanyServiceReference.ICompanyService).GetManager(Constants.ClientName).Client
      Dim existingCompany As CompanyServiceReference.CompanyInfo = New CompanyServiceReference.CompanyInfo()
      With existingCompany
        .CompanyId = ReadProperty(CompanyIdProperty)
        .CompanyName = ReadProperty(CompanyNameProperty)
        .DateAdded = ReadProperty(DateAddedProperty).Text
      End With
      AddHandler client.UpdateCompanyCompleted, AddressOf EndUpdate
      client.UpdateCompanyAsync(existingCompany)
    Catch ex As Exception
      _handler(Me, ex)
    End Try

  End Sub

  Private Sub EndUpdate(ByVal sender As Object, ByVal e As System.ComponentModel.AsyncCompletedEventArgs)
    Try
      If e.Error Is Nothing Then
        MarkOld()
        _handler(Me, Nothing)
      Else
        _handler(Me, e.Error)
      End If

    Catch ex As Exception
      _handler(Nothing, ex)
    Finally
      Dim client = Csla.Data.ServiceClientManager(Of CompanyServiceReference.CompanyServiceClient, CompanyServiceReference.ICompanyService).GetManager(Constants.ClientName).Client
      RemoveHandler client.UpdateCompanyCompleted, AddressOf EndUpdate
    End Try

  End Sub

#End If

End Class
