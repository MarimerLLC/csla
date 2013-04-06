Imports System
Imports System.Collections.Generic
Imports Csla

namespace $rootnamespace$

  <Serializable()> _
  Public Class $safeitemname$
    Inherits NameValueListBase(Of Integer, String)

#Region "Factory Methods"

    Private Shared _list As $safeitemname$

    Public Shared Function Get$safeitemname$() As $safeitemname$
      If _list Is Nothing Then
        _list = DataPortal.Fetch(Of $safeitemname$)()
      End If
      Return _list
    End Function

    Public Shared Sub InvalidateCache()
      _list = Nothing
    End Sub

    ' require use of factory methods 
    Private Sub New()
    End Sub

#End Region

#Region "Data Access"

    Private Overloads Sub DataPortal_Fetch()
      RaiseListChangedEvents = False
      IsReadOnly = False
      ' TODO: load values 
      Dim listData As Object = Nothing
      For Each oneItem As Object In CType(listData, IList(Of Object))
        Add(New $safeitemname$.NameValuePair(0, ""))
      Next
      IsReadOnly = True
      RaiseListChangedEvents = True
    End Sub

#End Region
  End Class

End Namespace