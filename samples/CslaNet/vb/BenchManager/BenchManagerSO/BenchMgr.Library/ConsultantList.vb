Imports System.Data.SqlClient

<Serializable()> _
  Public Class ConsultantList
  Inherits EditableRootListBase(Of ConsultantEdit)

#Region " Business Methods "

  Protected Overrides Function AddNewCore() As Object

    Dim child As ConsultantEdit = ConsultantEdit.NewConsultant
    Add(child)
    Return child

  End Function

  Public Sub SaveAll()

    For index As Integer = 0 To Me.Count - 1
      If Me(index).IsSavable Then
        Me.SaveItem(index)
      End If
    Next

  End Sub

#End Region

#Region " Factory Methods "

  Public Shared Function NewList() As ConsultantList

    Return New ConsultantList

  End Function

  Public Shared Function GetList() As ConsultantList

    Return DataPortal.Fetch(Of ConsultantList)()

  End Function

  Public Shared Function GetConsultant(ByVal id As Integer) As ConsultantList

    Return DataPortal.Fetch(Of ConsultantList)(New Criteria(id))

  End Function

  Private Sub New()

    AllowEdit = True
    AllowNew = True
    AllowRemove = True

  End Sub

#End Region

#Region " Data Access "

  <Serializable()> _
  Private Class Criteria

    Private _id As Integer
    Public ReadOnly Property Id() As Integer
      Get
        Return _id
      End Get
    End Property

    Public Sub New(ByVal id As Integer)
      _id = id
    End Sub

  End Class

  Private Overloads Sub DataPortal_Fetch()

    Using cn As New SqlConnection(Database.BenchMgrConnectionString)
      cn.Open()
      ApplicationContext.LocalContext.Add("cn", cn)
      Using cm As SqlCommand = cn.CreateCommand
        cm.CommandType = System.Data.CommandType.Text
        cm.CommandText = "SELECT id,name,onbench FROM Consultant ORDER BY name"
        Using dr As New Csla.Data.SafeDataReader(cm.ExecuteReader)
          While dr.Read
            Add(ConsultantEdit.GetConsultant(dr))
          End While
        End Using
      End Using
      ApplicationContext.LocalContext.Remove("cn")
    End Using

  End Sub

  Private Overloads Sub DataPortal_Fetch(ByVal criteria As Criteria)

    Using cn As New SqlConnection(Database.BenchMgrConnectionString)
      cn.Open()
      ApplicationContext.LocalContext.Add("cn", cn)
      Using cm As SqlCommand = cn.CreateCommand
        cm.CommandType = System.Data.CommandType.Text
        cm.CommandText = "SELECT id,name,onbench FROM Consultant WHERE id=@id ORDER BY name"
        cm.Parameters.AddWithValue("@id", criteria.Id)
        Using dr As New Csla.Data.SafeDataReader(cm.ExecuteReader)
          While dr.Read
            Add(ConsultantEdit.GetConsultant(dr))
          End While
        End Using
      End Using
      ApplicationContext.LocalContext.Remove("cn")
    End Using

  End Sub

#End Region

End Class
