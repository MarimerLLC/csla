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
#End If


Namespace Rolodex.Business.BusinessClasses
  <Serializable> _
  Public Class RankList
	  Inherits EditableRootListBase(Of Rank)



#If SILVERLIGHT Then
	Public Sub New()
	End Sub

	Protected Overrides Sub AddNewCore()
	  Add(Rank.NewRank())
	End Sub

#Else
	Private Sub New()
	End Sub
#End If

#If SILVERLIGHT Then
	Public Shared Sub GetRankList(ByVal handler As EventHandler(Of DataPortalResult(Of RankList)))
	  Dim dp As DataPortal(Of RankList) = New DataPortal(Of RankList)()
	  AddHandler dp.FetchCompleted, handler
	  dp.BeginFetch()
	End Sub
#Else

	Protected Sub DataPortal_Fetch()
	  RaiseListChangedEvents = False
	  Using connection As New SqlConnection(DataConnection.ConnectionString)
		connection.Open()
		Using command As New SqlCommand("RanksSelect", connection)
		  command.CommandType = System.Data.CommandType.StoredProcedure
		  Using reader As New Csla.Data.SafeDataReader(command.ExecuteReader())
			Do While reader.Read()
			  Add(Rank.GetRank(reader))
			Loop
		  End Using
		End Using
		connection.Close()
	  End Using
	  RaiseListChangedEvents = True
	End Sub
#End If

  End Class
End Namespace
