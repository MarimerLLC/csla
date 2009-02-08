Imports System
Imports System.ComponentModel
Imports System.Collections.Generic
Imports System.Reflection
Imports System.Collections
Imports System.Linq
Imports System.Linq.Expressions
Imports Csla.Core

''' <summary>
''' Provides a filtered view into an existing IList(Of T).
''' </summary>
''' <typeparam name="T">The type of the objects contained
''' in the original list.</typeparam>
Public Class LinqBindingList(Of T)
  Implements IList(Of T)
  Implements IBindingList
  Implements IEnumerable(Of T)
  Implements ICancelAddNew
  Implements IQueryable(Of T)
  Implements IOrderedQueryable(Of T)

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
      Return Key.ToString()
    End Function

  End Class

#End Region

#Region "Filter/Unfilter"
  ''' <summary>
  ''' Applies a filter from the original Linq query to the view 
  ''' </summary>
  Public Sub ApplyFilter()
    BuildFilterIndex()
    OnListChanged(New ListChangedEventArgs(ListChangedType.Reset, 0))
  End Sub

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

    '_newItem = (T)result;
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
      Throw New NotSupportedException(My.Resources.SortingNotSupported)
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
    RaiseEvent ListChanged(Me, e)
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
      Throw New NotSupportedException(My.Resources.SortingNotSupported)
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
  Public Sub CopyTo(ByVal array() As T, ByVal arrayIndex As Integer) Implements System.Collections.Generic.ICollection(Of T).CopyTo
    Dim pos As Integer = arrayIndex
    For Each listItem As ListItem In _filterIndex
      array(pos) = _list(listItem.BaseIndex)
      pos += 1
    Next listItem
  End Sub

  Private Sub ICollection_CopyTo(ByVal array As System.Array, ByVal index As Integer) Implements System.Collections.ICollection.CopyTo
    Dim tmp() As T = New T(array.Length - 1) {}
    CopyTo(tmp, index)
    array.Copy(tmp, 0, array, index, array.Length)
  End Sub

  ''' <summary>
  ''' Gets the number of items in the list.
  ''' </summary>
  Public ReadOnly Property Count() As Integer Implements System.Collections.Generic.ICollection(Of T).Count, System.Collections.ICollection.Count
    Get
      Return _filterIndex.Count
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
    Return  DirectCast(Me,IEnumerable(Of T)).GetEnumerator()
  End Function

  ''' <summary>
  ''' Adds an item to the list.
  ''' </summary>
  ''' <param name="item">Item to be added.</param>
  Public Sub Add(ByVal item As T) Implements System.Collections.Generic.ICollection(Of T).Add
    _list.Add(item)
  End Sub

  Private Function IList_Add(ByVal value As Object) As Integer Implements System.Collections.IList.Add
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
  Public Sub Clear() Implements System.Collections.Generic.ICollection(Of T).Clear, System.Collections.IList.Clear
    _list.Clear()
  End Sub

  ''' <summary>
  ''' Determines whether the specified
  ''' item is contained in the list.
  ''' </summary>
  ''' <param name="item">Item to find.</param>
  ''' <returns><see langword="true"/> if the item is
  ''' contained in the list.</returns>
  Public Function Contains(ByVal item As T) As Boolean Implements System.Collections.Generic.ICollection(Of T).Contains
    Return _list.Contains(item)
  End Function

  Private Function IList_Contains(ByVal value As Object) As Boolean Implements System.Collections.IList.Contains
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

  Private Function IList_IndexOf(ByVal value As Object) As Integer Implements System.Collections.IList.IndexOf
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

  Private Sub IList_Insert(ByVal index As Integer, ByVal value As Object) Implements System.Collections.IList.Insert
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
  Public ReadOnly Property IsReadOnly() As Boolean Implements System.Collections.Generic.ICollection(Of T).IsReadOnly, System.Collections.IList.IsReadOnly
    Get
      Return _list.IsReadOnly
    End Get
  End Property

  Public Property IList_Item(ByVal index As Integer) As Object Implements System.Collections.IList.Item
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
  Public Function Remove(ByVal item As T) As Boolean Implements System.Collections.Generic.ICollection(Of T).Remove
    Return _list.Remove(item)
  End Function

  Private Sub IList_Remove(ByVal value As Object) Implements System.Collections.IList.Remove
    Remove(CType(value, T))
  End Sub

  ''' <summary>
  ''' Removes an item from the list.
  ''' </summary>
  ''' <param name="index">Index of item
  ''' to be removed.</param>
  Public Sub RemoveAt(ByVal index As Integer) Implements IList(Of T).RemoveAt, System.Collections.IList.RemoveAt
    _list.RemoveAt(OriginalIndex(index))
  End Sub

  ''' <summary>
  ''' Gets or sets the item at 
  ''' the specified index.
  ''' </summary>
  ''' <param name="index">Index of the item.</param>
  ''' <returns>Item at the specified index.</returns>
  Default Public Property Item(ByVal index As Integer) As T Implements IList(Of T).Item
    Get
      Dim src As Integer = OriginalIndex(index)
      Return _list(src)
    End Get
    Set(ByVal value As T)
      _list(OriginalIndex(index)) = value
    End Set
  End Property

#End Region

#Region "SourceList"

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
  Private _filterBy As PropertyDescriptor
  Private _sortExpression As Expression
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

  Private _expression As Expression
  Private _queryProvider As IQueryProvider

  Friend Sub New(ByVal list As IList(Of T), ByVal queryProvider As IQueryProvider, ByVal expression As Expression)
    Me.New(list)
    SetFilterByExpression(expression)
    _queryProvider = queryProvider
    If expression Is Nothing Then
      _expression = expression.Constant(Me)
    Else
      _expression = expression
    End If
  End Sub

  Private Shared Function SelectAll() As Expression(Of Func(Of T, Boolean))
    Return Function(T) True 'TODO: Is this equivilent to c# return (T) => true;?
  End Function

  Private Sub SetFilterByExpression(ByVal expression As Expression)
    Dim whereFinder As InnermostWhereFinder = New InnermostWhereFinder()
    Dim whereExpression As MethodCallExpression = whereFinder.GetInnermostWhere(expression)
    If whereExpression Is Nothing Then
      Return
    End If
    If whereExpression.Arguments.Count < 2 Then
      Return
    End If
    If whereExpression.Arguments(1) Is Nothing Then
      Return
    End If

    Dim whereBody As Expression(Of Func(Of T, Boolean)) = _
      CType((CType(whereExpression.Arguments(1), UnaryExpression)).Operand, Expression(Of Func(Of T, Boolean)))
    If TypeOf whereBody.Body Is BinaryExpression Then
      Dim binExp As BinaryExpression = CType(whereBody.Body, BinaryExpression)
      If binExp.Left.NodeType = ExpressionType.MemberAccess AndAlso (TryCast(binExp.Left, MemberExpression)).Member.MemberType = MemberTypes.Property Then
        _filterBy = TypeDescriptor.GetProperties(GetType(T))((TryCast(binExp.Left, MemberExpression)).Member.Name)
      End If
    End If

  End Sub

  Private Function ItemShouldBeInList(ByVal item As T) As Boolean
    Dim whereFinder As InnermostWhereFinder = New InnermostWhereFinder()
    Dim whereExpression As MethodCallExpression = whereFinder.GetInnermostWhere(_expression)
    If whereExpression Is Nothing Then
      Return False
    End If
    If whereExpression.Arguments.Count < 2 Then
      Return False
    End If
    If whereExpression.Arguments(1) Is Nothing Then
      Return False
    End If
    Dim whereBody As Expression(Of Func(Of T, Boolean)) = _
      CType((CType(whereExpression.Arguments(1), UnaryExpression)).Operand, Expression(Of Func(Of T, Boolean)))

    Dim searchable = CType(_list, Linq.IIndexSearchable(Of T))

    If searchable Is Nothing Then
      Return False
    Else
      Return searchable.SearchByExpression(whereBody).Contains(item)
    End If

  End Function

  Private Sub SourceChanged(ByVal sender As Object, ByVal e As ListChangedEventArgs)
    Dim listIndex As Integer
    Dim filteredIndex As Integer = -1
    Dim newItem As T
    Dim newKey As Object
    Select Case e.ListChangedType
      Case ListChangedType.ItemAdded
        listIndex = e.NewIndex
        ' add new value to index
        newItem = _list(listIndex)
        If _filterBy IsNot Nothing Then
          newKey = _filterBy.GetValue(newItem)
        Else
          newKey = newItem
        End If

        'check to see if it is in the filter
        If ItemShouldBeInList(newItem) Then
          _filterIndex.Add(New ListItem(newKey, listIndex))
        End If

        filteredIndex = _filterIndex.Count - 1
        ' raise event 
        OnListChanged(New ListChangedEventArgs(e.ListChangedType, filteredIndex))

      Case ListChangedType.ItemChanged
        listIndex = e.NewIndex
        ' update index value
        filteredIndex = Me.FilteredIndex(listIndex)
        If filteredIndex <> -1 Then
          newItem = _list(listIndex)
          If _filterBy IsNot Nothing Then
            newKey = _filterBy.GetValue(newItem)
          Else
            newKey = newItem
          End If
          If ItemShouldBeInList(newItem) Then
            _filterIndex(filteredIndex) = New ListItem(newKey, listIndex)
          End If

        End If
        ' raise event if appropriate
        If filteredIndex > -1 Then
          OnListChanged(New ListChangedEventArgs(e.ListChangedType, filteredIndex, e.PropertyDescriptor))
        End If

      Case ListChangedType.ItemDeleted
        listIndex = e.NewIndex
        ' delete corresponding item from index
        ' (if any)
        filteredIndex = Me.FilteredIndex(listIndex)
        If filteredIndex <> -1 Then
          _filterIndex.RemoveAt(filteredIndex)
        End If
        ' adjust index xref values
        For Each item As ListItem In _filterIndex
          If item.BaseIndex > e.NewIndex Then
            item.BaseIndex -= 1
          End If
        Next item
        ' raise event if appropriate
        If filteredIndex > -1 Then
          OnListChanged(New ListChangedEventArgs(e.ListChangedType, filteredIndex))
        End If

      Case ListChangedType.PropertyDescriptorAdded, ListChangedType.PropertyDescriptorChanged, ListChangedType.PropertyDescriptorDeleted
        OnListChanged(e)

      Case Else
        BuildFilterIndex()
        OnListChanged(New ListChangedEventArgs(ListChangedType.Reset, 0))
    End Select
  End Sub

  Private Function OriginalIndex(ByVal filteredIndex As Integer) As Integer
    Return _filterIndex(filteredIndex).BaseIndex
  End Function

  Private Function FilteredIndex(ByVal originalIndex As Integer) As Integer
    Dim result As Integer = -1
    For index As Integer = 0 To _filterIndex.Count - 1
      If _filterIndex(index).BaseIndex = originalIndex Then
        result = index
        Exit For
      End If
    Next index
    Return result
  End Function

#Region "ICancelAddNew Members"

  Private Sub CancelNew(ByVal itemIndex As Integer) Implements ICancelAddNew.CancelNew
    Dim can As ICancelAddNew = TryCast(_list, ICancelAddNew)
    If can IsNot Nothing Then
      can.CancelNew(itemIndex)
    Else
      _list.RemoveAt(itemIndex)
    End If
  End Sub

  Private Sub EndNew(ByVal itemIndex As Integer) Implements ICancelAddNew.EndNew
    Dim can As ICancelAddNew = TryCast(_list, ICancelAddNew)
    If can IsNot Nothing Then
      can.EndNew(itemIndex)
    End If
  End Sub

#End Region

#Region "ToArray"

  ''' <summary>
  ''' Get an array containing all items in the list.
  ''' </summary>
  Public Function ToArray() As T()
    Dim result As List(Of T) = New List(Of T)()
    For Each item As T In Me
      result.Add(item)
    Next item
    Return result.ToArray()
  End Function

#End Region

  Friend Sub SortByExpression(ByVal expression As Expression)
    _sortExpression = expression
    BuildFilterIndex()
  End Sub

  Friend Sub BuildFilterIndex()
    ' Find the call to Where() and get the lambda expression predicate.
    Dim whereFinder As InnermostWhereFinder = New InnermostWhereFinder()
    Dim whereExpression As MethodCallExpression = whereFinder.GetInnermostWhere(_expression)
    Dim whereBody As Expression(Of Func(Of T, Boolean)) = CType((CType(whereExpression.Arguments(1), UnaryExpression)).Operand, Expression(Of Func(Of T, Boolean)))

    Dim subset = DirectCast(_list, Linq.IIndexSearchable(Of T)).SearchByExpression(whereBody)

    If _sortExpression IsNot Nothing AndAlso TypeOf (_sortExpression) Is MethodCallExpression Then
      subset = BuildSortedSubset(subset, DirectCast(_sortExpression, MethodCallExpression))
    End If

    _filterIndex.Clear()
    Dim searchable = TryCast(_list, Linq.IIndexSearchable(Of T))
    If searchable IsNot Nothing Then
      'before we can start, we do have to go through the whole thing once to make our filterindex.  
      For Each item As T In subset
        If Not _filterBy Is Nothing Then
          Dim tmp As Object = _filterBy.GetValue(item)
          _filterIndex.Add(New ListItem(tmp, (CType(_list, IPositionMappable(Of T))).PositionOf(item)))
        Else
          _filterIndex.Add(New ListItem(item.GetHashCode(), (CType(_list, IPositionMappable(Of T))).PositionOf(item)))
        End If
      Next item
    End If

  End Sub

  Private Shared Function GetWhereBodyFromExpression(ByVal whereExpression As MethodCallExpression) As Expression(Of Func(Of T, Boolean))
    Dim whereBody As Expression(Of Func(Of T, Boolean))
    If whereExpression Is Nothing Then
      whereBody = SelectAll()
    ElseIf whereExpression.Arguments.Count < 2 Then
      whereBody = SelectAll()
    ElseIf whereExpression.Arguments(1) Is Nothing Then
      whereBody = SelectAll()
    Else
      'TODO: Pain cast - whereBody = (Expression(Of Func(Of T, Boolean)(CType(whereExpression.Arguments(1), UnaryExpression).Operand))
    End If

    Return whereBody

  End Function

#Region "IEnumerable<T> Members"

  'TODO: getting dip signature and it's late.
  '''' <summary>
  '''' Gets an enumerator object.
  '''' </summary>
  '''' <returns></returns>
  'Public Function GetEnumerator() As IEnumerator(Of T) Implements System.Collections.Generic.IEnumerable(Of T).GetEnumerator
  '  Dim returnEnumerable As List(Of T) = New List(Of T)()
  '  If TypeOf _list Is Linq.IIndexSearchable(Of T) AndAlso TypeOf _list Is Core.IPositionMappable(Of T) Then
  '    ' Find the call to Where() and get the lambda expression predicate.
  '    Dim whereFinder As InnermostWhereFinder = New InnermostWhereFinder()
  '    Dim whereExpression As MethodCallExpression = whereFinder.GetInnermostWhere(_expression)
  '    Dim whereBody As Expression(Of Func(Of T, Boolean)) = CType((CType(whereExpression.Arguments(1), UnaryExpression)).Operand, Expression(Of Func(Of T, Boolean)))

  '    For Each item As T In (TryCast(_list, Linq.IIndexSearchable(Of T))).SearchByExpression(whereBody)
  '      returnEnumerable.Add(item)
  '    Next item
  '  Else
  '    For Each item As T In _list

  '      returnEnumerable.Add(item)
  '    Next item
  '  End If
  '  Return CType(returnEnumerable.GetEnumerator, Global.System.Collections.Generic.IEnumerator(Of T))
  'End Function

  Friend Function ToList(Of TResult)() As List(Of T)
    Dim newList = New List(Of T)(Me.Count)
    For Each item As T In Me
      newList.Add(item)
    Next
    Return newList
  End Function

#End Region

#Region " IEnumerable<T> Members "

  ''' <summary>
  ''' Gets an enumerator object.
  ''' </summary>
  ''' <returns></returns>
  Public Function GetEnumerator() As IEnumerator(Of T) Implements IEnumerable(Of T).GetEnumerator
    Dim isList = DirectCast(_list, Linq.IIndexSearchable(Of T))
    Dim isMappable = DirectCast(_list, Core.IPositionMappable(Of T))

    If isList IsNot Nothing AndAlso isMappable IsNot Nothing Then
      'Find the call to Where() and get the lambda expression predicate.
      Dim whereFinder As InnermostWhereFinder = New InnermostWhereFinder
      Dim whereExpression As MethodCallExpression = whereFinder.GetInnermostWhere(_expression)
      Dim whereBody As Expression(Of Func(Of T, Boolean)) = GetWhereBodyFromExpression(whereExpression)

      Dim subset = DirectCast(_list, Linq.IIndexSearchable(Of T)).SearchByExpression(whereBody)
      If _sortExpression IsNot Nothing AndAlso TypeOf (_sortExpression) Is MethodCallExpression Then
        subset = BuildSortedSubset(subset, DirectCast(_sortExpression, MethodCallExpression))

        'TODO: Need yield equivilent here
      End If
    Else
      'TODO: Need yield equivilent here
    End If

  End Function

  Private Shared Function BuildSortedSubset(ByVal subset As IEnumerable(Of T), ByVal sortExpression As MethodCallExpression) As IEnumerable(Of T)
    'get the lambda buried in the second argument
    Dim lambda As LambdaExpression = CType(CType(sortExpression.Arguments(1), UnaryExpression).Operand, LambdaExpression)

    'get the generic arguments of the lambda
    Dim genArgs = lambda.Type.GetGenericArguments()

    'get the sort method out via reflection
    'TODO: ugly lambda needed here

    'make a generic method using the generic arguments
    'Dim genericSortMethod = sortMethod.MakeGenericMethod(genArgs)  'TODO Uncomment when above todo is fixed

    'replace the subset with the sorted subset

    'TODO subset = (IEnumerable<T>)genericSortMethod.Invoke(null, new object[] { subset.AsQueryable<T>(), lambda });

    Return subset

  End Function

#End Region

#Region "IQueryable Members"

  Private ReadOnly Property ElementType() As Type Implements IQueryable.ElementType
    Get
      Return GetType(T)
    End Get
  End Property

  Private ReadOnly Property IQueryable_Expression() As Expression Implements IQueryable.Expression
    Get
      Return _expression
    End Get
  End Property

  Private ReadOnly Property IQueryable_Provider() As IQueryProvider Implements IQueryable.Provider
    Get
      Return _queryProvider
    End Get
  End Property

#End Region
End Class
