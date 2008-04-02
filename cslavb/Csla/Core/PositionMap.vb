Namespace Core

  Friend Class PositionMap(Of T As Core.IEditableBusinessObject)

    Private _map As Dictionary(Of T, Integer)
    Private _list As IList(Of T)

    Public Sub New(ByVal list As IList(Of T))
      _list = list
      RebuildMap()
    End Sub

    Public Sub ClearMap()
      _map = New Dictionary(Of T, Integer)(_list.Count)
    End Sub

    Public Sub AddToMap(ByVal item As T)
      If Not _map.ContainsKey(item) Then
        _map.Add(item, _list.Count - 1)
      End If
    End Sub

    Public Sub InsertIntoMap(ByVal item As T, ByVal position As Integer)
      If position = _list.Count - 1 Then
        AddToMap(item)
      Else
        Dim i As Integer = _list.Count - 1
        Do While i > position
          _map(_list(i)) += 1
          i -= 1
        Loop
        _map.Add(item, position)
      End If
    End Sub

    Public Sub RemoveFromMap(ByVal item As T)
      Dim oldPosition As Integer = PositionOf(item)
      If oldPosition = -1 Then
        Return
      End If
      _map.Remove(item)
      For i As Integer = oldPosition + 1 To _list.Count - 1
        _map(_list(i)) -= 1
      Next i
    End Sub

    Public Function PositionOf(ByVal item As T) As Integer
      If _map.ContainsKey(item) Then
        Return _map(item)
      Else
        Return -1
      End If
    End Function

    Public Sub RebuildMap()
      ClearMap()
      Dim i As Integer = 0
      For Each item As T In _list
        _map.Add(item, i)
        i += 1
      Next item

    End Sub

  End Class

End Namespace