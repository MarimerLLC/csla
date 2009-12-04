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

  Private Sub New()

    AllowEdit = True
    AllowNew = True
    AllowRemove = True

  End Sub

#End Region

#Region " Data Access "

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

#End Region

End Class
