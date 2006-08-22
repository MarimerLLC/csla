Namespace Validation

  Friend Class RulesList

    Private mList As New List(Of IRuleMethod)
    Private mSorted As Boolean
    Private mDependantProperties As List(Of String)

    Public Sub Add(ByVal item As IRuleMethod)

      mList.Add(item)
      mSorted = False

    End Sub

    Public Function GetList(ByVal applySort As Boolean) As List(Of IRuleMethod)

      If applySort AndAlso Not mSorted Then
        mList.Sort()
        mSorted = True
      End If
      Return mList

    End Function

    Public Function GetDependancyList(ByVal create As Boolean) As List(Of String)

      If mDependantProperties Is Nothing AndAlso create Then
        mDependantProperties = New List(Of String)
      End If
      Return mDependantProperties

    End Function

  End Class

End Namespace

