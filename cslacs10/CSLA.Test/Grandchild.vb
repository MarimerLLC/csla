<Serializable()> _
Public Class Grandchild
  Inherits BusinessBase

  Private mData As String = ""

  Public Property Data() As String
    Get
      Return mData
    End Get
    Set(ByVal Value As String)
      If mData <> Value Then
        mData = Value
        MarkDirty()
      End If
    End Set
  End Property

  Friend Shared Function NewGrandChild(ByVal Data As String) As Grandchild
    Dim obj As New Grandchild
    obj.mData = Data
    Return obj
  End Function

  Friend Shared Function GetGrandChild(ByVal dr As IDataReader) As Grandchild
    Dim obj As New Grandchild
    obj.Fetch(dr)
    Return obj
  End Function

  Private Sub New()
    ' prevent direct creation
    MarkAsChild()
  End Sub

  Private Sub Fetch(ByVal dr As IDataReader)
    MarkOld()
  End Sub

  Friend Sub Update(ByVal tr As IDbTransaction)
    If IsDeleted Then
      ' we would delete here
      MarkNew()
    Else
      If IsNew Then
        ' we would insert here

      Else
        ' we would update here
      End If
      MarkOld()
    End If
  End Sub

  Protected Overrides Sub Deserialized()
    MyBase.Deserialized()
    Session.Add("GCDeserialized", "GC Deserialized")
  End Sub

  Protected Overrides Sub Serialized()
    MyBase.Serialized()
    Session.Add("GCSerialized", "GC Serialized")
  End Sub

  Protected Overrides Sub Serializing()
    MyBase.Serializing()
    Session.Add("GCSerializing", "GC Serializing")
  End Sub

End Class
