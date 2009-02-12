Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Reflection
Imports System.Collections
Imports Csla.Properties

Namespace Linq
  Friend Class BalancedTreeIndex(Of T)
    Implements IRangeTestableIndex(Of T)

    Private _indexField As String = String.Empty
    Private _index As Object
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
      Dim propType = _theProp.PropertyType
      Dim propInfo = GetType(T).GetProperty(indexField)

      Dim classTypeWrapper = Type.GetType("Csla.Linq.RedBlackTreeWrapper`2")
      Dim typeParamsWrapper = New Type() {propType, GetType(T)}
      Dim constructedTypeWrapper = classTypeWrapper.MakeGenericType(typeParamsWrapper)
      Dim ctorParamsWrapper = New Object() {propInfo}
      _index = Activator.CreateInstance(constructedTypeWrapper, ctorParamsWrapper)

      If _indexAttribute.IndexMode = IndexModeEnum.IndexModeAlways Then
        _loaded = True
      End If
    End Sub

    Private Sub LoadOnDemandIndex()
      If Not _loaded AndAlso _indexAttribute.IndexMode <> IndexModeEnum.IndexModeNever Then
        CType(Me, IIndex(Of T)).LoadComplete()
      End If
    End Sub

#Region "IIndex<T> Members"

    Public ReadOnly Property IndexField() As System.Reflection.PropertyInfo Implements IIndex(Of T).IndexField
      Get
        Return _theProp
      End Get
    End Property

    Public Function WhereEqual(ByVal item As T) As System.Collections.Generic.IEnumerable(Of T) Implements IIndex(Of T).WhereEqual
      LoadOnDemandIndex()
      Dim pivotVal = _theProp.GetValue(item, Nothing)
      Dim balancedIndex = CType(_index, IBalancedSearch(Of T))
      Return balancedIndex.ItemsEqualTo(pivotVal)
    End Function

    Public Function WhereEqual(ByVal pivotVal As Object, ByVal expr As System.Func(Of T, Boolean)) As System.Collections.Generic.IEnumerable(Of T) Implements IIndex(Of T).WhereEqual
      Dim balancedIndex = CType(_index, IBalancedSearch(Of T))
      Return balancedIndex.ItemsEqualTo(pivotVal)
    End Function

    Public Sub RemoveByReference(ByVal item As T)
      For Each itemToCheck In Me
        If ReferenceEquals(itemToCheck, item) Then
          CType(Me, ICollection(Of T)).Remove(item)
        End If
      Next
    End Sub

    Public Sub ReIndex(ByVal item As T) Implements IIndex(Of T).ReIndex
      Dim wasRemoved As Boolean = CType(Me, ICollection(Of T)).Remove(item)
      If Not wasRemoved Then
        RemoveByReference(item)
      End If
      CType(Me, ICollection(Of T)).Add(item)
    End Sub

    Public ReadOnly Property Loaded() As Boolean Implements IIndex(Of T).Loaded
      Get
        Return _loaded
      End Get
    End Property

    Public Sub InvalidateIndex() Implements IIndex(Of T).InvalidateIndex
      If _indexAttribute.IndexMode <> IndexModeEnum.IndexModeNever Then
        _loaded = False
      End If
    End Sub

    Public Sub LoadComplete() Implements IIndex(Of T).LoadComplete
      If _indexAttribute.IndexMode <> IndexModeEnum.IndexModeNever Then
        _loaded = True
      End If
    End Sub

    Public Property IndexMode() As IndexModeEnum Implements IIndex(Of T).IndexMode
      Get
        Return _indexAttribute.IndexMode
      End Get
      Set(ByVal value As IndexModeEnum)
        _indexAttribute.IndexMode = value
      End Set
    End Property

#End Region

#Region "ICollection<T> Members"

    Private Sub DoAdd(ByVal item As T)
      If _theProp IsNot Nothing Then
        CType(_index, ICollection(Of T)).Add(item)
        _countCache += 1
      End If
    End Sub

    Public Sub Add(ByVal item As T) Implements System.Collections.Generic.ICollection(Of T).Add
      DoAdd(item)
    End Sub

    Public Sub Clear() Implements System.Collections.Generic.ICollection(Of T).Clear
      CType(_index, ICollection(Of T)).Clear()
    End Sub

    Public Function Contains(ByVal item As T) As Boolean Implements System.Collections.Generic.ICollection(Of T).Contains
      Return CType(_index, ICollection(Of T)).Contains(item)
    End Function

    Public Sub CopyTo(ByVal array() As T, ByVal arrayIndex As Integer) Implements System.Collections.Generic.ICollection(Of T).CopyTo
      If Object.ReferenceEquals(array, Nothing) Then
        Throw New ArgumentNullException(My.Resources.NullArrayReference, "array")
      End If

      If arrayIndex < 0 Then
        Throw New ArgumentOutOfRangeException(My.Resources.IndexIsOutOfRange, "index")
      End If

      If array.Rank > 1 Then
        Throw New ArgumentException(My.Resources.ArrayIsMultiDimensional, "array")
      End If

      For Each o As Object In Me
        array.SetValue(o, arrayIndex)
        arrayIndex += 1
      Next

    End Sub

    Public ReadOnly Property Count() As Integer Implements System.Collections.Generic.ICollection(Of T).Count
      Get
        Return _countCache
      End Get
    End Property

    Public ReadOnly Property IsReadOnly() As Boolean Implements System.Collections.Generic.ICollection(Of T).IsReadOnly
      Get
        Return False
      End Get
    End Property

    Public Function Remove(ByVal item As T) As Boolean Implements System.Collections.Generic.ICollection(Of T).Remove
      Dim removed = CType(_index, ICollection(Of T)).Remove(item)

      If removed Then
        _countCache -= 1
      End If

      Return removed
    End Function

#End Region

#Region "IEnumerable<T> Members"

    Public Function GetEnumerator() As System.Collections.Generic.IEnumerator(Of T) Implements System.Collections.Generic.IEnumerable(Of T).GetEnumerator
      Dim itemList As New List(Of T)
      For Each item In CType(_index, ICollection(Of T))        
        itemList.Add(item)        
      Next
      Return itemList.GetEnumerator()
    End Function

#End Region

#Region "IEnumerable Members"

    Public Function IEnumerable_GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
      Return Me.GetEnumerator()
    End Function

#End Region

#Region "IRangeTestableIndex<T> Members"

    Public Function WhereLessThan(ByVal pivotVal As Object) As System.Collections.Generic.IEnumerable(Of T) Implements IRangeTestableIndex(Of T).WhereLessThan
      Dim balancedIndex = CType(_index, IBalancedSearch(Of T))
      Return balancedIndex.ItemsLessThan(pivotVal)
    End Function

    Public Function WhereGreaterThan(ByVal pivotVal As Object) As System.Collections.Generic.IEnumerable(Of T) Implements IRangeTestableIndex(Of T).WhereGreaterThan
      Dim balancedIndex = CType(_index, IBalancedSearch(Of T))
      Return balancedIndex.ItemsGreaterThan(pivotVal)
    End Function

    Public Function WhereLessThanOrEqualTo(ByVal pivotVal As Object) As System.Collections.Generic.IEnumerable(Of T) Implements IRangeTestableIndex(Of T).WhereLessThanOrEqualTo
      Dim balancedIndex = CType(_index, IBalancedSearch(Of T))
      Return balancedIndex.ItemsLessThanOrEqualTo(pivotVal)
    End Function

    Public Function WhereGreaterThanOrEqualTo(ByVal pivotVal As Object) As System.Collections.Generic.IEnumerable(Of T) Implements IRangeTestableIndex(Of T).WhereGreaterThanOrEqualTo
      Dim balancedIndex = CType(_index, IBalancedSearch(Of T))
      Return balancedIndex.ItemsGreaterThanOrEqualTo(pivotVal)
    End Function

#End Region

  End Class

End Namespace

