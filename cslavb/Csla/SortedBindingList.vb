Imports System
Imports System.ComponentModel
Imports System.Collections.Generic
Imports System.Collections

''' <summary>
''' Provides a sorted view into an existing IList(Of T).
''' </summary>
''' <typeparam name="T">
''' Type of child object contained by
''' the original list or collection.
''' </typeparam>
Public Class SortedBindingList(Of T)

  Implements IList(Of T)
  Implements IBindingList
  Implements IEnumerable(Of T)
  Implements ICancelAddNew

#Region " ListItem class "

  Private Class ListItem
    Implements IComparable(Of ListItem)

    Private _key As Object
    Private _baseIndex As Integer

    Public ReadOnly Property Key() As Object
      Get
        Return _key
      End Get
    End Property

    Public Property BaseIndex() As Integer
      Get
        Return _baseIndex
      End Get
      Set(ByVal value As Integer)
        _baseIndex = value
      End Set
    End Property

    Public Sub New(ByVal key As Object, ByVal baseIndex As Integer)
      _key = key
      _baseIndex = baseIndex
    End Sub

    Public Function CompareTo(ByVal other As ListItem) As Integer _
      Implements System.IComparable(Of ListItem).CompareTo

      Dim target As Object = other.Key

      If TypeOf Key Is IComparable Then
        Return DirectCast(Key, IComparable).CompareTo(target)

      Else
        If Key Is Nothing Then
          If target Is Nothing Then
            Return 0

          Else
            Return -1
          End If

        ElseIf Key.Equals(target) Then
          Return 0

        Else
          Return Key.ToString.CompareTo(target.ToString)
        End If
      End If

    End Function

    Public Overrides Function ToString() As String
      Return Key.ToString
    End Function

  End Class

#End Region

#Region " Sorted enumerator "

  Private Class SortedEnumerator

    Implements IEnumerator(Of T)

    Private _list As IList(Of T)
    Private _sortIndex As List(Of ListItem)
    Private _sortOrder As ListSortDirection
    Private _index As Integer

    Public Sub New( _
      ByVal list As IList(Of T), _
      ByVal sortIndex As List(Of ListItem), _
      ByVal direction As ListSortDirection)

      _list = list
      _sortIndex = sortIndex
      _sortOrder = direction
      Reset()
    End Sub

    Public ReadOnly Property Current() As T _
      Implements System.Collections.Generic.IEnumerator(Of T).Current
      Get
        Return _list(_sortIndex(_index).BaseIndex)
      End Get
    End Property

    Private ReadOnly Property CurrentItem() As Object _
      Implements System.Collections.IEnumerator.Current
      Get
        Return _list(_sortIndex(_index).BaseIndex)
      End Get
    End Property

    Public Function MoveNext() As Boolean _
    Implements System.Collections.IEnumerator.MoveNext

      If _sortOrder = ListSortDirection.Ascending Then
        If _index < _sortIndex.Count - 1 Then
          _index += 1
          Return True

        Else
          Return False
        End If

      Else
        If _index > 0 Then
          _index -= 1
          Return True

        Else
          Return False
        End If
      End If

    End Function

    Public Sub Reset() Implements System.Collections.IEnumerator.Reset
      If _sortOrder = ListSortDirection.Ascending Then
        _index = -1
      Else
        _index = _sortIndex.Count
      End If
    End Sub

#Region " IDisposable Support "

    Private disposedValue As Boolean = False    ' To detect redundant calls

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
      If Not Me.disposedValue Then
        If disposing Then
          ' TODO: free unmanaged resources when explicitly called
        End If

        ' TODO: free shared unmanaged resources
      End If
      Me.disposedValue = True
    End Sub

    ' This code added by Visual Basic to 
    ' correctly implement the disposable pattern.
    Public Sub Dispose() Implements IDisposable.Dispose
      ' Do not change this code.  
      ' Put cleanup code in Dispose(ByVal disposing As Boolean) above.
      Dispose(True)
      GC.SuppressFinalize(Me)
    End Sub

    Protected Overrides Sub Finalize()
      Dispose(False)
    End Sub

#End Region

  End Class

#End Region

#Region " Sort/Unsort "

  Private Sub DoSort()

    Dim index As Integer

    _sortIndex.Clear()

    If _sortBy Is Nothing Then
      For Each obj As T In _list
        _sortIndex.Add(New ListItem(obj, index))
        index += 1
      Next

    Else
      For Each obj As T In _list
        _sortIndex.Add(New ListItem(_sortBy.GetValue(obj), index))
        index += 1
      Next
    End If

    _sortIndex.Sort()
    _sorted = True

    OnListChanged(New ListChangedEventArgs(ListChangedType.Reset, 0))

  End Sub

  Private Sub UndoSort()

    _sortIndex.Clear()
    _sortBy = Nothing
    _sortOrder = ListSortDirection.Ascending
    _sorted = False

    OnListChanged(New ListChangedEventArgs(ListChangedType.Reset, 0))

  End Sub

#End Region

#Region " IEnumerable(Of T) "

  ''' <summary>
  ''' Returns an enumerator for the list, honoring
  ''' any sort that is active at the time.
  ''' </summary>
  Public Function GetEnumerator() As  _
    System.Collections.Generic.IEnumerator(Of T) _
    Implements System.Collections.Generic.IEnumerable(Of T).GetEnumerator

    If _sorted Then
      Return New SortedEnumerator(_list, _sortIndex, _sortOrder)
    Else
      Return _list.GetEnumerator
    End If

  End Function

#End Region

#Region " IBindingList, IList(Of T) "

  ''' <summary>
  ''' Implemented by IList source object.
  ''' </summary>
  ''' <param name="property">Property on which
  ''' to build the index.</param>
  Public Sub AddIndex(ByVal [property] As System.ComponentModel.PropertyDescriptor) Implements System.ComponentModel.IBindingList.AddIndex
    If _supportsBinding Then
      _bindingList.AddIndex([property])
    End If
  End Sub

  ''' <summary>
  ''' Implemented by IList source object.
  ''' </summary>
  Public Function AddNew() As Object _
    Implements System.ComponentModel.IBindingList.AddNew

    Dim result As Object

    If _supportsBinding Then
      _initiatedLocally = True
      result = _bindingList.AddNew
      _initiatedLocally = False
      OnListChanged(New ListChangedEventArgs( _
        ListChangedType.ItemAdded, _bindingList.Count - 1))

    Else
      result = Nothing
    End If

    Return result

  End Function

  ''' <summary>
  ''' Implemented by IList source object.
  ''' </summary>
  Public ReadOnly Property AllowEdit() As Boolean _
    Implements System.ComponentModel.IBindingList.AllowEdit
    Get
      If _supportsBinding Then
        Return _bindingList.AllowEdit

      Else
        Return False
      End If
    End Get
  End Property

  ''' <summary>
  ''' Implemented by IList source object.
  ''' </summary>
  Public ReadOnly Property AllowNew() As Boolean Implements System.ComponentModel.IBindingList.AllowNew
    Get
      If _supportsBinding Then
        Return _bindingList.AllowNew

      Else
        Return False
      End If
    End Get
  End Property

  ''' <summary>
  ''' Implemented by IList source object.
  ''' </summary>
  Public ReadOnly Property AllowRemove() As Boolean Implements System.ComponentModel.IBindingList.AllowRemove
    Get
      If _supportsBinding Then
        Return _bindingList.AllowRemove

      Else
        Return False
      End If
    End Get
  End Property

  ''' <summary>
  ''' Applies a sort to the view.
  ''' </summary>
  ''' <param name="propertyName">The text name of the property on which to sort.</param>
  ''' <param name="direction">The direction to sort the data.</param>
  Public Sub ApplySort( _
    ByVal propertyName As String, _
    ByVal direction As System.ComponentModel.ListSortDirection)

    _sortBy = Nothing

    If Not String.IsNullOrEmpty(propertyName) Then
      Dim itemType As Type = GetType(T)
      For Each prop As PropertyDescriptor In TypeDescriptor.GetProperties(itemType)
        If prop.Name = propertyName Then
          _sortBy = prop
          Exit For
        End If
      Next
    End If

    ApplySort(_sortBy, direction)

  End Sub

  ''' <summary>
  ''' Applies a sort to the view.
  ''' </summary>
  ''' <param name="property">A PropertyDescriptor for the property on which to sort.</param>
  ''' <param name="direction">The direction to sort the data.</param>
  Public Sub ApplySort( _
    ByVal [property] As System.ComponentModel.PropertyDescriptor, _
    ByVal direction As System.ComponentModel.ListSortDirection) _
    Implements System.ComponentModel.IBindingList.ApplySort

    _sortBy = [property]
    _sortOrder = direction
    DoSort()
  End Sub

  ''' <summary>
  ''' Finds an item in the view
  ''' </summary>
  ''' <param name="propertyName">Name of the property to search</param>
  ''' <param name="key">Value to find</param>
  Public Function Find(ByVal propertyName As String, ByVal key As Object) As Integer

    Dim findProperty As PropertyDescriptor = Nothing

    If Not String.IsNullOrEmpty(propertyName) Then
      Dim itemType As Type = GetType(T)
      For Each prop As PropertyDescriptor In TypeDescriptor.GetProperties(itemType)
        If prop.Name = propertyName Then
          findProperty = prop
          Exit For
        End If
      Next
    End If

    Return Find(findProperty, key)

  End Function

  ''' <summary>
  ''' Implemented by IList source object.
  ''' </summary>
  ''' <param name="key">Key value for which to search.</param>
  ''' <param name="property">Property to search for the key
  ''' value.</param>
  Public Function Find(ByVal [property] As System.ComponentModel.PropertyDescriptor, ByVal key As Object) As Integer Implements System.ComponentModel.IBindingList.Find
    If _supportsBinding Then
      Return SortedIndex(_bindingList.Find([property], key))
    Else
      Return -1
    End If
  End Function

  ''' <summary>
  ''' Gets a value indicating whether the view is currently sorted.
  ''' </summary>
  Public ReadOnly Property IsSorted() As Boolean Implements System.ComponentModel.IBindingList.IsSorted
    Get
      Return _sorted
    End Get
  End Property

  ''' <summary>
  ''' Raised to indicate that the list's data has changed.
  ''' </summary>
  ''' <remarks>
  ''' This event is raised if the underling IList object's data changes
  ''' (assuming the underling IList also implements the IBindingList
  ''' interface). It is also raised if the sort property or direction
  ''' is changed to indicate that the view's data has changed. See
  ''' Chapter 5 for details.
  ''' </remarks>
  Public Event ListChanged(ByVal sender As Object, ByVal e As System.ComponentModel.ListChangedEventArgs) Implements System.ComponentModel.IBindingList.ListChanged

  ''' <summary>
  ''' Raises the <see cref="ListChanged"/> event.
  ''' </summary>
  ''' <param name="e">Event arguments.</param>
  Protected Sub OnListChanged(ByVal e As System.ComponentModel.ListChangedEventArgs)
    RaiseEvent ListChanged(Me, e)
  End Sub

  ''' <summary>
  ''' Implemented by IList source object.
  ''' </summary>
  ''' <param name="property">Property for which the
  ''' index should be removed.</param>
  Public Sub RemoveIndex(ByVal [property] As System.ComponentModel.PropertyDescriptor) Implements System.ComponentModel.IBindingList.RemoveIndex
    If _supportsBinding Then
      _bindingList.RemoveIndex([property])
    End If
  End Sub

  ''' <summary>
  ''' Removes any sort currently applied to the view.
  ''' </summary>
  Public Sub RemoveSort() Implements System.ComponentModel.IBindingList.RemoveSort
    UndoSort()
  End Sub

  ''' <summary>
  ''' Returns the direction of the current sort.
  ''' </summary>
  Public ReadOnly Property SortDirection() As  _
    System.ComponentModel.ListSortDirection _
    Implements System.ComponentModel.IBindingList.SortDirection
    Get
      Return _sortOrder
    End Get
  End Property

  ''' <summary>
  ''' Returns the PropertyDescriptor of the current sort.
  ''' </summary>
  Public ReadOnly Property SortProperty() As System.ComponentModel.PropertyDescriptor Implements System.ComponentModel.IBindingList.SortProperty
    Get
      Return _sortBy
    End Get
  End Property

  ''' <summary>
  ''' Returns <see langword="true"/> since this object does raise the
  ''' ListChanged event.
  ''' </summary>
  Public ReadOnly Property SupportsChangeNotification() As Boolean Implements System.ComponentModel.IBindingList.SupportsChangeNotification
    Get
      Return True
    End Get
  End Property

  ''' <summary>
  ''' Implemented by IList source object.
  ''' </summary>
  Public ReadOnly Property SupportsSearching() As Boolean Implements System.ComponentModel.IBindingList.SupportsSearching
    Get
      If _supportsBinding Then
        Return _bindingList.SupportsSearching

      Else
        Return False
      End If
    End Get
  End Property

  ''' <summary>
  ''' Returns <see langword="true"/>. Sorting is supported.
  ''' </summary>
  Public ReadOnly Property SupportsSorting() As Boolean Implements System.ComponentModel.IBindingList.SupportsSorting
    Get
      Return True
    End Get
  End Property

  ''' <summary>
  ''' Implemented by IList source object.
  ''' </summary>
  ''' <param name="array">Array to receive the data.</param>
  ''' <param name="arrayIndex">Starting array index.</param>
  Public Sub CopyTo(ByVal array As T(), ByVal arrayIndex As Integer) Implements IList(Of T).CopyTo
    Dim pos As Integer = arrayIndex
    For Each child As T In Me
      array(pos) = child
      pos += 1
    Next
  End Sub

  Private Sub ICollection_CopyTo(ByVal array As System.Array, ByVal index As Integer) Implements System.Collections.ICollection.CopyTo
    Dim tmp(array.Length) As T
    CopyTo(tmp, index)
    array.Copy(tmp, 0, array, index, array.Length)
  End Sub

  ''' <summary>
  ''' Implemented by IList source object.
  ''' </summary>
  Public ReadOnly Property Count() As Integer _
    Implements System.Collections.ICollection.Count, _
    System.Collections.Generic.ICollection(Of T).Count
    Get
      Return _list.Count
    End Get
  End Property

  Private ReadOnly Property IsSynchronized() As Boolean Implements System.Collections.ICollection.IsSynchronized
    Get
      Return False
    End Get
  End Property

  Private ReadOnly Property SyncRoot() As Object Implements System.Collections.ICollection.SyncRoot
    Get
      Return _list
    End Get
  End Property

  Private Function GetItemEnumerator() As System.Collections.IEnumerator _
    Implements System.Collections.IEnumerable.GetEnumerator

    Return GetEnumerator()
  End Function

  ''' <summary>
  ''' Implemented by IList source object.
  ''' </summary>
  ''' <param name="item">Item to add to the list.</param>
  Public Sub Add(ByVal item As T) Implements System.Collections.Generic.ICollection(Of T).Add
    _list.Add(item)
  End Sub

  Private Function Add(ByVal value As Object) As Integer Implements System.Collections.IList.Add
    Add(CType(value, T))
    Return SortedIndex(_list.Count - 1)
  End Function

  ''' <summary>
  ''' Implemented by IList source object.
  ''' </summary>
  Public Sub Clear() Implements System.Collections.IList.Clear, System.Collections.Generic.ICollection(Of T).Clear
    _list.Clear()
  End Sub

  ''' <summary>
  ''' Implemented by IList source object.
  ''' </summary>
  ''' <param name="item">Item for which to search.</param>
  Public Function Contains(ByVal item As T) As Boolean Implements System.Collections.Generic.ICollection(Of T).Contains
    Return _list.Contains(item)
  End Function

  Private Function Contains(ByVal value As Object) As Boolean Implements System.Collections.IList.Contains
    Return Contains(CType(value, T))
  End Function

  ''' <summary>
  ''' Implemented by IList source object.
  ''' </summary>
  ''' <param name="item">Item for which to search.</param>
  Public Function IndexOf(ByVal item As T) As Integer Implements System.Collections.Generic.IList(Of T).IndexOf
    Return SortedIndex(_list.IndexOf(item))
  End Function

  Private Function IndexOf(ByVal value As Object) As Integer Implements System.Collections.IList.IndexOf
    Return IndexOf(CType(value, T))
  End Function

  ''' <summary>
  ''' Implemented by IList source object.
  ''' </summary>
  ''' <param name="index">Index at
  ''' which to insert the item.</param>
  ''' <param name="item">Item to insert.</param>
  Public Sub Insert(ByVal index As Integer, ByVal item As T) _
    Implements System.Collections.Generic.IList(Of T).Insert

    _list.Insert(index, item)
  End Sub

  Private Sub Insert(ByVal index As Integer, ByVal value As Object) _
    Implements System.Collections.IList.Insert

    Insert(index, CType(value, T))
  End Sub

  Private ReadOnly Property IsFixedSize() As Boolean Implements System.Collections.IList.IsFixedSize
    Get
      Return False
    End Get
  End Property

  ''' <summary>
  ''' Implemented by IList source object.
  ''' </summary>
  Public ReadOnly Property IsReadOnly() As Boolean Implements System.Collections.IList.IsReadOnly, System.Collections.Generic.ICollection(Of T).IsReadOnly
    Get
      Return _list.IsReadOnly
    End Get
  End Property

  Private Property Item1(ByVal index As Integer) As Object _
    Implements System.Collections.IList.Item
    Get
      Return Me(index)
    End Get
    Set(ByVal value As Object)
      Me(index) = CType(value, T)
    End Set
  End Property

  ''' <summary>
  ''' Implemented by IList source object.
  ''' </summary>
  ''' <param name="item">Item to be removed.</param>
  Public Function Remove(ByVal item As T) As Boolean Implements System.Collections.Generic.ICollection(Of T).Remove
    Return _list.Remove(item)
  End Function

  Private Sub Remove(ByVal value As Object) Implements System.Collections.IList.Remove
    Remove(CType(value, T))
  End Sub

  ''' <summary>
  ''' Removes the child object at the specified index
  ''' in the list, resorting the display as needed.
  ''' </summary>
  ''' <param name="index">The index of the object to remove.</param>
  ''' <remarks>
  ''' See Chapter 5 for details on how and why the list is
  ''' altered during the remove process.
  ''' </remarks>
  Public Sub RemoveAt(ByVal index As Integer) _
    Implements System.Collections.IList.RemoveAt, _
    System.Collections.Generic.IList(Of T).RemoveAt

    If _sorted Then
      _initiatedLocally = True
      Dim baseIndex As Integer = OriginalIndex(index)

      ' remove the item from the source list
      _list.RemoveAt(baseIndex)

      If _list.Count <> _sortIndex.Count Then
        ' delete the corresponding value in the sort index
        If _sortOrder = ListSortDirection.Ascending Then
          _sortIndex.RemoveAt(index)
        Else
          _sortIndex.RemoveAt(_sortIndex.Count - 1 - index)
        End If

        ' now fix up all index pointers in the sort index
        For Each item As ListItem In _sortIndex
          If item.BaseIndex > baseIndex Then
            item.BaseIndex -= 1
          End If
        Next
      End If

      OnListChanged( _
        New ListChangedEventArgs(ListChangedType.ItemDeleted, index))
      _initiatedLocally = False

    Else
      _list.RemoveAt(index)
    End If

  End Sub

  ''' <summary>
  ''' Gets the child item at the specified index in the list,
  ''' honoring the sort order of the items.
  ''' </summary>
  ''' <param name="index">The index of the item in the sorted list.</param>
  Default Public Overloads Property Item(ByVal index As Integer) As T _
    Implements System.Collections.Generic.IList(Of T).Item
    Get
      If _sorted Then
        Return _list(OriginalIndex(index))
      Else
        Return _list(index)
      End If
    End Get
    Set(ByVal value As T)
      If _sorted Then
        _list(OriginalIndex(index)) = value
      Else
        _list(index) = value
      End If
    End Set
  End Property

#End Region

#Region " SourceList "

  ''' <summary>
  ''' Gets the source list over which this
  ''' SortedBindingList is a view.
  ''' </summary>
  <EditorBrowsable(EditorBrowsableState.Advanced)> _
  Public ReadOnly Property SourceList() As IList(Of T)
    Get
      Return _list
    End Get
  End Property

#End Region

  Private _list As IList(Of T)
  Private _supportsBinding As Boolean
  Private _bindingList As IBindingList
  Private _sorted As Boolean
  Private _initiatedLocally As Boolean
  Private _sortBy As PropertyDescriptor
  Private _sortOrder As ListSortDirection = ListSortDirection.Ascending
  Private _sortIndex As New List(Of ListItem)

  ''' <summary>
  ''' Creates a new view based on the provided IList object.
  ''' </summary>
  ''' <param name="list">The IList (collection) containing the data.</param>
  Public Sub New(ByVal list As IList(Of T))

    _list = list

    If TypeOf _list Is IBindingList Then
      _supportsBinding = True
      _bindingList = DirectCast(_list, IBindingList)
      AddHandler _bindingList.ListChanged, AddressOf SourceChanged
    End If

  End Sub

  Private Sub SourceChanged( _
    ByVal sender As Object, ByVal e As ListChangedEventArgs)

    If _sorted Then
      Select Case e.ListChangedType
        Case ListChangedType.ItemAdded
          Dim newItem As T = _list(e.NewIndex)
          If e.NewIndex = _list.Count - 1 Then
            Dim newKey As Object
            If _sortBy IsNot Nothing Then
              newKey = _sortBy.GetValue(newItem)
            Else
              newKey = newItem
            End If

            If _sortOrder = ListSortDirection.Ascending Then
              _sortIndex.Add(New ListItem(newKey, e.NewIndex))

            Else
              _sortIndex.Insert(0, New ListItem(newKey, e.NewIndex))
            End If
            If Not _initiatedLocally Then
              OnListChanged( _
                New ListChangedEventArgs( _
                  ListChangedType.ItemAdded, SortedIndex(e.NewIndex)))
            End If

          Else
            DoSort()
          End If

        Case ListChangedType.ItemChanged
          ' an item changed - just relay the event with
          ' a translated index value
          OnListChanged( _
            New ListChangedEventArgs( _
              ListChangedType.ItemChanged, SortedIndex(e.NewIndex), e.PropertyDescriptor))

        Case ListChangedType.ItemDeleted
          If Not _initiatedLocally Then
            DoSort()
          End If

        Case Else
          ' for anything other than add, delete or change
          ' just re-sort the list
          If Not _initiatedLocally Then
            DoSort()
          End If
      End Select

    Else
      OnListChanged(e)
    End If

  End Sub

  Private Function OriginalIndex(ByVal sortedIndex As Integer) As Integer
    If _sorted Then
      If _sortOrder = ListSortDirection.Ascending Then
        Return _sortIndex.Item(sortedIndex).BaseIndex
      Else
        Return _sortIndex.Item(_sortIndex.Count - 1 - sortedIndex).BaseIndex
      End If
    Else
      Return sortedIndex
    End If
    
  End Function

  Private Function SortedIndex(ByVal originalIndex As Integer) As Integer

    Dim result As Integer
    If _sorted Then
      For index As Integer = 0 To _sortIndex.Count - 1
        If _sortIndex(index).BaseIndex = originalIndex Then
          result = index
          Exit For
        End If
      Next
      If _sortOrder = ListSortDirection.Descending Then
        result = _sortIndex.Count - 1 - result
      End If
    Else
      result = originalIndex
    End If
    Return result

  End Function

#Region " ICancelAddNew "

  Public Sub CancelNew(ByVal itemIndex As Integer) Implements System.ComponentModel.ICancelAddNew.CancelNew

    Dim can As ICancelAddNew = TryCast(_list, ICancelAddNew)
    If can IsNot Nothing Then
      can.CancelNew(itemIndex)

    Else
      _list.RemoveAt(itemIndex)
    End If

  End Sub

  Public Sub EndNew(ByVal itemIndex As Integer) Implements System.ComponentModel.ICancelAddNew.EndNew

    Dim can As ICancelAddNew = TryCast(_list, ICancelAddNew)
    If can IsNot Nothing Then
      can.EndNew(itemIndex)
    End If

  End Sub

#End Region

#Region " ToArray "

  ''' <summary>
  ''' Get an array containing all items in the list.
  ''' </summary>
  Public Function ToArray() As T()

    Dim result As New List(Of T)
    For Each Item As T In Me
      result.Add(Item)
    Next
    Return result.ToArray

  End Function

#End Region

End Class
