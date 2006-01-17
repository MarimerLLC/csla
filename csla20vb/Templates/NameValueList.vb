Imports System.Data.SqlClient

<Serializable()> _
Public Class NameValueList
  Inherits NameValueListBase(Of Integer, String)

#Region " Factory Methods "

  Public Shared Function GetList() As NameValueList
    Return DataPortal.Fetch(Of NameValueList)(New Criteria(GetType(NameValueList)))
  End Function

  Private Sub New()
    ' require use of factory methods
  End Sub

#End Region

#Region " Data Access "

  Protected Overrides Sub DataPortal_Fetch(ByVal criteria As Object)

    IsReadOnly = False
    ' TODO: load values
    Using dr As SqlDataReader = Nothing
      While dr.Read
        Add(New NameValueListBase(Of Integer, String).NameValuePair( _
          dr.GetInt32(0), dr.GetString(1)))
      End While
    End Using
    IsReadOnly = True

  End Sub

#End Region

End Class
