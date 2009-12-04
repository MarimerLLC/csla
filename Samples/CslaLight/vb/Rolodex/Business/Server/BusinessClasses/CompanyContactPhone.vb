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

#If (Not SILVERLIGHT) Then
Imports System.Data.SqlClient
Imports Rolodex.Business.Data
Imports Csla.Data
#End If


Namespace Rolodex.Business.BusinessClasses
  <Serializable()> _
  Public Class CompanyContactPhone
    Inherits BusinessBase(Of CompanyContactPhone)

#If SILVERLIGHT Then
    Public Sub New()
      DisableIEditableObject = True
      MarkAsChild()
    End Sub
#Else
	  Private Sub New()
      DisableIEditableObject = True
      MarkAsChild()
	  End Sub
#End If

    Private Shared CompanyContactPhoneIdProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(New PropertyInfo(Of Integer)("CompanyId", "Company Contact Phone Id", 0))
    Public ReadOnly Property CompanyContactPhoneId() As Integer
      Get
        Return GetProperty(CompanyContactPhoneIdProperty)
      End Get
    End Property

    Private Shared CompanyContactIdProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(New PropertyInfo(Of Integer)("CompanyContactId", "Contact Id", 0))
    Public Property CompanyContactId() As Integer
      Get
        Return GetProperty(CompanyContactIdProperty)
      End Get
      Set(ByVal value As Integer)
        SetProperty(CompanyContactIdProperty, value)
      End Set
    End Property

    Private Shared PhoneNumberProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(New PropertyInfo(Of String)("PhoneNumber", "Phone Number", String.Empty))
    Public Property PhoneNumber() As String
      Get
        Return GetProperty(PhoneNumberProperty)
      End Get
      Set(ByVal value As String)
        SetProperty(PhoneNumberProperty, value)
      End Set
    End Property

    Private Shared FaxNumberProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(New PropertyInfo(Of String)("FaxNumber", "Fax Number", String.Empty))
    Public Property FaxNumber() As String
      Get
        Return GetProperty(FaxNumberProperty)
      End Get
      Set(ByVal value As String)
        SetProperty(FaxNumberProperty, value)
      End Set
    End Property

    Protected Overrides Sub AddAuthorizationRules()
      Dim canWrite() As String = {"AdminUser", "RegularUser"}
      Dim canRead() As String = {"AdminUser", "RegularUser", "ReadOnlyUser"}
      Dim admin() As String = {"AdminUser"}

      AuthorizationRules.AllowCreate(GetType(CompanyContact), admin)
      AuthorizationRules.AllowDelete(GetType(CompanyContact), admin)
      AuthorizationRules.AllowEdit(GetType(CompanyContact), canWrite)
      AuthorizationRules.AllowGet(GetType(CompanyContact), canRead)

      AuthorizationRules.AllowWrite(PhoneNumberProperty, canWrite)
      AuthorizationRules.AllowWrite(FaxNumberProperty, canWrite)
      AuthorizationRules.AllowWrite(CompanyContactIdProperty, canWrite)

      AuthorizationRules.AllowRead(PhoneNumberProperty, canRead)
      AuthorizationRules.AllowRead(FaxNumberProperty, canRead)
      AuthorizationRules.AllowRead(CompanyContactIdProperty, canRead)
      AuthorizationRules.AllowRead(CompanyContactPhoneIdProperty, canRead)
    End Sub

    Protected Overrides Sub AddBusinessRules()
      ValidationRules.AddRule(AddressOf Csla.Validation.CommonRules.StringRequired, New Csla.Validation.RuleArgs(PhoneNumberProperty))
      ValidationRules.AddRule(AddressOf Csla.Validation.CommonRules.StringRequired, New Csla.Validation.RuleArgs(FaxNumberProperty))
      ValidationRules.AddRule(AddressOf Csla.Validation.CommonRules.StringMaxLength, New Csla.Validation.CommonRules.MaxLengthRuleArgs(PhoneNumberProperty, 30))
      ValidationRules.AddRule(AddressOf Csla.Validation.CommonRules.StringMaxLength, New Csla.Validation.CommonRules.MaxLengthRuleArgs(FaxNumberProperty, 50))
    End Sub

    Friend Shared Function NewCompanyContactPhone() As CompanyContactPhone
      Dim returnValue As New CompanyContactPhone()
      returnValue.MarkAsChild()
      returnValue.ValidationRules.CheckRules()
      Return returnValue
    End Function


#If (Not Silverlight) Then

	Friend Shared Function GetCompanyContactPhone(ByVal reader As SafeDataReader) As CompanyContactPhone
	  Return DataPortal.FetchChild(Of CompanyContactPhone)(reader)
	End Function

	Private Sub Child_Fetch(ByVal reader As SafeDataReader)
	  LoadProperty(Of Integer)(CompanyContactPhoneIdProperty, reader.GetInt32("CompanyContactPhoneId"))
	  LoadProperty(Of Integer)(CompanyContactIdProperty, reader.GetInt32("CompanyContactId"))
	  LoadProperty(Of String)(PhoneNumberProperty, reader.GetString("PhoneNumber"))
	  LoadProperty(Of String)(FaxNumberProperty, reader.GetString("FaxNumber"))
	End Sub

	Private Sub Child_Insert(ByVal companyContact As CompanyContact, ByVal connection As SqlConnection)
	  InsertUpdate(True, connection, companyContact)
	End Sub

	Private Sub InsertUpdate(ByVal insert As Boolean, ByVal connection As SqlConnection, ByVal companyContact As CompanyContact)
	  Using command As New SqlCommand("CompanyContactPhonesUpdate", connection)
		command.CommandType = System.Data.CommandType.StoredProcedure
		command.Parameters.Add(New SqlParameter("@companyContactPhoneId", ReadProperty(CompanyContactPhoneIdProperty)))
		If insert Then
		  command.Parameters("@companyContactPhoneId").Direction = System.Data.ParameterDirection.Output
		  LoadProperty(Of Integer)(CompanyContactIdProperty, companyContact.CompanyContactId)
		  command.CommandText = "CompanyContactPhonesInsert"
		End If

		command.Parameters.Add(New SqlParameter("@companyContactId", ReadProperty(CompanyContactIdProperty)))
		command.Parameters.Add(New SqlParameter("@phoneNumber", ReadProperty(PhoneNumberProperty)))
		command.Parameters.Add(New SqlParameter("@faxNumber", ReadProperty(FaxNumberProperty)))

		command.ExecuteNonQuery()
		If insert Then
		  LoadProperty(CompanyContactPhoneIdProperty, command.Parameters("@companyContactPhoneId").Value)
		End If
		MarkOld()
	  End Using
	End Sub

	Private Sub Child_Update(ByVal companyContact As CompanyContact, ByVal connection As SqlConnection)
	  InsertUpdate(False, connection, companyContact)
	End Sub

	Private Sub Child_DeleteSelf(ByVal companyContact As CompanyContact, ByVal connection As SqlConnection)

	  Using command As New SqlCommand("CompanyContactPhonesDelete", connection)
		command.CommandType = System.Data.CommandType.StoredProcedure
		command.Parameters.Add(New SqlParameter("@companyContactPhoneId", ReadProperty(CompanyContactPhoneIdProperty)))
		command.ExecuteNonQuery()
	  End Using
	End Sub


#End If

  End Class
End Namespace
