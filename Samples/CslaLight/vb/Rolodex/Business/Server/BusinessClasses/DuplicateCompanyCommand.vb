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
Imports Csla.DataPortalClient
Imports System.Threading

#If(Not SILVERLIGHT) Then
Imports System.Data.SqlClient
Imports Rolodex.Business.Data
#End If

Namespace Rolodex.Business.BusinessClasses
  <Serializable> _
  Public Class DuplicateCompanyCommand
	  Inherits CommandBase
#If SILVERLIGHT Then
	Public Sub New()
	End Sub
#Else
	Protected Sub New()
	End Sub
#End If

	Private _companyName As String
	Private _companyId As Integer
	Private _isDuplicate As Boolean

	Public Sub New(ByVal companyName As String, ByVal companyId As Integer)
	  _companyName = companyName
	  _companyId = companyId
	  _isDuplicate = False
	End Sub

	Public ReadOnly Property IsDuplicate() As Boolean
	  Get
		  Return _isDuplicate
	  End Get
	End Property

	Protected Overrides Sub OnGetState(ByVal info As Csla.Serialization.Mobile.SerializationInfo, ByVal mode As Csla.Core.StateMode)
	  MyBase.OnGetState(info, mode)
	  info.AddValue("_companyName", _companyName)
	  info.AddValue("_companyId", _companyId)
	  info.AddValue("_isDuplicate", _isDuplicate)
	End Sub
	Protected Overrides Sub OnSetState(ByVal info As Csla.Serialization.Mobile.SerializationInfo, ByVal mode As Csla.Core.StateMode)
	  _companyName = info.GetValue(Of String)("_companyName")
	  _companyId = info.GetValue(Of Integer)("_companyId")
	  _isDuplicate = info.GetValue(Of Boolean)("_isDuplicate")
	  MyBase.OnSetState(info, mode)
	End Sub

#If (Not SILVERLIGHT) Then

	Protected Overrides Sub DataPortal_Execute()
	  Using connection As New SqlConnection(DataConnection.ConnectionString)
		connection.Open()
		Using command As New SqlCommand("IsDuplicateCompany", connection)
		  command.CommandType = System.Data.CommandType.StoredProcedure
		  command.Parameters.Add(New SqlParameter("@companyID", _companyId))
		  command.Parameters.Add(New SqlParameter("@companyName", _companyName))
		  Dim isDuplicateParameter As New SqlParameter("@isDuplicate", _isDuplicate)
		  isDuplicateParameter.Direction = System.Data.ParameterDirection.Output
		  command.Parameters.Add(isDuplicateParameter)
		  command.ExecuteNonQuery()
		  _isDuplicate = CBool(isDuplicateParameter.Value)
		End Using
		connection.Close()
	  End Using
	End Sub
#End If
  End Class
End Namespace
