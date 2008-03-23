Imports System.ComponentModel
Imports System.Linq.Expressions


Namespace Core

  ''' <summary>
  ''' A readonly version of BindingList(Of T)
  ''' </summary>
  ''' <typeparam name="C">Type of item contained in the list.</typeparam>
  ''' <remarks>
  ''' This is a subclass of BindingList(Of T) that implements
  ''' a readonly list, preventing adding and removing of items
  ''' from the list. Use the IsReadOnly property
  ''' to unlock the list for loading/unloading data.
  ''' </remarks>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")> _
  <Serializable()> _
  Public MustInherit Class ReadOnlyBindingList(Of C)
    Inherits Core.ExtendedBindingList(Of C)

    Implements Core.IBusinessObject

    Private _isReadOnly As Boolean = True

    ''' <summary>
    ''' Gets or sets a value indicating whether the list is readonly.
    ''' </summary>
    ''' <remarks>
    ''' Subclasses can set this value to unlock the collection
    ''' in order to alter the collection's data.
    ''' </remarks>
    ''' <value>True indicates that the list is readonly.</value>
    Public Property IsReadOnly() As Boolean
      Get
        Return _isReadOnly
      End Get
      Protected Set(ByVal value As Boolean)
        _isReadOnly = value
      End Set
    End Property

    ''' <summary>
    ''' Creates an instance of the object.
    ''' </summary>
    Protected Sub New()
      Me.RaiseListChangedEvents = False
      AllowEdit = False
      AllowRemove = False
      AllowNew = False
      Me.RaiseListChangedEvents = True
    End Sub

    ''' <summary>
    ''' Prevents clearing the collection.
    ''' </summary>
    Protected Overrides Sub ClearItems()
      If (Not IsReadOnly) Then
        Dim oldValue As Boolean = AllowRemove
        AllowRemove = True
        MyBase.ClearItems()
        DeferredLoadIndexIfNotLoaded()
        _indexSet.ClearIndexes()
        AllowRemove = oldValue
      Else
        Throw New NotSupportedException("")
      End If
    End Sub

    ''' <summary>
    ''' Prevents insertion of items into the collection.
    ''' </summary>
    Protected Overrides Function AddNewCore() As Object
      If (Not IsReadOnly) Then
        Return MyBase.AddNewCore()
      Else
        Throw New NotSupportedException("")
      End If
    End Function

    ''' <summary>
    ''' Prevents insertion of items into the collection.
    ''' </summary>
    ''' <param name="index">Index at which to insert the item.</param>
    ''' <param name="item">Item to insert.</param>
    Protected Overrides Sub InsertItem(ByVal index As Integer, ByVal item As C)
      If (Not IsReadOnly) Then
        InsertIndexItem(item)
        MyBase.InsertItem(index, item)
      Else
        Throw New NotSupportedException("")
      End If
    End Sub

    ''' <summary>
    ''' Removes the item at the specified index if the collection is
    ''' not in readonly mode.
    ''' </summary>
    ''' <param name="index">Index of the item to remove.</param>
    Protected Overrides Sub RemoveItem(ByVal index As Integer)
      If (Not IsReadOnly) Then
        Dim oldValue As Boolean = AllowRemove
        AllowRemove = True
        RemoveIndexItem(Me(index))
        MyBase.RemoveItem(index)
        AllowRemove = oldValue
      Else
        Throw New NotSupportedException("")
      End If
    End Sub

    ''' <summary>
    ''' Replaces the item at the specified index with the 
    ''' specified item if the collection is not in
    ''' readonly mode.
    ''' </summary>
    ''' <param name="index">Index of the item to replace.</param>
    ''' <param name="item">New item for the list.</param>
    Protected Overrides Sub SetItem(ByVal index As Integer, ByVal item As C)
      If (Not IsReadOnly) Then
        RemoveIndexItem(Me(index))
        MyBase.SetItem(index, item)
        InsertIndexItem(item)
      Else
        Throw New NotSupportedException("")
      End If
    End Sub

    <NonSerialized()> _
    Private _indexSet As Linq.IIndexSet(Of C)

    Private Sub DeferredLoadIndexIfNotLoaded()
      If _indexSet Is Nothing Then
        _indexSet = New Csla.Linq.IndexSet(Of C)()
      End If
    End Sub

    ''' <summary>
    ''' Allows users of CSLA to override the indexing behavior of BLB
    ''' </summary>
    <EditorBrowsable(EditorBrowsableState.Advanced)> _
    Public Property IndexingProvider() As Type
      Get
        DeferredLoadIndexIfNotLoaded()
        Return _indexSet.GetType()
      End Get
      Set(ByVal value As Type)
        If value.IsClass AndAlso (Not value.IsAbstract) AndAlso value.IsAssignableFrom(GetType(Linq.IIndexSet(Of C))) Then
          _indexSet = TryCast(Activator.CreateInstance(value), Linq.IIndexSet(Of C))
          ReIndexAll()
        End If
      End Set
    End Property


    Private Function IndexModeFor(ByVal [property] As String) As IndexModeEnum
      DeferredLoadIndexIfNotLoaded()
      If _indexSet.HasIndexFor([property]) Then
        Return _indexSet([property]).IndexMode
      Else
        Return IndexModeEnum.IndexModeNever
      End If
    End Function

    Private Function IndexLoadedFor(ByVal [property] As String) As Boolean
      DeferredLoadIndexIfNotLoaded()
      If _indexSet.HasIndexFor([property]) Then
        Return _indexSet([property]).Loaded
      Else
        Return False
      End If
    End Function
    Private Sub LoadIndexIfNotLoaded(ByVal [property] As String)
      If IndexModeFor([property]) <> IndexModeEnum.IndexModeNever Then
        If (Not IndexLoadedFor([property])) Then
          _indexSet.LoadIndex([property])
          ReIndex([property])
        End If
      End If
    End Sub

    Private Sub InsertIndexItem(ByVal item As C)
      DeferredLoadIndexIfNotLoaded()
      _indexSet.InsertItem(item)
    End Sub

    Private Sub InsertIndexItem(ByVal item As C, ByVal [property] As String)
      DeferredLoadIndexIfNotLoaded()
      _indexSet.InsertItem(item, [property])
    End Sub

    Private Sub RemoveIndexItem(ByVal item As C)
      DeferredLoadIndexIfNotLoaded()
      _indexSet.RemoveItem(item)
    End Sub

    Private Sub RemoveIndexItem(ByVal item As C, ByVal [property] As String)
      DeferredLoadIndexIfNotLoaded()
      _indexSet.RemoveItem(item, [property])
    End Sub

    Private Sub ReIndexItem(ByVal item As C, ByVal [property] As String)
      DeferredLoadIndexIfNotLoaded()
      _indexSet.ReIndexItem(item, [property])
    End Sub

    Private Sub ReIndexItem(ByVal item As C)
      DeferredLoadIndexIfNotLoaded()
      _indexSet.ReIndexItem(item)
    End Sub

    Private Sub ReIndexAll()
      DeferredLoadIndexIfNotLoaded()
      _indexSet.ClearIndexes()
      For Each item As C In Me
        InsertIndexItem(item)
      Next item
    End Sub

    Private Sub ReIndex(ByVal [property] As String)
      DeferredLoadIndexIfNotLoaded()
      _indexSet.ClearIndex([property])
      For Each item As C In Me
        InsertIndexItem(item, [property])
      Next item
      _indexSet([property]).LoadComplete()
    End Sub

    Public Function SearchByExpression(ByVal expr As Expression(Of Func(Of C, Boolean))) As IEnumerable(Of C)
      DeferredLoadIndexIfNotLoaded()
      Dim [property] As String = _indexSet.HasIndexFor(expr)
      If [property] IsNot Nothing AndAlso IndexModeFor([property]) <> IndexModeEnum.IndexModeNever Then
        LoadIndexIfNotLoaded([property])
        Return _indexSet.Search(expr, [property])
      Else
        Return Me.AsEnumerable().Where(expr.Compile())
      End If
    End Function

  End Class

  ''' <summary>
  ''' Extension method for implementation of LINQ methods on BusinessListBase
  ''' </summary>
  Public Module ReadOnlyBindingListExtension

    ''' <summary>
    ''' Custom implementation of Where for BusinessListBase - used in LINQ
    ''' </summary>
    <System.Runtime.CompilerServices.Extension()> _
    Public Function Where(Of C As Core.IEditableBusinessObject)(ByVal source As ReadOnlyBindingList(Of C), ByVal expr As Expression(Of Func(Of C, Boolean))) As IEnumerable(Of C)
      Return source.SearchByExpression(expr)
    End Function

  End Module

End Namespace
