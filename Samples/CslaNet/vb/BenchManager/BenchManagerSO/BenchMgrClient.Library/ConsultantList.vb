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

    Dim svc As New BenchService.BenchServiceClient
    Dim list As New List(Of BenchService.ConsultantData)(svc.GetConsultantList(False))
    For Each item As BenchService.ConsultantData In list
      Add(ConsultantEdit.GetConsultant(item))
    Next

  End Sub

#End Region

End Class
