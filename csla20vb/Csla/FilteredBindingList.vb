Imports System
Imports System.ComponentModel
Imports System.Collections.Generic
Imports System.Reflection
Imports System.Collections

''' <summary>
''' Provides a filtered view into an existing IList(Of T).
''' </summary>
Public Class FilteredBindingList(Of T)
  Implements IList(Of T), IBindingList, IEnumerable(Of T), ICancelAddNew

#Region "ListItem class"

  Private Class ListItem

    Private mKey As Object
    Private mBaseIndex As Integer

    Public ReadOnly Property Key() As Object
      Get
        Return mKey
      End Get
    End Property

    Public Property BaseIndex() As Integer
      Get
        Return mBaseIndex
      End Get
      Set(ByVal value As Integer)
        mBaseIndex = value
      End Set
    End Property

    Public Sub New(ByVal key As Object, ByVal baseIndex As Integer)
      mKey = key
      mBaseIndex = baseIndex
    End Sub

    Public Overrides Function ToString() As String
      Return Key.ToString
    End Function

  End Class

#End Region

#Region "Filtered enumerator"

  Private Class FilteredEnumerator

    Implements IEnumerator(Of T)

    Private mList As IList(Of T)
    Private mFilterIndex As List(Of ListItem)
    Private mIndex As Integer

    Public Sub New(ByVal list As IList(Of T), ByVal filterIndex As List(Of ListItem))
      mList = list
      mFilterIndex = filterIndex
      Reset()
    End Sub

    Public ReadOnly Property Current() As T Implements IEnumerator(Of T).Current
      Get
        Return mList(mFilterIndex(mIndex).BaseIndex)
      End Get
    End Property

    Private ReadOnly Property IEnumerator_Current() As Object Implements System.Collections.IEnumerator.Current
      Get
        Return mList(mFilterIndex(mIndex).BaseIndex)
      End Get
    End Property

    Public Function MoveNext() As Boolean Implements IEnumerator(Of T).MoveNext
      If mIndex < mFilterIndex.Count - 1 Then
        mIndex += 1
        Return True
      Else
        Return False
      End If
    End Function

    Public Sub Reset() Implements IEnumerator(Of T).Reset
      mIndex = -1
    End Sub

#Region "IDisposable Support"

    Private mDisposedValue As Boolean = False ' To detect redundant calls.

    ' IDisposable
    Protected Overridable Sub Dispose(ByVal disposing As Boolean)
      If (Not mDisposedValue) Then
        If disposing Then
          ' TODO: free unmanaged resources when explicitly called
        End If
        ' TODO: free shared unmanaged resources
      End If
      mDisposedValue = True
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
    mFilterIndex.Clear()

    If mProvider Is Nothing Then
      mProvider = AddressOf DefaultFilter.Filter
    End If

    If mFilterBy Is Nothing Then
      For Each obj As T In mList
        If mProvider.Invoke(obj, mFilter) Then
          mFilterIndex.Add(New ListItem(obj, index))
        End If
        index += 1
      Next obj
    Else
      For Each obj As T In mList
        Dim tmp As Object = mFilterBy.GetValue(obj)
        If mProvider.Invoke(tmp, mFilter) Then
          mFilterIndex.Add(New ListItem(tmp, index))
        End If
        index += 1
      Next obj
    End If

    mFiltered = True

    OnListChanged(New ListChangedEventArgs(ListChangedType.Reset, 0))

  End Sub

  Private Sub UnDoFilter()
    mFilterIndex.Clear()
    mFilterBy = Nothing
    mFilter = Nothing
    mFiltered = False

    OnListChanged(New ListChangedEventArgs(ListChangedType.Reset, 0))

  End Sub

#End Region

#Region "IEnumerable<T>"

  Public Function GetEnumerator() As IEnumerator(Of T) Implements IEnumerable(Of T).GetEnumerator
    If mFiltered Then
      Return New FilteredEnumerator(mList, mFilterIndex)
    Else
      Return mList.GetEnumerator()
    End If
  End Function

#End Region

#Region "IBindingList, IList<T>"

  ''' <summary>
  ''' Implemented by IList source object.
  ''' </summary>
  Public Sub AddIndex(ByVal [property] As PropertyDescriptor) Implements IBindingList.AddIndex
    If mSupportsBinding Then
      mBindingList.AddIndex([property])
    End If
  End Sub

  ''' <summary>
  ''' Implemented by IList source object.
  ''' </summary>
  Public Function AddNew() As Object Implements IBindingList.AddNew
    Dim result As T
    If mSupportsBinding Then
      result = CType(mBindingList.AddNew(), T)
    Else
      result = Nothing
    End If

    mNewItem = CType(result, T)
    Return result
  End Function

  ''' <summary>
  ''' Implemented by IList source object.
  ''' </summary>
  Public ReadOnly Property AllowEdit() As Boolean Implements IBindingList.AllowEdit
    Get
      If mSupportsBinding Then
        Return mBindingList.AllowEdit
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
      If mSupportsBinding Then
        Return mBindingList.AllowNew
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
      If mSupportsBinding Then
        Return mBindingList.AllowRemove
      Else
        Return False
      End If
    End Get
  End Property

  Public Sub ApplySort(ByVal [property] As PropertyDescriptor, ByVal direction As ListSortDirection) Implements IBindingList.ApplySort
    If SupportsSorting Then
      mBindingList.ApplySort([property], direction)
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
  Public Function Find(ByVal [property] As PropertyDescriptor, ByVal key As Object) As Integer Implements IBindingList.Find
    If mSupportsBinding Then
      Return mBindingList.Find([property], key)
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
        Return mBindingList.IsSorted
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

  Protected Sub OnListChanged(ByVal e As ListChangedEventArgs)
    If Not ListChangedEvent Is Nothing Then
      RaiseEvent ListChanged(Me, e)
    End If
  End Sub

  ''' <summary>
  ''' Implemented by IList source object.
  ''' </summary>
  Public Sub RemoveIndex(ByVal [property] As PropertyDescriptor) Implements IBindingList.RemoveIndex
    If mSupportsBinding Then
      mBindingList.RemoveIndex([property])
    End If
  End Sub

  ''' <summary>
  ''' Removes any sort currently applied to the view.
  ''' </summary>
  Public Sub RemoveSort() Implements IBindingList.RemoveSort
    If SupportsSorting Then
      mBindingList.RemoveSort()
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
        Return mBindingList.SortDirection
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
        Return mBindingList.SortProperty
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
      If mSupportsBinding Then
        Return mBindingList.SupportsSearching
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
      If mSupportsBinding Then
        Return mBindingList.SupportsSorting
      Else
        Return False
      End If
    End Get
  End Property

  Public Sub CopyTo(ByVal array As T(), ByVal arrayIndex As Integer) Implements IList(Of T).CopyTo
    mList.CopyTo(array, arrayIndex)
  End Sub

  Private Sub ICollection_CopyTo(ByVal array As System.Array, ByVal index As Integer) Implements System.Collections.ICollection.CopyTo
    CopyTo(CType(array, T()), index)
  End Sub

  Public ReadOnly Property Count() As Integer Implements IList(Of T).Count, IBindingList.Count
    Get
      If mFiltered Then
        Return mFilterIndex.Count
      Else
        Return mList.Count
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
      Return mList
    End Get
  End Property

  Private Function IEnumerable_GetEnumerator() As IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
    Return GetEnumerator()
  End Function

  Public Sub Add(ByVal item As T) Implements IList(Of T).Add
    mList.Add(item)
  End Sub

  Private Function Add(ByVal value As Object) As Integer Implements System.Collections.IList.Add
    Add(CType(value, T))
    Dim index As Integer = FilteredIndex(mList.Count - 1)
    If index > -1 Then
      Return index
    Else
      Return 0
    End If
  End Function

  Public Sub Clear() Implements IList(Of T).Clear, IBindingList.Clear
    mList.Clear()
  End Sub

  Public Function Contains(ByVal item As T) As Boolean Implements IList(Of T).Contains
    Return mList.Contains(item)
  End Function

  Private Function Contains(ByVal value As Object) As Boolean Implements System.Collections.IList.Contains
    Return Contains(CType(value, T))
  End Function

  Public Function IndexOf(ByVal item As T) As Integer Implements IList(Of T).IndexOf
    Return mList.IndexOf(item)
  End Function

  Private Function IndexOf(ByVal value As Object) As Integer Implements System.Collections.IList.IndexOf
    Return IndexOf(CType(value, T))
  End Function

  Public Sub Insert(ByVal index As Integer, ByVal item As T) Implements IList(Of T).Insert
    mList.Insert(index, item)
  End Sub

  Private Sub Insert(ByVal index As Integer, ByVal value As Object) Implements System.Collections.IList.Insert
    Insert(index, CType(value, T))
  End Sub

  Private ReadOnly Property IsFixedSize() As Boolean Implements System.Collections.IList.IsFixedSize
    Get
      Return False
    End Get
  End Property

  Public ReadOnly Property IsReadOnly() As Boolean Implements IList(Of T).IsReadOnly, IBindingList.IsReadOnly
    Get
      Return mList.IsReadOnly
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

  Public Function Remove(ByVal item As T) As Boolean Implements IList(Of T).Remove
    Return mList.Remove(item)
  End Function

  Private Sub Remove(ByVal value As Object) Implements System.Collections.IList.Remove
    Remove(CType(value, T))
  End Sub

  Public Sub RemoveAt(ByVal index As Integer) Implements IList(Of T).RemoveAt, IBindingList.RemoveAt
    If mFiltered Then
      mList.RemoveAt(OriginalIndex(index))
    Else
      mList.RemoveAt(index)
    End If
  End Sub

  Default Public Property Item(ByVal index As Integer) As T Implements IList(Of T).Item
    Get
      If mFiltered Then
        Dim src As Integer = OriginalIndex(index)
        Return mList(src)
      Else
        Return mList(index)
      End If
    End Get
    Set(ByVal value As T)
      If mFiltered Then
        mList(OriginalIndex(index)) = value
      Else
        mList(index) = value
      End If
    End Set
  End Property

#End Region

  Private mList As IList(Of T)
  Private mSupportsBinding As Boolean
  Private mBindingList As IBindingList
  Private mFiltered As Boolean
  Private mFilterBy As PropertyDescriptor
  Private mFilter As Object
  Private mProvider As FilterProvider = Nothing
  Private mFilterIndex As List(Of ListItem) = New List(Of ListItem)()

  ''' <summary>
  ''' Creates a new view based on the provided IList object.
  ''' </summary>
  ''' <param name="list">The IList (collection) containing the data.</param>
  Public Sub New(ByVal list As IList(Of T))

    mList = list
    If TypeOf mList Is IBindingList Then
      mSupportsBinding = True
      mBindingList = CType(mList, IBindingList)
      AddHandler mBindingList.ListChanged, AddressOf SourceChanged
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
    mProvider = filterProvider

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
      Return mProvider
    End Get
    Set(ByVal value As FilterProvider)
      mProvider = value
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
      Return mFilterBy
    End Get
  End Property

  ''' <summary>
  ''' Returns True if the view is currently filtered.
  ''' </summary>
  Public ReadOnly Property IsFiltered() As Boolean
    Get
      Return mFiltered
    End Get
  End Property

  ''' <summary>
  ''' Applies a filter to the view.
  ''' </summary>
  ''' <param name="propertyName">The text name of the property on which to filter.</param>
  ''' <param name="filter">The filter criteria.</param>
  Public Sub ApplyFilter(ByVal propertyName As String, ByVal filter As Object)
    mFilterBy = Nothing
    mFilter = filter

    If (Not String.IsNullOrEmpty(propertyName)) Then
      Dim itemType As Type = GetType(T)
      For Each prop As PropertyDescriptor In TypeDescriptor.GetProperties(itemType)
        If prop.Name = propertyName Then
          mFilterBy = prop
          Exit For
        End If
      Next prop
    End If

    ApplyFilter(mFilterBy, filter)

  End Sub

  ''' <summary>
  ''' Applies a filter to the view.
  ''' </summary>
  ''' <param name="property">A PropertyDescriptor for the property on which to filter.</param>
  ''' <param name="filter">The filter criteria.</param>
  Public Sub ApplyFilter(ByVal [property] As PropertyDescriptor, ByVal filter As Object)
    mFilterBy = [property]
    DoFilter()
  End Sub

  Public Sub RemoveFilter()
    UnDoFilter()
  End Sub

  Private Sub SourceChanged(ByVal sender As Object, ByVal e As ListChangedEventArgs)
    If mFiltered Then
      Dim listIndex As Integer
      Dim filteredIndexValue As Integer = -1
      Dim newItem As T
      Dim newKey As Object
      Select Case e.ListChangedType
        Case ListChangedType.ItemAdded
          listIndex = e.NewIndex
          ' add new value to index
          newItem = mList(listIndex)
          If Not mFilterBy Is Nothing Then
            newKey = mFilterBy.GetValue(newItem)
          Else
            newKey = newItem
          End If
          mFilterIndex.Add(New ListItem(newKey, listIndex))
          filteredIndexValue = mFilterIndex.Count - 1
          ' raise event 
          OnListChanged(New ListChangedEventArgs(e.ListChangedType, filteredIndexValue))

        Case ListChangedType.ItemChanged
          listIndex = e.OldIndex
          ' update index value
          filteredIndexValue = FilteredIndex(listIndex)
          If filteredIndexValue <> -1 Then
            newItem = mList(listIndex)
            If Not mFilterBy Is Nothing Then
              newKey = mFilterBy.GetValue(newItem)
            Else
              newKey = newItem
            End If
            mFilterIndex(filteredIndexValue) = New ListItem(newKey, listIndex)
          End If
          ' raise event if appropriate
          If filteredIndexValue > -1 Then
            OnListChanged(New ListChangedEventArgs(e.ListChangedType, filteredIndexValue))
          End If

        Case ListChangedType.ItemDeleted
          listIndex = e.NewIndex
          ' delete corresponding item from index
          ' (if any)
          filteredIndexValue = FilteredIndex(listIndex)
          If filteredIndexValue <> -1 Then
            mFilterIndex.RemoveAt(filteredIndexValue)
          End If
          ' adjust index xref values
          For Each item As ListItem In mFilterIndex
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
    Return mFilterIndex(filteredIndex).BaseIndex
  End Function

  Private Function FilteredIndex(ByVal originalIndex As Integer) As Integer
    Dim result As Integer = -1
    Dim index As Integer = 0
    Do While index < mFilterIndex.Count
      If mFilterIndex(index).BaseIndex = originalIndex Then
        result = index
        Exit Do
      End If
      index += 1
    Loop
    Return result

  End Function

#Region " ICancelAddNew Members "

  Private mNewItem As T

  Private Sub CancelNew(ByVal itemIndex As Integer) Implements ICancelAddNew.CancelNew
    If Not mNewItem Is Nothing Then
      Remove(mNewItem)
    End If
  End Sub

  Private Sub EndNew(ByVal itemIndex As Integer) Implements ICancelAddNew.EndNew
    ' do nothing
  End Sub

#End Region

End Class
