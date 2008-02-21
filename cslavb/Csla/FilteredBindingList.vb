Imports System.ComponentModel
Imports System.Reflection

''' <summary>
''' Provides a filtered view into an existing IList(Of T).
''' </summary>
''' <typeparam name="T">The type of the objects contained
''' in the original list.</typeparam>
Public Class FilteredBindingList(Of T)

  Implements IList(Of T)
  Implements IBindingList
  Implements IEnumerable(Of T)
  Implements ICancelAddNew

#Region "ListItem class"

  Private Class ListItem

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

    Public Overrides Function ToString() As String
      Return Key.ToString
    End Function

  End Class

#End Region

#Region "Filtered enumerator"

  Private Class FilteredEnumerator

    Implements IEnumerator(Of T)

    Private _list As IList(Of T)
    Private _filterIndex As List(Of ListItem)
    Private _index As Integer

    Public Sub New(ByVal list As IList(Of T), ByVal filterIndex As List(Of ListItem))
      _list = list
      _filterIndex = filterIndex
      Reset()
    End Sub

    Public ReadOnly Property Current() As T Implements IEnumerator(Of T).Current
      Get
        Return _list(_filterIndex(_index).BaseIndex)
      End Get
    End Property

    Private ReadOnly Property IEnumerator_Current() As Object Implements System.Collections.IEnumerator.Current
      Get
        Return _list(_filterIndex(_index).BaseIndex)
      End Get
    End Property

    Public Function MoveNext() As Boolean Implements IEnumerator(Of T).MoveNext
      If _index < _filterIndex.Count - 1 Then
        _index += 1
        Return True
      Else
        Return False
      End If
    End Function

    Public Sub Reset() Implements IEnumerator(Of T).Reset
      _index = -1
    End Sub

#Region "IDisposable Support"

    Private _disposed As Boolean = False ' To detect redundant calls.

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
      If (Not _disposed) Then
        If disposing Then
          ' TODO: free unmanaged resources when explicitly called
        End If
        ' TODO: free shared unmanaged resources
      End If
      _disposed = True
    End Sub

    ' this code added to correctly implement the disposable pattern.
    Public Sub Dispose() Implements IEnumerator(Of T).Dispose
      ' Do not change this code.  Put cleanup code in Dispose(bool disposing) above.
      Dispose(True)
      GC.SuppressFinalize(Me)
    End Sub

    Protected Overrides Sub Finalize()
      Dispose(False)
    End Sub

#End Region

  End Class

#End Region

#Region "Filter/Unfilter"

  Private Sub DoFilter()
    Dim index As Integer = 0
    _filterIndex.Clear()

    If _provider Is Nothing Then
      _provider = AddressOf DefaultFilter.Filter
    End If

    If _filterBy Is Nothing Then
      For Each obj As T In _list
        If _provider.Invoke(obj, _filter) Then
          _filterIndex.Add(New ListItem(obj, index))
        End If
        index += 1
      Next obj
    Else
      For Each obj As T In _list
        Dim tmp As Object = _filterBy.GetValue(obj)
        If _provider.Invoke(tmp, _filter) Then
          _filterIndex.Add(New ListItem(tmp, index))
        End If
        index += 1
      Next obj
    End If

    _filtered = True

    OnListChanged(New ListChangedEventArgs(ListChangedType.Reset, 0))

  End Sub

  Private Sub UnDoFilter()
    _filterIndex.Clear()
    _filterBy = Nothing
    _filter = Nothing
    _filtered = False

    OnListChanged(New ListChangedEventArgs(ListChangedType.Reset, 0))

  End Sub

#End Region

#Region "IEnumerable<T>"

  ''' <summary>
  ''' Gets an enumerator object.
  ''' </summary>
  Public Function GetEnumerator() As IEnumerator(Of T) Implements IEnumerable(Of T).GetEnumerator
    If _filtered Then
      Return New FilteredEnumerator(_list, _filterIndex)
    Else
      Return _list.GetEnumerator()
    End If
  End Function

#End Region

#Region "IBindingList, IList<T>"

  ''' <summary>
  ''' Implemented by IList source object.
  ''' </summary>
  ''' <param name="property">Property on which
  ''' to build the index.</param>
  Public Sub AddIndex(ByVal [property] As PropertyDescriptor) Implements IBindingList.AddIndex
    If _supportsBinding Then
      _bindingList.AddIndex([property])
    End If
  End Sub

  ''' <summary>
  ''' Implemented by IList source object.
  ''' </summary>
  Public Function AddNew() As Object Implements IBindingList.AddNew
    Dim result As T
    If _supportsBinding Then
      result = CType(_bindingList.AddNew(), T)
    Else
      result = Nothing
    End If

    Return result
  End Function

  ''' <summary>
  ''' Implemented by IList source object.
  ''' </summary>
  Public ReadOnly Property AllowEdit() As Boolean Implements IBindingList.AllowEdit
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
  Public ReadOnly Property AllowNew() As Boolean Implements IBindingList.AllowNew
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
  Public ReadOnly Property AllowRemove() As Boolean Implements IBindingList.AllowRemove
    Get
      If _supportsBinding Then
        Return _bindingList.AllowRemove
      Else
        Return False
      End If
    End Get
  End Property

  ''' <summary>
  ''' Sorts the list if the original list
  ''' supports sorting.
  ''' </summary>
  ''' <param name="property">Property on which to sort.</param>
  ''' <param name="direction">Direction of the sort.</param>
  Public Sub ApplySort(ByVal [property] As PropertyDescriptor, ByVal direction As ListSortDirection) Implements IBindingList.ApplySort
    If SupportsSorting Then
      _bindingList.ApplySort([property], direction)
    Else
      Throw New NotSupportedException("Sorting not supported.")
    End If
  End Sub

  ''' <summary>
  ''' Finds an item in the view
  ''' </summary>
  ''' <param name="propertyName">Name of the property to search</param>
  ''' <param name="key">Value to find</param>
  Public Function Find(ByVal propertyName As String, ByVal key As Object) As Integer
    Dim findProperty As PropertyDescriptor = Nothing

    If (Not String.IsNullOrEmpty(propertyName)) Then
      Dim itemType As Type = GetType(T)
      For Each prop As PropertyDescriptor In TypeDescriptor.GetProperties(itemType)
        If prop.Name = propertyName Then
          findProperty = prop
          Exit For
        End If
      Next prop
    End If

    Return Find(findProperty, key)

  End Function

  ''' <summary>
  ''' Implemented by IList source object.
  ''' </summary>
  ''' <param name="key">Key value for which to search.</param>
  ''' <param name="property">Property to search for the key
  ''' value.</param>
  Public Function Find(ByVal [property] As PropertyDescriptor, ByVal key As Object) As Integer Implements IBindingList.Find
    If _supportsBinding Then
      Return FilteredIndex(_bindingList.Find([property], key))
    Else
      Return -1
    End If
  End Function

  ''' <summary>
  ''' Returns True if the view is currently sorted.
  ''' </summary>
  Public ReadOnly Property IsSorted() As Boolean Implements IBindingList.IsSorted
    Get
      If SupportsSorting Then
        Return _bindingList.IsSorted
      Else
        Return False
      End If
    End Get
  End Property

  ''' <summary>
  ''' Raised to indicate that the list's data has changed.
  ''' </summary>
  ''' <remarks>
  ''' This event is raised if the underling IList object's data changes
  ''' (assuming the underling IList also implements the IBindingList
  ''' interface). It is also raised if the filter
  ''' is changed to indicate that the view's data has changed.
  ''' </remarks>
  Public Event ListChanged As ListChangedEventHandler Implements IBindingList.ListChanged

  ''' <summary>
  ''' Raises the ListChanged event.
  ''' </summary>
  ''' <param name="e">Parameter for the event.</param>
  Protected Sub OnListChanged(ByVal e As ListChangedEventArgs)
    If Not ListChangedEvent Is Nothing Then
      RaiseEvent ListChanged(Me, e)
    End If
  End Sub

  ''' <summary>
  ''' Implemented by IList source object.
  ''' </summary>
  ''' <param name="property">Property for which the
  ''' index should be removed.</param>
  Public Sub RemoveIndex(ByVal [property] As PropertyDescriptor) Implements IBindingList.RemoveIndex
    If _supportsBinding Then
      _bindingList.RemoveIndex([property])
    End If
  End Sub

  ''' <summary>
  ''' Removes any sort currently applied to the view.
  ''' </summary>
  Public Sub RemoveSort() Implements IBindingList.RemoveSort
    If SupportsSorting Then
      _bindingList.RemoveSort()
    Else
      Throw New NotSupportedException("Sorting not supported")
    End If
  End Sub

  ''' <summary>
  ''' Returns the direction of the current sort.
  ''' </summary>
  Public ReadOnly Property SortDirection() As ListSortDirection Implements IBindingList.SortDirection
    Get
      If SupportsSorting Then
        Return _bindingList.SortDirection
      Else
        Return ListSortDirection.Ascending
      End If
    End Get
  End Property

  ''' <summary>
  ''' Returns the PropertyDescriptor of the current sort.
  ''' </summary>
  Public ReadOnly Property SortProperty() As PropertyDescriptor Implements IBindingList.SortProperty
    Get
      If SupportsSorting Then
        Return _bindingList.SortProperty
      Else
        Return Nothing
      End If
    End Get
  End Property

  ''' <summary>
  ''' Returns True since this object does raise the
  ''' ListChanged event.
  ''' </summary>
  Public ReadOnly Property SupportsChangeNotification() As Boolean Implements IBindingList.SupportsChangeNotification
    Get
      Return True
    End Get
  End Property

  ''' <summary>
  ''' Implemented by IList source object.
  ''' </summary>
  Public ReadOnly Property SupportsSearching() As Boolean Implements IBindingList.SupportsSearching
    Get
      If _supportsBinding Then
        Return _bindingList.SupportsSearching
      Else
        Return False
      End If
    End Get
  End Property

  ''' <summary>
  ''' Returns True. Sorting is supported.
  ''' </summary>
  Public ReadOnly Property SupportsSorting() As Boolean Implements IBindingList.SupportsSorting
    Get
      If _supportsBinding Then
        Return _bindingList.SupportsSorting
      Else
        Return False
      End If
    End Get
  End Property

  ''' <summary>
  ''' Copies the contents of the list to
  ''' an array.
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
  ''' Gets the number of items in the list.
  ''' </summary>
  Public ReadOnly Property Count() As Integer Implements IList(Of T).Count, IBindingList.Count
    Get
      If _filtered Then
        Return _filterIndex.Count
      Else
        Return _list.Count
      End If
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

  Private Function IEnumerable_GetEnumerator() As IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
    Return GetEnumerator()
  End Function

  ''' <summary>
  ''' Adds an item to the list.
  ''' </summary>
  ''' <param name="item">Item to be added.</param>
  Public Sub Add(ByVal item As T) Implements IList(Of T).Add
    _list.Add(item)
  End Sub

  Private Function Add(ByVal value As Object) As Integer Implements System.Collections.IList.Add
    Add(CType(value, T))
    Dim index As Integer = FilteredIndex(_list.Count - 1)
    If index > -1 Then
      Return index
    Else
      Return 0
    End If
  End Function

  ''' <summary>
  ''' Clears the list.
  ''' </summary>
  Public Sub Clear() Implements IList(Of T).Clear, IBindingList.Clear
    _list.Clear()
  End Sub

  ''' <summary>
  ''' Determines whether the specified
  ''' item is contained in the list.
  ''' </summary>
  ''' <param name="item">Item to find.</param>
  ''' <returns><see langword="true"/> if the item is
  ''' contained in the list.</returns>
  Public Function Contains(ByVal item As T) As Boolean Implements IList(Of T).Contains
    Return _list.Contains(item)
  End Function

  Private Function Contains(ByVal value As Object) As Boolean Implements System.Collections.IList.Contains
    Return Contains(CType(value, T))
  End Function

  ''' <summary>
  ''' Gets the 0-based index of an
  ''' item in the list.
  ''' </summary>
  ''' <param name="item">The item to find.</param>
  ''' <returns>0-based index of the item
  ''' in the list.</returns>
  Public Function IndexOf(ByVal item As T) As Integer Implements IList(Of T).IndexOf
    Return FilteredIndex(_list.IndexOf(item))
  End Function

  Private Function IndexOf(ByVal value As Object) As Integer Implements System.Collections.IList.IndexOf
    Return IndexOf(CType(value, T))
  End Function

  ''' <summary>
  ''' Inserts an item into the list.
  ''' </summary>
  ''' <param name="index">Index at
  ''' which to insert the item.</param>
  ''' <param name="item">Item to insert.</param>
  Public Sub Insert(ByVal index As Integer, ByVal item As T) Implements IList(Of T).Insert
    _list.Insert(index, item)
  End Sub

  Private Sub Insert(ByVal index As Integer, ByVal value As Object) Implements System.Collections.IList.Insert
    Insert(index, CType(value, T))
  End Sub

  Private ReadOnly Property IsFixedSize() As Boolean Implements System.Collections.IList.IsFixedSize
    Get
      Return False
    End Get
  End Property

  ''' <summary>
  ''' Gets a value indicating whether the list
  ''' is read-only.
  ''' </summary>
  Public ReadOnly Property IsReadOnly() As Boolean Implements IList(Of T).IsReadOnly, IBindingList.IsReadOnly
    Get
      Return _list.IsReadOnly
    End Get
  End Property

  Private Property IList_Item(ByVal index As Integer) As Object Implements System.Collections.IList.Item
    Get
      Return Me(index)
    End Get
    Set(ByVal value As Object)
      Me(index) = CType(value, T)
    End Set
  End Property

  ''' <summary>
  ''' Removes an item from the list.
  ''' </summary>
  ''' <param name="item">Item to remove.</param>
  ''' <returns><see langword="true"/> if the 
  ''' remove succeeds.</returns>
  Public Function Remove(ByVal item As T) As Boolean Implements IList(Of T).Remove
    Return _list.Remove(item)
  End Function

  Private Sub Remove(ByVal value As Object) Implements System.Collections.IList.Remove
    Remove(CType(value, T))
  End Sub

  ''' <summary>
  ''' Removes an item from the list.
  ''' </summary>
  ''' <param name="index">Index of item
  ''' to be removed.</param>
  Public Sub RemoveAt(ByVal index As Integer) Implements IList(Of T).RemoveAt, IBindingList.RemoveAt
    If _filtered Then
      _list.RemoveAt(OriginalIndex(index))
    Else
      _list.RemoveAt(index)
    End If
  End Sub

  ''' <summary>
  ''' Gets or sets the 
  ''' item at the specified
  ''' index.
  ''' </summary>
  ''' <param name="index">Index of the item.</param>
  ''' <returns>Item at the specified index.</returns>
  Default Public Property Item(ByVal index As Integer) As T Implements IList(Of T).Item
    Get
      If _filtered Then
        Dim src As Integer = OriginalIndex(index)
        Return _list(src)
      Else
        Return _list(index)
      End If
    End Get
    Set(ByVal value As T)
      If _filtered Then
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
  Private _filtered As Boolean
  Private _filterBy As PropertyDescriptor
  Private _filter As Object
  Private _provider As FilterProvider = Nothing
  Private _filterIndex As List(Of ListItem) = New List(Of ListItem)()

  ''' <summary>
  ''' Creates a new view based on the provided IList object.
  ''' </summary>
  ''' <param name="list">The IList (collection) containing the data.</param>
  Public Sub New(ByVal list As IList(Of T))

    _list = list
    If TypeOf _list Is IBindingList Then
      _supportsBinding = True
      _bindingList = CType(_list, IBindingList)
      AddHandler _bindingList.ListChanged, AddressOf SourceChanged
    End If

  End Sub

  ''' <summary>
  ''' Creates a new view based on the provided IList object.
  ''' </summary>
  ''' <param name="list">The IList (collection) containing the data.</param>
  ''' <param name="filterProvider">
  ''' Delegate pointer to a method that implements the filter behavior.
  ''' </param>
  Public Sub New(ByVal list As IList(Of T), ByVal filterProvider As FilterProvider)

    Me.New(list)
    _provider = filterProvider

  End Sub

  ''' <summary>
  ''' Gets or sets the filter provider method.
  ''' </summary>
  ''' <value>
  ''' Delegate pointer to a method that implements the filter behavior.
  ''' </value>
  ''' <returns>
  ''' Delegate pointer to a method that implements the filter behavior.
  ''' </returns>
  ''' <remarks>
  ''' If this value is set to Nothing (null in C#) then the default
  ''' filter provider, <see cref="DefaultFilter" /> will be used.
  ''' </remarks>
  Public Property FilterProvider() As FilterProvider
    Get
      Return _provider
    End Get
    Set(ByVal value As FilterProvider)
      _provider = value
    End Set
  End Property

  ''' <summary>
  ''' The property on which the items will be filtered.
  ''' </summary>
  ''' <value>A descriptor for the property on which
  ''' the items in the collection will be filtered.</value>
  ''' <returns></returns>
  ''' <remarks></remarks>
  Public ReadOnly Property FilterProperty() As PropertyDescriptor
    Get
      Return _filterBy
    End Get
  End Property

  ''' <summary>
  ''' Returns True if the view is currently filtered.
  ''' </summary>
  Public ReadOnly Property IsFiltered() As Boolean
    Get
      Return _filtered
    End Get
  End Property

  ''' <summary>
  ''' Applies a filter to the view using the
  ''' most recently used property name and
  ''' filter provider.
  ''' </summary>
  Public Sub ApplyFilter()

    If _filterBy Is Nothing OrElse _filter Is Nothing Then
      Throw New ArgumentNullException(My.Resources.FilterRequiredException)
    End If
    DoFilter()

  End Sub

  ''' <summary>
  ''' Applies a filter to the view.
  ''' </summary>
  ''' <param name="propertyName">The text name of the property on which to filter.</param>
  ''' <param name="filter">The filter criteria.</param>
  Public Sub ApplyFilter(ByVal propertyName As String, ByVal filter As Object)
    _filterBy = Nothing
    _filter = filter

    If (Not String.IsNullOrEmpty(propertyName)) Then
      Dim itemType As Type = GetType(T)
      For Each prop As PropertyDescriptor In TypeDescriptor.GetProperties(itemType)
        If prop.Name = propertyName Then
          _filterBy = prop
          Exit For
        End If
      Next prop
    End If

    ApplyFilter(_filterBy, filter)

  End Sub

  ''' <summary>
  ''' Applies a filter to the view.
  ''' </summary>
  ''' <param name="property">A PropertyDescriptor for the property on which to filter.</param>
  ''' <param name="filter">The filter criteria.</param>
  Public Sub ApplyFilter(ByVal [property] As PropertyDescriptor, ByVal filter As Object)
    _filterBy = [property]
    _filter = filter
    DoFilter()
  End Sub

  ''' <summary>
  ''' Removes the filter from the list,
  ''' so the view reflects the state of
  ''' the original list.
  ''' </summary>
  Public Sub RemoveFilter()
    UnDoFilter()
  End Sub

  Private Sub SourceChanged(ByVal sender As Object, ByVal e As ListChangedEventArgs)
    If _filtered Then
      Dim listIndex As Integer
      Dim filteredIndexValue As Integer = -1
      Dim newItem As T
      Dim newKey As Object
      Select Case e.ListChangedType
        Case ListChangedType.ItemAdded
          listIndex = e.NewIndex
          ' add new value to index
          newItem = _list(listIndex)
          If Not _filterBy Is Nothing Then
            newKey = _filterBy.GetValue(newItem)
          Else
            newKey = newItem
          End If
          _filterIndex.Add(New ListItem(newKey, listIndex))
          filteredIndexValue = _filterIndex.Count - 1
          ' raise event 
          OnListChanged(New ListChangedEventArgs(e.ListChangedType, filteredIndexValue))

        Case ListChangedType.ItemChanged
          listIndex = e.NewIndex
          ' update index value
          filteredIndexValue = FilteredIndex(listIndex)
          If filteredIndexValue <> -1 Then
            newItem = _list(listIndex)
            If Not _filterBy Is Nothing Then
              newKey = _filterBy.GetValue(newItem)
            Else
              newKey = newItem
            End If
            _filterIndex(filteredIndexValue) = New ListItem(newKey, listIndex)
          End If
          ' raise event if appropriate
          If filteredIndexValue > -1 Then
            OnListChanged( _
              New ListChangedEventArgs(e.ListChangedType, filteredIndexValue, e.PropertyDescriptor))
          End If

        Case ListChangedType.ItemDeleted
          listIndex = e.NewIndex
          ' delete corresponding item from index
          ' (if any)
          filteredIndexValue = FilteredIndex(listIndex)
          If filteredIndexValue <> -1 Then
            _filterIndex.RemoveAt(filteredIndexValue)
          End If
          ' adjust index xref values
          For Each item As ListItem In _filterIndex
            If item.BaseIndex > e.NewIndex Then
              item.BaseIndex -= 1
            End If
          Next item
          ' raise event if appropriate
          If filteredIndexValue > -1 Then
            OnListChanged(New ListChangedEventArgs(e.ListChangedType, filteredIndexValue))
          End If

        Case ListChangedType.PropertyDescriptorAdded, ListChangedType.PropertyDescriptorChanged, ListChangedType.PropertyDescriptorDeleted
          OnListChanged(e)

        Case Else
          DoFilter()
          OnListChanged(New ListChangedEventArgs(ListChangedType.Reset, 0))
      End Select
    Else
      OnListChanged(e)
    End If
  End Sub

  Private Function OriginalIndex(ByVal filteredIndex As Integer) As Integer
    Return _filterIndex(filteredIndex).BaseIndex
  End Function

  Private Function FilteredIndex(ByVal originalIndex As Integer) As Integer

    Dim result As Integer = -1
    If _filtered Then
      Dim index As Integer = 0
      Do While index < _filterIndex.Count
        If _filterIndex(index).BaseIndex = originalIndex Then
          result = index
          Exit Do
        End If
        index += 1
      Loop

    Else
      result = originalIndex
    End If
    Return result

  End Function

#Region " ICancelAddNew Members "

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
