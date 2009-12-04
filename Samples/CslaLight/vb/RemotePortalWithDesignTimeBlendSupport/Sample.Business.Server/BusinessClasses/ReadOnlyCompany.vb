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
Imports Csla.Data
Imports System.Data.SqlClient
#End If

Namespace Sample.Business
  <Serializable()> _
  Public Class ReadOnlyCompany
    Inherits ReadOnlyBase(Of ReadOnlyCompany)

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
    Public ReadOnly Property CompanyName() As String
      Get
        Return GetProperty(CompanyNameProperty)
      End Get
    End Property

    Protected Overrides Sub AddAuthorizationRules()
      Dim canRead() As String = {"AdminUser", "RegularUser", "ReadOnlyUser"}
      AuthorizationRules.AllowGet(GetType(ReadOnlyCompany), canRead)
      AuthorizationRules.AllowRead(CompanyNameProperty, canRead)
      AuthorizationRules.AllowRead(CompanyIdProperty, canRead)
    End Sub

    <ComponentModel.EditorBrowsable(ComponentModel.EditorBrowsableState.Never)> _
    Public Shared Function DesignTime_Create(ByVal name As String) As ReadOnlyCompany
      Dim returnValue As New ReadOnlyCompany
      returnValue.LoadProperty(CompanyNameProperty, name)
      Return returnValue
    End Function

#If (Not Silverlight) Then

    Public Shared Function GetReadOnlyCompany(ByVal reader As SafeDataReader) As ReadOnlyCompany
      Return DataPortal.FetchChild(Of ReadOnlyCompany)(reader)
    End Function

    Private Sub Child_Fetch(ByVal reader As SafeDataReader)
      LoadProperty(Of Integer)(CompanyIdProperty, reader.GetInt32("CompanyId"))
      LoadProperty(Of String)(CompanyNameProperty, reader.GetString("CompanyName"))
    End Sub


#End If
  End Class
End Namespace
