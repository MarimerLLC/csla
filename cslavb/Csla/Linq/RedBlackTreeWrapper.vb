Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Reflection
Imports C5

Namespace Linq
  Friend Class RedBlackTreeWrapper(Of K As IComparable(Of T), T)
    Implements IBalancedSearch(Of T), System.Collections.Generic.ICollection(Of T)

    Private _internalWrapper As TreeDictionary(Of K, List(Of T))
    Private _propInfo As PropertyInfo = Nothing

    Private Sub New()

    End Sub

    Public Sub New(ByVal propInfo As PropertyInfo)
      _propInfo = propInfo      
      _internalWrapper = New TreeDictionary(Of K, List(Of T))()
    End Sub

#Region "IBalancedSearch<T> Members"

    Public Function ItemsLessThan(ByVal pivot As Object) As System.Collections.Generic.IEnumerable(Of T) Implements IBalancedSearch(Of T).ItemsLessThan      
      Dim list As List(Of T) = New List(Of T)
      For Each pair In _internalWrapper.RangeTo(CType(pivot, K))
        For Each item In pair.Value
          list.Add(item)
        Next
      Next

      Return list
    End Function

    Public Function ItemsGreaterThan(ByVal pivot As Object) As System.Collections.Generic.IEnumerable(Of T) Implements IBalancedSearch(Of T).ItemsGreaterThan
      Dim comparer = System.Collections.Generic.Comparer(Of K).Default
      Dim list As List(Of T) = New List(Of T)
      For Each pair In _internalWrapper.RangeFrom(CType(pivot, K))
        If comparer.Compare(CType(pivot, K), pair.Key) <> 0 Then
          For Each item In pair.Value
            list.Add(item)
          Next
        End If
      Next
      Return list

    End Function

    Public Function ItemsLessThanOrEqualTo(ByVal pivot As Object) As System.Collections.Generic.IEnumerable(Of T) Implements IBalancedSearch(Of T).ItemsLessThanOrEqualTo
      Dim list As List(Of T) = New List(Of T)
      For Each item In ItemsLessThan(pivot)
        list.Add(item)
      Next

      For Each item In ItemsEqualTo(pivot)
        list.Add(item)
      Next

      Return list
    End Function

    Public Function ItemsGreaterThanOrEqualTo(ByVal pivot As Object) As System.Collections.Generic.IEnumerable(Of T) Implements IBalancedSearch(Of T).ItemsGreaterThanOrEqualTo
      Dim list As List(Of T) = New List(Of T)
      For Each item In ItemsGreaterThan(pivot)
        list.Add(item)
      Next

      For Each item In ItemsEqualTo(pivot)
        list.Add(item)
      Next

      Return list
    End Function

    Public Function ItemsEqualTo(ByVal pivot As Object) As System.Collections.Generic.IEnumerable(Of T) Implements IBalancedSearch(Of T).ItemsEqualTo
      Dim result As List(Of T) = Nothing
      Dim list As List(Of T) = New List(Of T)      
      Dim handler As TreeDictionary(Of K, List(Of T)).FindKeyByVal = _internalWrapper.GetFindByValMethod()

      handler.Invoke(CType(pivot, K), result)

      If result IsNot Nothing Then
        For Each item As T In result
          list.Add(item)
        Next
      End If

      Return list
    End Function

#End Region

    Private Function GetKeyVal(ByVal obj As T) As K
      Return CType(_propInfo.GetValue(obj, Nothing), K)
    End Function

#Region "ICollection<T> Members"

    Public Sub Add(ByVal item As T) Implements System.Collections.Generic.ICollection(Of T).Add

      If Not _internalWrapper.Contains(GetKeyVal(item)) Then
        _internalWrapper.Add(GetKeyVal(item), New List(Of T)())
      End If

      _internalWrapper(GetKeyVal(item)).Add(item)

    End Sub

    Public Sub Clear() Implements System.Collections.Generic.ICollection(Of T).Clear
      _internalWrapper.Clear()
    End Sub

    Public Function Contains(ByVal item As T) As Boolean Implements System.Collections.Generic.ICollection(Of T).Contains
      Return _internalWrapper.Contains(GetKeyVal(item))
    End Function

    Public Sub CopyTo(ByVal array() As T, ByVal arrayIndex As Integer) Implements System.Collections.Generic.ICollection(Of T).CopyTo
      Throw New NotImplementedException()
    End Sub

    Public ReadOnly Property Count() As Integer Implements System.Collections.Generic.ICollection(Of T).Count
      Get
        Return _internalWrapper.Count
      End Get
    End Property

    Public ReadOnly Property IsReadOnly() As Boolean Implements System.Collections.Generic.ICollection(Of T).IsReadOnly
      Get
        Return False
      End Get
    End Property

    Public Function Remove(ByVal item As T) As Boolean Implements System.Collections.Generic.ICollection(Of T).Remove
      If _internalWrapper.Contains(GetKeyVal(item)) Then
        _internalWrapper(GetKeyVal(item)).Remove(item)
      End If
    End Function

#End Region

#Region "IEnumerable<T> Members"

    Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of T) Implements System.Collections.Generic.IEnumerable(Of T).GetEnumerator
      Dim list As List(Of T) = New List(Of T)
      For Each pair In _internalWrapper
        For Each item In pair.Value
          list.Add(item)
        Next
      Next

      Return list.GetEnumerator
    End Function

#End Region

#Region "IEnumerable Members"
    Public Function IEnum_GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
      Dim list As List(Of T) = New List(Of T)
      For Each pair In _internalWrapper
        For Each item In pair.Value
          list.Add(item)
        Next
      Next

      Return list.GetEnumerator
    End Function
#End Region

  End Class
End Namespace

