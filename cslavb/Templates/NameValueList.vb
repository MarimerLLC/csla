Imports System.Data.SqlClient

<Serializable()> _
Public Class NameValueList
  Inherits NameValueListBase(Of Integer, String)

#Region " Factory Methods "

  Private Shared mList As NameValueList

  Public Shared Function GetList() As NameValueList
    If mList Is Nothing Then
      mList = DataPortal.Fetch(Of NameValueList) _
        (New Criteria(GetType(NameValueList)))
    End If
    Return mList
  End Function

  Public Shared Sub InvalidateCache()
    mList = Nothing
  End Sub

  Private Sub New()
    ' require use of factory methods
  End Sub

#End Region

#Region " Data Access "

  Protected Overrides Sub DataPortal_Fetch(ByVal criteria As Object)

    RaiseListChangedEvents = False
    IsReadOnly = False
    ' TODO: load values
    Using dr As SqlDataReader = Nothing
      While dr.Read
        Add(New NameValueListBase(Of Integer, String). _
          NameValuePair(dr.GetInt32(0), dr.GetString(1)))
      End While
    End Using
    IsReadOnly = True
    RaiseListChangedEvents = True

  End Sub

#End Region

End Class
