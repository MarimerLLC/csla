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
Imports System.Data.SqlClient
Imports Rolodex.Business.Data
Imports Csla.Data
#End If

Namespace Rolodex.Business.BusinessClasses
  <Serializable> _
  Public Class Rank
	  Inherits BusinessBase(Of Rank)
#If SILVERLIGHT Then
	Public Sub New()
	End Sub
#Else
	Private Sub New()
	End Sub
#End If

	Friend Shared Function NewRank() As Rank
	  Dim returnValue As New Rank()
	  returnValue.ValidationRules.CheckRules()
	  Return returnValue
	End Function

	Private Shared RankIdProperty As PropertyInfo(Of Integer) = RegisterProperty(Of Integer)(New PropertyInfo(Of Integer)("RankId", "Rank Id", 0))
	Public ReadOnly Property RankId() As Integer
	  Get
		Return GetProperty(RankIdProperty)
	  End Get
	End Property

	Private Shared RankNameProperty As PropertyInfo(Of String) = RegisterProperty(Of String)(New PropertyInfo(Of String)("RankName", "Rank Name", String.Empty))
	Public Property RankName() As String
	  Get
		Return GetProperty(RankNameProperty)
	  End Get
	  Set(ByVal value As String)
		SetProperty(RankNameProperty, value)
	  End Set
	End Property

	Protected Overrides Sub AddAuthorizationRules()
	  Dim canWrite() As String = { "AdminUser", "RegularUser" }
	  Dim canRead() As String = { "AdminUser", "RegularUser", "ReadOnlyUser" }
	  Dim admin() As String = { "AdminUser" }
	  AuthorizationRules.AllowCreate(GetType(Rank), admin)
	  AuthorizationRules.AllowDelete(GetType(Rank), admin)
	  AuthorizationRules.AllowEdit(GetType(Rank), canWrite)
	  AuthorizationRules.AllowGet(GetType(Rank), canRead)
	  AuthorizationRules.AllowWrite(RankNameProperty, canWrite)
	  AuthorizationRules.AllowRead(RankNameProperty, canRead)
	  AuthorizationRules.AllowRead(RankIdProperty, canRead)
	End Sub

	Protected Overrides Sub AddBusinessRules()
      ValidationRules.AddRule(AddressOf Csla.Validation.CommonRules.StringRequired, New Csla.Validation.RuleArgs(RankNameProperty))
      ValidationRules.AddRule(AddressOf Csla.Validation.CommonRules.StringMaxLength, New Csla.Validation.CommonRules.MaxLengthRuleArgs(RankNameProperty, 20))

	End Sub

#If (Not SILVERLIGHT) Then

	Protected Overrides Sub DataPortal_Insert()
	  Using connection As New SqlConnection(DataConnection.ConnectionString)
		connection.Open()
		Using command As New SqlCommand("RanksInsert", connection)
		  command.CommandType = System.Data.CommandType.StoredProcedure
		  Dim idParameter As New SqlParameter("@rankId", ReadProperty(RankIdProperty))
		  idParameter.Direction = System.Data.ParameterDirection.Output
		  command.Parameters.Add(idParameter)
		  command.Parameters.Add(New SqlParameter("@rank", ReadProperty(RankNameProperty)))

		  command.ExecuteNonQuery()
		  LoadProperty(RankIdProperty, CInt(Fix(idParameter.Value)))
		End Using
		connection.Close()
	  End Using
	End Sub
	Protected Overrides Sub DataPortal_Update()
	  Using connection As New SqlConnection(DataConnection.ConnectionString)
		connection.Open()
		Using command As New SqlCommand("RanksUpdate", connection)
		  command.CommandType = System.Data.CommandType.StoredProcedure
		  command.Parameters.Add(New SqlParameter("@rankId", ReadProperty(RankIdProperty)))
		  command.Parameters.Add(New SqlParameter("@rank", ReadProperty(RankNameProperty)))
		  command.ExecuteNonQuery()
		End Using
		connection.Close()
	  End Using
	End Sub

	Protected Overrides Sub DataPortal_DeleteSelf()
	  Using connection As New SqlConnection(DataConnection.ConnectionString)
		connection.Open()
		Using command As New SqlCommand("RanksDelete", connection)
		  command.CommandType = System.Data.CommandType.StoredProcedure
		  command.Parameters.Add(New SqlParameter("@rankId", ReadProperty(RankIdProperty)))
		  command.ExecuteNonQuery()
		End Using
		connection.Close()
	  End Using
	End Sub

	Friend Shared Function GetRank(ByVal reader As SafeDataReader) As Rank
	  Dim returnValue As New Rank()
	  returnValue.LoadProperty(Of Integer)(RankIdProperty, reader.GetInt32("RankID"))
	  returnValue.LoadProperty(Of String)(RankNameProperty, reader.GetString("Rank"))
	  returnValue.MarkOld()
	  Return returnValue
	End Function

#End If
  End Class
End Namespace
