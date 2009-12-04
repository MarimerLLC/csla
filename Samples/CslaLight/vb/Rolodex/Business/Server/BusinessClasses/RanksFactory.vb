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
  Public Class RanksFactory

#If(Not SILVERLIGHT) Then

	Public Function FetchRanks() As Ranks
	  Dim returnValue As New Ranks()
	  returnValue.RaiseListChangedEvents = False
	  returnValue.SetReadOnlyFlag(False)
	  returnValue.Add(New Ranks.NameValuePair(0, ""))

	  Using connection As New SqlConnection(DataConnection.ConnectionString)
		connection.Open()
		Using command As New SqlCommand("RanksSelect", connection)
		  command.CommandType = System.Data.CommandType.StoredProcedure
		  Using reader As New Csla.Data.SafeDataReader(command.ExecuteReader())
			Do While reader.Read()
			  returnValue.Add(New Ranks.NameValuePair(reader.GetInt32("RankID"), reader.GetString("Rank")))
			Loop
		  End Using
		End Using
		connection.Close()
	  End Using

	  returnValue.SetReadOnlyFlag(True)
	  returnValue.RaiseListChangedEvents = True
	  Return returnValue
	End Function
#End If
  End Class
End Namespace
