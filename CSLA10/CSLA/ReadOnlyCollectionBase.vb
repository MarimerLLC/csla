Imports System.IO
Imports System.Runtime.Serialization.Formatters.Binary
Imports System.Configuration

<Serializable()> _
Public MustInherit Class ReadOnlyCollectionBase
  Inherits CSLA.Core.BindableCollectionBase

  Implements ICloneable

  Public Sub New()
    AllowEdit = False
    AllowNew = False
    AllowRemove = False
  End Sub

#Region " Remove, Clear, Set "

  Protected Locked As Boolean = True

  Protected Overrides Sub OnInsert(ByVal index As Integer, ByVal value As Object)
    If Locked Then
      Throw New NotSupportedException("Insert is invalid for a read-only collection")
    End If
  End Sub

  Protected Overrides Sub OnRemove(ByVal index As Integer, ByVal value As Object)
    If Locked Then
      Throw New NotSupportedException("Remove is invalid for a read-only collection")
    End If
  End Sub

  Protected Overrides Sub OnClear()
    If Locked Then
      Throw New NotSupportedException("Clear is invalid for a read-only collection")
    End If
  End Sub

  Protected Overrides Sub OnSet(ByVal index As Integer, ByVal oldValue As Object, ByVal newValue As Object)
    If Locked Then
      Throw New NotSupportedException("Items can not be changed in a read-only collection")
    End If
  End Sub

#End Region

#Region " Clone "

  ' all business objects _must_ be serializable
  ' and thus can be cloned - this just clinches
  ' the deal
  Public Function Clone() As Object Implements ICloneable.Clone
    Dim buffer As New MemoryStream
    Dim formatter As New BinaryFormatter

    formatter.Serialize(buffer, Me)
    buffer.Position = 0
    Return formatter.Deserialize(buffer)
  End Function

#End Region

#Region " Data Access "

  Private Sub DataPortal_Create(ByVal Criteria As Object)
    Throw New NotSupportedException("Invalid operation - create not allowed")
  End Sub

  Protected Overridable Sub DataPortal_Fetch(ByVal Criteria As Object)
    Throw New NotSupportedException("Invalid operation - fetch not allowed")
  End Sub

  Private Sub DataPortal_Update()
    Throw New NotSupportedException("Invalid operation - update not allowed")
  End Sub

  Private Sub DataPortal_Delete(ByVal Criteria As Object)
    Throw New NotSupportedException("Invalid operation - delete not allowed")
  End Sub

  Protected Function DB(ByVal DatabaseName As String) As String
    Return ConfigurationSettings.AppSettings("DB:" & DatabaseName)
  End Function

#End Region

End Class
