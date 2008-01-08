Imports Microsoft.VisualBasic
Imports System
Imports System.Collections.Generic
Imports System.Collections.ObjectModel
Imports System.Reflection
Imports System.Linq
Imports System.Linq.Expressions
Imports System.Text

Namespace Linq

  Friend Class Index(Of T)

    Implements IIndex(Of T)

    'implements a hashtable with chaining for cases
    '   where we have a collision on hash code

    Private _indexField As String = ""
    Private _index As Dictionary(Of Integer, List(Of T)) = New Dictionary(Of Integer, List(Of T))()
    Private _countCache As Integer = 0

    Private _theProp As PropertyInfo = Nothing
    Private _indexAttribute As IndexableAttribute = Nothing
    Private _loaded As Boolean = False

    Private Sub New()
    End Sub

    Public Sub New(ByVal indexField As String, ByVal indexAttribute As IndexableAttribute)
      _indexField = indexField
      _theProp = GetType(T).GetProperty(_indexField)
      _indexAttribute = indexAttribute
      If _indexAttribute.IndexMode = IndexModeEnum.IndexModeAlways Then
        _loaded = True
      End If
    End Sub

#Region "IIndex<T> Members"

    Private ReadOnly Property IndexField() As PropertyInfo Implements IIndex(Of T).IndexField
      Get
        Return _theProp
      End Get
    End Property

    Private Function WhereEqual(ByVal item As T) As IEnumerable(Of T) Implements IIndex(Of T).WhereEqual
      Dim hashCode As Integer = item.GetHashCode()
      Dim propertyValue As IComparable = TryCast(_theProp.GetValue(item, Nothing), IComparable)

      If _index.ContainsKey(hashCode) Then
        For Each itemFromIndex As T In _index(hashCode)
          Dim propertyValueFromIndex As IComparable = TryCast(_theProp.GetValue(itemFromIndex, Nothing), IComparable)
          If CType(propertyValue, Object).Equals(propertyValueFromIndex) Then
            'TODO: INSTANT VB TODO TASK: VB does not support iterators and has no equivalent to the C# 'yield' keyword:
            'yield Return itemFromIndex
          End If
        Next itemFromIndex
      End If
    End Function

    Private Function WhereEqual(ByVal hashCode As Integer, ByVal expr As Func(Of T, Boolean)) As IEnumerable(Of T) Implements IIndex(Of T).WhereEqual
      If _index.ContainsKey(hashCode) Then
        For Each item As T In _index(hashCode)
          If expr(item) Then
            'TODO: INSTANT VB TODO TASK: VB does not support iterators and has no equivalent to the C# 'yield' keyword:
            'yield Return item
          End If
        Next item
      End If
    End Function


#End Region

    Private Sub DoAdd(ByVal item As T)
      If _theProp IsNot Nothing Then
        Dim hashCode As Integer = _theProp.GetValue(item, Nothing).GetHashCode()
        If _index.ContainsKey(hashCode) Then
          _index(hashCode).Add(item)
        Else
          Dim newList As List(Of T) = New List(Of T)(1)
          newList.Add(item)
          _index.Add(hashCode, newList)
        End If
        _countCache += 1
      End If
    End Sub

#Region "ICollection<T> Members"

    Private Sub Add(ByVal item As T) Implements ICollection(Of T).Add
      DoAdd(item)
    End Sub

    Private Sub Clear() Implements ICollection(Of T).Clear
      _index.Clear()
    End Sub

    Private Function Contains(ByVal item As T) As Boolean Implements ICollection(Of T).Contains
      Dim hashCode As Integer = _theProp.GetValue(item, Nothing).GetHashCode()
      If _index.ContainsKey(hashCode) Then
        Return _index(hashCode).Contains(item)
      Else
        Return False
      End If
    End Function

    Private Sub CopyTo(ByVal array() As T, ByVal arrayIndex As Integer) Implements ICollection(Of T).CopyTo
      If Object.ReferenceEquals(array, Nothing) Then
        Throw New ArgumentNullException("Null array reference", "array")
      End If

      If arrayIndex < 0 Then
        Throw New ArgumentOutOfRangeException("Index is out of range", "index")
      End If

      If array.Rank > 1 Then
        Throw New ArgumentException("Array is multi-dimensional", "array")
      End If

      For Each o As Object In Me
        array.SetValue(o, arrayIndex)
        arrayIndex += 1
      Next o
    End Sub

    Private ReadOnly Property Count() As Integer Implements ICollection(Of T).Count
      Get
        Return _countCache
      End Get
    End Property

    Private ReadOnly Property IsReadOnly() As Boolean Implements ICollection(Of T).IsReadOnly
      Get
        Return False
      End Get
    End Property

    Private Function Remove(ByVal item As T) As Boolean Implements ICollection(Of T).Remove
      Dim hashCode As Integer = _theProp.GetValue(item, Nothing).GetHashCode()
      If _index.ContainsKey(hashCode) Then
        If _index(hashCode).Contains(item) Then
          _index(hashCode).Remove(item)
          _countCache -= 1
          Return True
        Else
          Return False
        End If
      Else
        Return False
      End If
    End Function

    Private Sub ReIndex(ByVal item As T) Implements IIndex(Of T).ReIndex
      Dim wasRemoved As Boolean = (TryCast(Me, ICollection(Of T))).Remove(item)
      If (Not wasRemoved) Then
        RemoveByReference(item)
      End If
      DirectCast(Me, ICollection(Of T)).Add(item)
    End Sub

#End Region

#Region "IEnumerable<T> Members"

    Private Function GetEnumerator() As IEnumerator(Of T) Implements IEnumerable(Of T).GetEnumerator
      For Each list As List(Of T) In _index.Values
        For Each item As T In list
          'TODO: INSTANT VB TODO TASK: VB does not support iterators and has no equivalent to the C# 'yield' keyword:
          'yield Return item
        Next item
      Next list
    End Function

#End Region

#Region "IEnumerable Members"

    Private Function IEnumerable_GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
      For Each list As List(Of T) In _index.Values
        For Each item As T In list
          'TODO: INSTANT VB TODO TASK: VB does not support iterators and has no equivalent to the C# 'yield' keyword:
          'yield Return item
        Next item
      Next list
    End Function

#End Region

    Private Sub RemoveByReference(ByVal item As T)
      For Each itemToCheck As T In Me
        If ReferenceEquals(itemToCheck, item) Then
          DirectCast(Me, ICollection(Of T)).Remove(item)
        End If
      Next itemToCheck
    End Sub

#Region "IIndex<T> Members"

    Private ReadOnly Property Loaded() As Boolean Implements IIndex(Of T).Loaded
      Get
        Return _loaded
      End Get
    End Property

    Private Sub InvalidateIndex() Implements IIndex(Of T).InvalidateIndex
      If _indexAttribute.IndexMode <> IndexModeEnum.IndexModeNever Then
        _loaded = False
      End If
    End Sub

    Private Sub LoadComplete() Implements IIndex(Of T).LoadComplete
      If _indexAttribute.IndexMode <> IndexModeEnum.IndexModeNever Then
        _loaded = True
      End If
    End Sub
    Private Property IIndex_IndexMode() As IndexModeEnum Implements IIndex(Of T).IndexMode
      Get
        Return _indexAttribute.IndexMode
      End Get
      Set(ByVal value As IndexModeEnum)
        _indexAttribute.IndexMode = value
      End Set
    End Property

#End Region

  End Class

End Namespace
