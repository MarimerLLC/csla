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
Imports System.ComponentModel


#If (Not SILVERLIGHT) Then
Imports System.Data.SqlClient
Imports Rolodex.Business.Data
Imports Csla.Data
#End If

Namespace Rolodex.Business.BusinessClasses
  <Serializable()> _
  Public Class CompanyContact
    Inherits BusinessBase(Of CompanyContact)
#If SILVERLIGHT Then
    Public Sub New()
      MarkAsChild()
      DisableIEditableObject = True
    End Sub
#Else
    Private Sub New()
      MarkAsChild()
      DisableIEditableObject = True
    End Sub
#End If

    Private Shared CompanyContactIdProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(New PropertyInfo(Of Integer)("CompanyContactId", "Contact Id", 0))
    Public ReadOnly Property CompanyContactId() As Integer
      Get
        Return GetProperty(CompanyContactIdProperty)
      End Get
    End Property

    Private Shared CompanyIdProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(New PropertyInfo(Of Integer)("CompanyId", "Company Id", 0))
    Public Property CompanyId() As Integer
      Get
        Return GetProperty(CompanyIdProperty)
      End Get
      Set(ByVal value As Integer)
        SetProperty(CompanyIdProperty, value)
      End Set
    End Property

    Private Shared FirstNameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(New PropertyInfo(Of String)("FirstName", "First Name", String.Empty))
    Public Property FirstName() As String
      Get
        Return GetProperty(FirstNameProperty)
      End Get
      Set(ByVal value As String)
        SetProperty(FirstNameProperty, value)
      End Set
    End Property

    Private Shared LastNameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(New PropertyInfo(Of String)("LastName", "Last Name", String.Empty))
    Public Property LastName() As String
      Get
        Return GetProperty(LastNameProperty)
      End Get
      Set(ByVal value As String)
        SetProperty(LastNameProperty, value)
      End Set
    End Property

    Private Shared RankIdProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(New PropertyInfo(Of Integer)("RankId", "Rank", 0))
    Public Property RankId() As Integer
      Get
        Return GetProperty(RankIdProperty)
      End Get
      Set(ByVal value As Integer)
        SetProperty(RankIdProperty, value)
      End Set
    End Property

    Private Shared BirthdayProperty As PropertyInfo(Of SmartDate) = RegisterProperty(Of SmartDate)(New PropertyInfo(Of SmartDate)("Birthday", "Birthday"))
    Public Property Birthday() As String
      Get
        Return GetProperty(BirthdayProperty).Text
      End Get
      Set(ByVal value As String)
        Dim test As New SmartDate()
        If SmartDate.TryParse(value, test) = True Then
          SetProperty(BirthdayProperty, test)
        End If
      End Set
    End Property

    Private Shared BaseSalaryProperty As PropertyInfo(Of Decimal) = RegisterProperty(Of Decimal)(New PropertyInfo(Of Decimal)("BaseSalary", "Base Salary", 0))
    Public Property BaseSalary() As Decimal
      Get
        Return GetProperty(BaseSalaryProperty)
      End Get
      Set(ByVal value As Decimal)
        SetProperty(BaseSalaryProperty, value)
      End Set
    End Property

    Private Shared ContactsPhonesProperty As PropertyInfo(Of CompanyContactPhoneList) = RegisterProperty(Of CompanyContactPhoneList)(New PropertyInfo(Of CompanyContactPhoneList)("ContactPhones", "Contact Phones"))
    Public ReadOnly Property ContactPhones() As CompanyContactPhoneList
      Get
        Return GetProperty(ContactsPhonesProperty)
      End Get
    End Property

    Protected Overrides Sub AddAuthorizationRules()
      Dim canWrite() As String = {"AdminUser", "RegularUser"}
      Dim canRead() As String = {"AdminUser", "RegularUser", "ReadOnlyUser"}
      Dim admin() As String = {"AdminUser"}

      AuthorizationRules.AllowCreate(GetType(CompanyContact), admin)
      AuthorizationRules.AllowDelete(GetType(CompanyContact), admin)
      AuthorizationRules.AllowEdit(GetType(CompanyContact), canWrite)
      AuthorizationRules.AllowGet(GetType(CompanyContact), canRead)

      AuthorizationRules.AllowWrite(FirstNameProperty, canWrite)
      AuthorizationRules.AllowWrite(LastNameProperty, canWrite)
      AuthorizationRules.AllowWrite(CompanyContactIdProperty, canWrite)
      AuthorizationRules.AllowWrite(CompanyIdProperty, canWrite)
      AuthorizationRules.AllowWrite(RankIdProperty, canWrite)
      AuthorizationRules.AllowWrite(BirthdayProperty, canWrite)
      AuthorizationRules.AllowWrite(BaseSalaryProperty, canWrite)

      AuthorizationRules.AllowRead(FirstNameProperty, canRead)
      AuthorizationRules.AllowRead(LastNameProperty, canRead)
      AuthorizationRules.AllowRead(CompanyContactIdProperty, canRead)
      AuthorizationRules.AllowRead(CompanyIdProperty, canRead)
      AuthorizationRules.AllowRead(RankIdProperty, canRead)
      AuthorizationRules.AllowRead(BirthdayProperty, canRead)
      AuthorizationRules.AllowRead(BaseSalaryProperty, canRead)
      AuthorizationRules.AllowRead(ContactsPhonesProperty, canRead)
    End Sub

    Protected Overrides Sub AddBusinessRules()
      ValidationRules.AddRule(AddressOf Csla.Validation.CommonRules.StringRequired, New Csla.Validation.RuleArgs(FirstNameProperty))
      ValidationRules.AddRule(AddressOf Csla.Validation.CommonRules.StringRequired, New Csla.Validation.RuleArgs(LastNameProperty))
      ValidationRules.AddRule(AddressOf Csla.Validation.CommonRules.StringMaxLength, New Csla.Validation.CommonRules.MaxLengthRuleArgs(FirstNameProperty, 30))
      ValidationRules.AddRule(AddressOf Csla.Validation.CommonRules.StringMaxLength, New Csla.Validation.CommonRules.MaxLengthRuleArgs(LastNameProperty, 50))
      ValidationRules.AddRule(Of CompanyContact)(AddressOf IsDateValid, BirthdayProperty)
      ValidationRules.AddRule(AddressOf Csla.Validation.CommonRules.IntegerMinValue, New Csla.Validation.CommonRules.IntegerMinValueRuleArgs(RankIdProperty, 1))
    End Sub

    Private Shared Function IsDateValid(ByVal target As CompanyContact, ByVal e As RuleArgs) As Boolean
      Dim dateAdded As SmartDate = target.GetProperty(BirthdayProperty)
      If (Not dateAdded.IsEmpty) Then
        If dateAdded.Date < (New DateTime(1900, 1, 1)) Then
          e.Description = "Date must be greater that 1/1/1900!"
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

    Friend Shared Function NewCompanyContact() As CompanyContact
      Dim newContact As New CompanyContact()
      newContact.LoadProperty(ContactsPhonesProperty, CompanyContactPhoneList.NewCompanyContactPhoneList())
      newContact.MarkAsChild()
      newContact.ValidationRules.CheckRules()
      Return newContact
    End Function


#If (Not Silverlight) Then

    Friend Shared Function GetCompanyContact(ByVal reader As SafeDataReader) As CompanyContact
      Return DataPortal.FetchChild(Of CompanyContact)(reader)
    End Function

    Private Sub Child_Fetch(ByVal reader As SafeDataReader)
      LoadProperty(Of Integer)(CompanyIdProperty, reader.GetInt32("CompanyId"))
      LoadProperty(Of Integer)(CompanyContactIdProperty, reader.GetInt32("CompanyContactId"))
      LoadProperty(Of String)(FirstNameProperty, reader.GetString("FirstName"))
      LoadProperty(Of String)(LastNameProperty, reader.GetString("LastName"))
      LoadProperty(Of SmartDate)(BirthdayProperty, reader.GetSmartDate("Birthday"))
      LoadProperty(Of Integer)(RankIdProperty, reader.GetInt32("RankId"))
      LoadProperty(Of Decimal)(BaseSalaryProperty, reader.GetDecimal("BaseSalary"))
      LoadProperty(ContactsPhonesProperty, CompanyContactPhoneList.NewCompanyContactPhoneList())
    End Sub

    Private Sub Child_Insert(ByVal company As Company, ByVal connection As SqlConnection)
      InsertUpdate(True, connection, company)
    End Sub

    Private Sub InsertUpdate(ByVal insert As Boolean, ByVal connection As SqlConnection, ByVal company As Company)
      Using command As New SqlCommand("CompanyContactsUpdate", connection)
        command.CommandType = System.Data.CommandType.StoredProcedure
        command.Parameters.Add(New SqlParameter("@companyContactId", ReadProperty(CompanyContactIdProperty)))
        If insert Then
          command.Parameters("@companyContactId").Direction = System.Data.ParameterDirection.Output
          LoadProperty(Of Integer)(CompanyIdProperty, company.CompanyId)
          command.CommandText = "CompanyContactsInsert"
        End If

        command.Parameters.Add(New SqlParameter("@companyId", ReadProperty(CompanyIdProperty)))
        command.Parameters.Add(New SqlParameter("@firstName", ReadProperty(FirstNameProperty)))
        command.Parameters.Add(New SqlParameter("@lastName", ReadProperty(LastNameProperty)))
        command.Parameters.Add(New SqlParameter("@baseSalary", ReadProperty(BaseSalaryProperty)))
        command.Parameters.Add(New SqlParameter("@rankId", ReadProperty(RankIdProperty)))
        command.Parameters.Add(New SqlParameter("@birthday", ReadProperty(BirthdayProperty).DBValue))

        command.ExecuteNonQuery()
        If insert Then
          LoadProperty(CompanyContactIdProperty, command.Parameters("@companyContactId").Value)
        End If
        DataPortal.UpdateChild(ReadProperty(ContactsPhonesProperty), Me, connection)
        MarkOld()
      End Using
    End Sub

    Private Sub Child_Update(ByVal company As Company, ByVal connection As SqlConnection)
      InsertUpdate(False, connection, company)
    End Sub

    Private Sub Child_DeleteSelf(ByVal company As Company, ByVal connection As SqlConnection)
      Using command As New SqlCommand("CompanyContactsDelete", connection)
        command.CommandType = System.Data.CommandType.StoredProcedure
        command.Parameters.Add(New SqlParameter("@companyContactId", ReadProperty(CompanyContactIdProperty)))
        command.ExecuteNonQuery()
      End Using
    End Sub

#End If
  End Class
End Namespace
