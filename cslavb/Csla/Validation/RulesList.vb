Namespace Validation

  Friend Class RulesList

    Private _list As New List(Of IRuleMethod)
    Private _sorted As Boolean
    Private _dependantProperties As List(Of String)

    Public Sub Add(ByVal item As IRuleMethod)

      _list.Add(item)
      _sorted = False

    End Sub

    Public Function GetList(ByVal applySort As Boolean) As List(Of IRuleMethod)

      If applySort AndAlso Not _sorted Then
        SyncLock _list
          If applySort AndAlso Not _sorted Then
            _list.Sort()
            _sorted = True
          End If
        End SyncLock
      End If
      Return _list

    End Function

    Public Function GetDependancyList(ByVal create As Boolean) As List(Of String)

      If _dependantProperties Is Nothing AndAlso create Then
        _dependantProperties = New List(Of String)
      End If
      Return _dependantProperties

    End Function

  End Class

End Namespace

