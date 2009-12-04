Imports System.Data.SqlClient

<Serializable()> _
Public Class ConsultantNVList
  Inherits NameValueListBase(Of Integer, String)

#Region " Factory Methods "

  Private Shared _list As ConsultantNVList

  Public Shared Function GetList() As ConsultantNVList

    If _list Is Nothing Then
      _list = DataPortal.Fetch(Of ConsultantNVList)(New Criteria(False))
    End If
    Return _list

  End Function

  Public Shared Function GetBenchList() As ConsultantNVList

    If _list Is Nothing Then
      _list = DataPortal.Fetch(Of ConsultantNVList)(New Criteria(True))
    End If
    Return _list

  End Function

  Public Shared Sub FlushCache()

    _list = Nothing

  End Sub

  Private Sub New()

    ' require use of factory methods

  End Sub

#End Region

#Region " Data Access "

  <Serializable()> _
  Private Shadows Class Criteria

    Private _benchOnly As Boolean
    Public ReadOnly Property BenchOnly() As Boolean
      Get
        Return _benchOnly
      End Get
    End Property

    Public Sub New(ByVal benchOnly As Boolean)
      _benchOnly = benchOnly
    End Sub

  End Class

  Private Overloads Sub DataPortal_Fetch(ByVal criteria As Criteria)

    Using cn As New SqlConnection(Database.BenchMgrConnectionString)
      cn.Open()
      Using cm As SqlCommand = cn.CreateCommand
        cm.CommandType = System.Data.CommandType.Text
        If criteria.BenchOnly Then
          cm.CommandText = "SELECT id,name FROM Consultant WHERE onbench='True' ORDER BY name"

        Else
          cm.CommandText = "SELECT id,name FROM Consultant ORDER BY name"
        End If
        Using dr As New Csla.Data.SafeDataReader(cm.ExecuteReader)
          IsReadOnly = False
          While dr.Read
            Add(New NameValuePair(dr.GetInt32("id"), dr.GetString("name")))
          End While
          IsReadOnly = True
        End Using
      End Using
    End Using

  End Sub

#End Region

End Class
