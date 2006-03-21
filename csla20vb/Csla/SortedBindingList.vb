Imports System.ComponentModel
Imports System.Reflection

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

#Region " ListItem class "

  Private Class ListItem
    Implements IComparable(Of ListItem)

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

    Public Function CompareTo(ByVal other As ListItem) As Integer _
      Implements System.IComparable(Of ListItem).CompareTo

      Dim target As Object = other.Key

      If TypeOf Key Is IComparable Then
        Return DirectCast(Key, IComparable).CompareTo(target)

      Else
        If Key.Equals(target) Then
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

    Private mList As IList(Of T)
    Private mSortIndex As List(Of ListItem)
    Private mSortOrder As ListSortDirection
    Private mIndex As Integer

    Public Sub New( _
      ByVal list As IList(Of T), _
      ByVal sortIndex As List(Of ListItem), _
      ByVal direction As ListSortDirection)

      mList = list
      mSortIndex = sortIndex
      mSortOrder = direction
      Reset()
    End Sub

    Public ReadOnly Property Current() As T _
      Implements System.Collections.Generic.IEnumerator(Of T).Current
      Get
        Return mList(mSortIndex(mIndex).BaseIndex)
      End Get
    End Property

    Private ReadOnly Property CurrentItem() As Object _
      Implements System.Collections.IEnumerator.Current
      Get
        Return mList(mSortIndex(mIndex).BaseIndex)
      End Get
    End Property

    Public Function MoveNext() As Boolean _
    Implements System.Collections.IEnumerator.MoveNext

      If mSortOrder = ListSortDirection.Ascending Then
        If mIndex < mSortIndex.Count - 1 Then
          mIndex += 1
          Return True

        Else
          Return False
        End If

      Else
        If mIndex > 0 Then
          mIndex -= 1
          Return True

        Else
          Return False
        End If
      End If

    End Function

    Public Sub Reset() Implements System.Collections.IEnumerator.Reset
      If mSortOrder = ListSortDirection.Ascending Then
        mIndex = -1
      Else
        mIndex = mSortIndex.Count
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

    mSortIndex.Clear()

    If mSortBy Is Nothing Then
      For Each obj As T In mList
        mSortIndex.Add(New ListItem(obj, index))
        index += 1
      Next

    Else
      For Each obj As T In mList
        mSortIndex.Add(New ListItem(mSortBy.GetValue(obj), index))
        index += 1
      Next
    End If

    mSortIndex.Sort()
    mSorted = True

    OnListChanged(New ListChangedEventArgs(ListChangedType.Reset, 0))

  End Sub

  Private Sub UndoSort()

    mSortIndex.Clear()
    mSortBy = Nothing
    mSortOrder = ListSortDirection.Ascending
    mSorted = False

    OnListChanged(New ListChangedEventArgs(ListChangedType.Reset, 0))

  End Sub

#End Region

#Region " IEnumerable(Of T) "

  ''' <summary>
  ''' Returns an enumerator for the list, honoring
  ''' any sort that is active at the time.
  ''' </summary>
  Public Function GetEnumerator() As _
    System.Collections.Generic.IEnumerator(Of T) _
    Implements System.Collections.Generic.IEnumerable(Of T).GetEnumerator

    If mSorted Then
      Return New SortedEnumerator(mList, mSortIndex, mSortOrder)
    Else
      Return mList.GetEnumerator
    End If

  End Function

#End Region

#Region " IBindingList, IList(Of T) "

  ''' <summary>
  ''' Implemented by IList source object.
  ''' </summary>
  Public Sub AddIndex(ByVal [property] As System.ComponentModel.PropertyDescriptor) Implements System.ComponentModel.IBindingList.AddIndex
    If mSupportsBinding Then
      mBindingList.AddIndex([property])
    End If
  End Sub

  ''' <summary>
  ''' Implemented by IList source object.
  ''' </summary>
  Public Function AddNew() As Object _
    Implements System.ComponentModel.IBindingList.AddNew

    Dim result As Object

    If mSupportsBinding Then
      mInitiatedLocally = True
      result = mBindingList.AddNew
      mInitiatedLocally = False
      OnListChanged(New ListChangedEventArgs( _
        ListChangedType.ItemAdded, mBindingList.Count - 1))

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
  Public ReadOnly Property AllowNew() As Boolean Implements System.ComponentModel.IBindingList.AllowNew
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
  Public ReadOnly Property AllowRemove() As Boolean Implements System.ComponentModel.IBindingList.AllowRemove
    Get
      If mSupportsBinding Then
        Return mBindingList.AllowRemove

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

    mSortBy = Nothing

    If Len(propertyName) > 0 Then
      Dim itemType As Type = GetType(T)
      For Each prop As PropertyDescriptor In _
              TypeDescriptor.GetProperties(itemType)
        If prop.Name = propertyName Then
          mSortBy = prop
          Exit For
        End If
      Next
    End If

    ApplySort(mSortBy, direction)

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

    mSortBy = [property]
    mSortOrder = direction
    DoSort()
  End Sub

  ''' <summary>
  ''' Finds an item in the view
  ''' </summary>
  ''' <param name="propertyName">Name of the property to search</param>
  ''' <param name="key">Value to find</param>
  Public Function Find(ByVal propertyName As String, ByVal key As Object) As Integer

    Dim findProperty As PropertyDescriptor = Nothing

    If Len(propertyName) > 0 Then
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
  Public Function Find(ByVal [property] As System.ComponentModel.PropertyDescriptor, ByVal key As Object) As Integer Implements System.ComponentModel.IBindingList.Find
    If mSupportsBinding Then
      Return mBindingList.Find([property], key)
    Else
      Return -1
    End If
  End Function

  ''' <summary>
  ''' Gets a value indicating whether the view is currently sorted.
  ''' </summary>
  Public ReadOnly Property IsSorted() As Boolean Implements System.ComponentModel.IBindingList.IsSorted
    Get
      Return mSorted
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

  Protected Sub OnListChanged(ByVal e As System.ComponentModel.ListChangedEventArgs)
    RaiseEvent ListChanged(Me, e)
  End Sub

  ''' <summary>
  ''' Implemented by IList source object.
  ''' </summary>
  Public Sub RemoveIndex(ByVal [property] As System.ComponentModel.PropertyDescriptor) Implements System.ComponentModel.IBindingList.RemoveIndex
    If mSupportsBinding Then
      mBindingList.RemoveIndex([property])
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
  Public ReadOnly Property SortDirection() As _
    System.ComponentModel.ListSortDirection _
    Implements System.ComponentModel.IBindingList.SortDirection
    Get
      Return mSortOrder
    End Get
  End Property

  ''' <summary>
  ''' Returns the PropertyDescriptor of the current sort.
  ''' </summary>
  Public ReadOnly Property SortProperty() As System.ComponentModel.PropertyDescriptor Implements System.ComponentModel.IBindingList.SortProperty
    Get
      Return mSortBy
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
      If mSupportsBinding Then
        Return mBindingList.SupportsSearching

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
  Public Sub CopyTo(ByVal array() As T, ByVal arrayIndex As Integer) Implements System.Collections.Generic.ICollection(Of T).CopyTo
    mList.CopyTo(array, arrayIndex)
  End Sub

  Private Sub CopyTo(ByVal array As System.Array, ByVal index As Integer) Implements System.Collections.ICollection.CopyTo
    CopyTo(CType(array, T()), index)
  End Sub

  ''' <summary>
  ''' Implemented by IList source object.
  ''' </summary>
  Public ReadOnly Property Count() As Integer _
    Implements System.Collections.ICollection.Count, _
    System.Collections.Generic.ICollection(Of T).Count
    Get
      Return mList.Count
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

  Private Function GetItemEnumerator() As System.Collections.IEnumerator _
    Implements System.Collections.IEnumerable.GetEnumerator

    Return GetEnumerator()
  End Function

  ''' <summary>
  ''' Implemented by IList source object.
  ''' </summary>
  Public Sub Add(ByVal item As T) Implements System.Collections.Generic.ICollection(Of T).Add
    mList.Add(item)
  End Sub

  Private Function Add(ByVal value As Object) As Integer Implements System.Collections.IList.Add
    Add(CType(value, T))
    Return SortedIndex(mList.Count - 1)
  End Function

  ''' <summary>
  ''' Implemented by IList source object.
  ''' </summary>
  Public Sub Clear() Implements System.Collections.IList.Clear, System.Collections.Generic.ICollection(Of T).Clear
    mList.Clear()
  End Sub

  ''' <summary>
  ''' Implemented by IList source object.
  ''' </summary>
  Public Function Contains(ByVal item As T) As Boolean Implements System.Collections.Generic.ICollection(Of T).Contains
    Return mList.Contains(item)
  End Function

  Private Function Contains(ByVal value As Object) As Boolean Implements System.Collections.IList.Contains
    Return Contains(CType(value, T))
  End Function

  ''' <summary>
  ''' Implemented by IList source object.
  ''' </summary>
  Public Function IndexOf(ByVal item As T) As Integer Implements System.Collections.Generic.IList(Of T).IndexOf
    Return mList.IndexOf(item)
  End Function

  Private Function IndexOf(ByVal value As Object) As Integer Implements System.Collections.IList.IndexOf
    Return IndexOf(CType(value, T))
  End Function

  ''' <summary>
  ''' Implemented by IList source object.
  ''' </summary>
  Public Sub Insert(ByVal index As Integer, ByVal item As T) _
    Implements System.Collections.Generic.IList(Of T).Insert

    mList.Insert(index, item)
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
      Return mList.IsReadOnly
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
  Public Function Remove(ByVal item As T) As Boolean Implements System.Collections.Generic.ICollection(Of T).Remove
    Return mList.Remove(item)
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

    If mSorted Then
      mInitiatedLocally = True
      Dim baseIndex As Integer = OriginalIndex(index)
      ' remove the item from the source list
      mList.RemoveAt(baseIndex)
      ' delete the corresponding value in the sort index
      mSortIndex.RemoveAt(index)
      ' now fix up all index pointers in the sort index
      For Each item As ListItem In mSortIndex
        If item.BaseIndex > baseIndex Then
          item.BaseIndex -= 1
        End If
      Next
      OnListChanged( _
        New ListChangedEventArgs(ListChangedType.ItemDeleted, index))
      mInitiatedLocally = False

    Else
      mList.RemoveAt(index)
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
      If mSorted Then
        Return mList(OriginalIndex(index))
      Else
        Return mList(index)
      End If
    End Get
    Set(ByVal value As T)
      If mSorted Then
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
  Private mSorted As Boolean
  Private mInitiatedLocally As Boolean
  Private mSortBy As PropertyDescriptor
  Private mSortOrder As ListSortDirection = ListSortDirection.Ascending
  Private mSortIndex As New List(Of ListItem)

  ''' <summary>
  ''' Creates a new view based on the provided IList object.
  ''' </summary>
  ''' <param name="list">The IList (collection) containing the data.</param>
  Public Sub New(ByVal list As IList(Of T))

    mList = list

    If TypeOf mList Is IBindingList Then
      mSupportsBinding = True
      mBindingList = DirectCast(mList, IBindingList)
      AddHandler mBindingList.ListChanged, AddressOf SourceChanged
    End If

  End Sub

  Private Sub SourceChanged( _
    ByVal sender As Object, ByVal e As ListChangedEventArgs)

    If mSorted Then
      Select Case e.ListChangedType
        Case ListChangedType.ItemAdded
          Dim newItem As T = mList(e.NewIndex)
          If e.NewIndex = mList.Count - 1 Then
            Dim newKey As Object
            If mSortBy IsNot Nothing Then
              newKey = mSortBy.GetValue(newItem)
            Else
              newKey = newItem
            End If

            If mSortOrder = ListSortDirection.Ascending Then
              mSortIndex.Add(New ListItem(newKey, e.NewIndex))

            Else
              mSortIndex.Insert(0, New ListItem(newKey, e.NewIndex))
            End If
            If Not mInitiatedLocally Then
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
              ListChangedType.ItemChanged, SortedIndex(e.NewIndex)))

        Case ListChangedType.ItemDeleted
          If Not mInitiatedLocally Then
            DoSort()
          End If

        Case Else
          ' for anything other than add, delete or change
          ' just re-sort the list
          DoSort()
      End Select

    Else
      OnListChanged(e)
    End If

  End Sub

  Private Function OriginalIndex(ByVal sortedIndex As Integer) As Integer
    If mSortOrder = ListSortDirection.Ascending Then
      Return mSortIndex.Item(sortedIndex).BaseIndex
    Else
      Return mSortIndex.Item(mSortIndex.Count - 1 - sortedIndex).BaseIndex
    End If
  End Function

  Private Function SortedIndex(ByVal originalIndex As Integer) As Integer
    Dim result As Integer
    For index As Integer = 0 To mSortIndex.Count - 1
      If mSortIndex(index).BaseIndex = originalIndex Then
        result = index
        Exit For
      End If
    Next
    If mSortOrder = ListSortDirection.Descending Then
      result = mSortIndex.Count - 1 - result
    End If
    Return result
  End Function

End Class
