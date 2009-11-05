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

Namespace Rolodex.Business.BusinessClasses
  <Serializable, MobileFactory("Rolodex.Business.BusinessClasses.RanksFactory, Rolodex.Business", "FetchRanks")> _
  Public Class Ranks
	  Inherits NameValueListBase(Of Integer, String)
#If SILVERLIGHT Then
	Public Sub New()
	End Sub

	Public Shared Sub GetRanks(ByVal handler As EventHandler(Of DataPortalResult(Of Ranks)))
	  Dim dp As DataPortal(Of Ranks) = New DataPortal(Of Ranks)()
	  AddHandler dp.FetchCompleted, handler
	  dp.BeginFetch()
	End Sub
#End If

	Friend Sub SetReadOnlyFlag(ByVal flag As Boolean)
	  IsReadOnly = flag
	End Sub

	Public Function GetRankName(ByVal rank As Integer) As String
	  Dim returnValue As String = String.Empty
	  For Each oneItem In Me
		If oneItem.Key = rank Then
		  returnValue = oneItem.Value
		  Exit For
		End If
	  Next oneItem
	  Return returnValue
	End Function
  End Class
End Namespace
