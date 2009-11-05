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
Imports System.Data

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

#If Silverlight = 1 Then

  Private _handler As LocalProxy(Of Company).CompletedHandler
  Private _criteria As SingleCriteria(Of Company, Integer)

  <EditorBrowsable(EditorBrowsableState.Never)> _
  Public Sub DataPortal_Fetch(ByVal criteria As SingleCriteria(Of Company, Integer), ByVal handler As LocalProxy(Of Company).CompletedHandler)
    Try
      _handler = handler
      _criteria = criteria
      Dim context As CompanyServiceReference.CompanyEntities = Csla.Data.DataServiceContextManager(Of CompanyServiceReference.CompanyEntities).GetManager(GetServiceUri).DataServiceContext

      Dim query = From oneCompany In context.Companies _
                  Where oneCompany.CompanyId = criteria.Value _
                  Select oneCompany
      Dim companyQuery As Services.Client.DataServiceQuery(Of CompanyServiceReference.Companies) = CType(query, Global.System.Data.Services.Client.DataServiceQuery(Of Global.DataServices.Business.CompanyServiceReference.Companies))
      companyQuery.BeginExecute(AddressOf EndFetch, companyQuery)
    Catch ex As Exception
      _handler(Nothing, ex)
      _handler = Nothing
      _criteria = Nothing
    End Try

  End Sub

  Private Sub EndFetch(ByVal e As IAsyncResult)
    Try
      Dim queryCompany = CType(e.AsyncState, Services.Client.DataServiceQuery(Of CompanyServiceReference.Companies))
      If queryCompany IsNot Nothing Then
        Dim aCompany = queryCompany.EndExecute(e).FirstOrDefault()
        If aCompany IsNot Nothing Then
          LoadProperty(CompanyIdProperty, aCompany.CompanyId)
          LoadProperty(CompanyNameProperty, aCompany.CompanyName)
          LoadProperty(DateAddedProperty, New SmartDate(aCompany.DateAdded))
          _handler(Me, Nothing)
        Else
          _handler(Nothing, New ArgumentException("Company with Id of " & _criteria.Value.ToString() & " is not found."))
        End If
      Else
        _handler(Nothing, New ArgumentException("Company with Id of " & _criteria.Value.ToString() & " is not found."))
      End If
    Catch ex As Exception
      _handler(Nothing, ex)
    Finally
      _handler = Nothing
      _criteria = Nothing
    End Try

  End Sub

  <EditorBrowsable(EditorBrowsableState.Never)> _
  Public Shadows Sub DataPortal_DeleteSelf(ByVal handler As LocalProxy(Of Company).CompletedHandler)
    If Not IsNew Then
      _handler = handler
      Dim context As CompanyServiceReference.CompanyEntities = Csla.Data.DataServiceContextManager(Of CompanyServiceReference.CompanyEntities).GetManager(GetServiceUri).DataServiceContext
      Dim company As CompanyServiceReference.Companies = GetCompany()
      context.DeleteObject(company)
      context.BeginSaveChanges(Services.Client.SaveChangesOptions.Batch, AddressOf EndUpdate, Nothing)
    Else
      _handler(Me, Nothing)
    End If

  End Sub

  <EditorBrowsable(EditorBrowsableState.Never)> _
  Public Shadows Sub DataPortal_Insert(ByVal handler As LocalProxy(Of Company).CompletedHandler)
    Try
      _handler = handler
      Dim context As CompanyServiceReference.CompanyEntities = Csla.Data.DataServiceContextManager(Of CompanyServiceReference.CompanyEntities).GetManager(GetServiceUri).DataServiceContext
      Dim company As CompanyServiceReference.Companies = New CompanyServiceReference.Companies
      With company
        .CompanyId = ReadProperty(CompanyIdProperty)
        .CompanyName = ReadProperty(CompanyNameProperty)
        .DateAdded = ReadProperty(DateAddedProperty)
      End With
      context.AddToCompanies(company)
      context.BeginSaveChanges(Services.Client.SaveChangesOptions.Batch, AddressOf EndUpdate, Nothing)
    Catch ex As Exception
      _handler(Me, ex)
    End Try

  End Sub

  <EditorBrowsable(EditorBrowsableState.Never)> _
  Public Shadows Sub DataPortal_Update(ByVal handler As LocalProxy(Of Company).CompletedHandler)

    Try
      If IsDirty Then
        _handler = handler
        Dim context As CompanyServiceReference.CompanyEntities = Csla.Data.DataServiceContextManager(Of CompanyServiceReference.CompanyEntities).GetManager(GetServiceUri).DataServiceContext
        Dim company As CompanyServiceReference.Companies = GetCompany()
        With company
          .CompanyId = ReadProperty(CompanyIdProperty)
          .CompanyName = ReadProperty(CompanyNameProperty)
          .DateAdded = ReadProperty(DateAddedProperty)
        End With
        context.UpdateObject(company)
        context.BeginSaveChanges(Services.Client.SaveChangesOptions.Batch, AddressOf EndUpdate, Nothing)
      Else
        _handler(Me, Nothing)
      End If
    Catch ex As Exception
      _handler(Me, ex)
    End Try

  End Sub

  Private Sub EndUpdate(ByVal e As IAsyncResult)
    Try
      Dim context As CompanyServiceReference.CompanyEntities = Csla.Data.DataServiceContextManager(Of CompanyServiceReference.CompanyEntities).GetManager(GetServiceUri).DataServiceContext
      Dim response As Services.Client.DataServiceResponse = context.EndSaveChanges(e)
      Dim anError As Exception = Nothing
      For Each oneResponse As Services.Client.OperationResponse In response
        If oneResponse.Error IsNot Nothing Then
          anError = oneResponse.Error
        Else
          If Not IsDeleted Then
            Dim details As System.Data.Services.Client.ChangeOperationResponse = CType(oneResponse, Services.Client.ChangeOperationResponse)
            Dim company As CompanyServiceReference.Companies = CType(CType(details.Descriptor, Services.Client.EntityDescriptor).Entity, CompanyServiceReference.Companies)
            LoadProperty(CompanyIdProperty, company.CompanyId)
          End If
        End If
      Next
      If anError Is Nothing Then
        If Not IsDeleted Then
          MarkOld()
        End If
        _handler(Me, Nothing)
        _handler = Nothing
      Else
        _handler(Me, anError)
      End If
    Catch ex As Exception
      _handler(Me, ex)
    End Try

  End Sub

  Private Function GetCompany() As CompanyServiceReference.Companies
    Dim returnValue As CompanyServiceReference.Companies = Nothing
    Dim manager = Csla.Data.DataServiceContextManager(Of CompanyServiceReference.CompanyEntities).GetManager(GetServiceUri)
    returnValue = manager.GetEntity(Of CompanyServiceReference.Companies)(CompanyIdProperty.Name, ReadProperty(CompanyIdProperty))
    Return returnValue
  End Function

#End If

End Class
