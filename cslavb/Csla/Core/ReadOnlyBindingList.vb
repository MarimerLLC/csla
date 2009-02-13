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
        Throw New NotSupportedException(My.Resources.ClearInvalidException)
      End If
    End Sub

    ''' <summary>
    ''' Prevents insertion of items into the collection.
    ''' </summary>
    Protected Overrides Function AddNewCore() As Object
      If (Not IsReadOnly) Then
        Return MyBase.AddNewCore()
      Else
        Throw New NotSupportedException(My.Resources.InsertInvalidException)
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
        Throw New NotSupportedException(My.Resources.InsertInvalidException)
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
        Throw New NotSupportedException(My.Resources.RemoveInvalidException)
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
        Throw New NotSupportedException(My.Resources.ChangeInvalidException)
      End If
    End Sub

#Region " ITrackStatus "

    ''' <summary>
    ''' Gets a value indicating whether this object or its
    ''' child objects are busy.
    ''' </summary>
    Public Overrides ReadOnly Property IsBusy() As Boolean
      Get
        ' run through all the child objects
        ' and if any are dirty then then
        ' collection is dirty
        For Each child As C In Me
          Dim busy As INotifyBusy = TryCast(child, INotifyBusy)
          If busy IsNot Nothing AndAlso busy.IsBusy Then
            Return True
          End If
        Next

        Return False
      End Get
    End Property

#End Region

#Region " Indexing "

    <NonSerialized()> _
    Private _indexSet As Linq.IIndexSet(Of C)

    Private Sub DeferredLoadIndexIfNotLoaded()
      If _indexSet Is Nothing Then
        _indexSet = New Linq.IndexSet(Of C)()
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

#End Region

#Region " Where Implementation "

    ''' <summary>
    ''' Iterates through a set of items according to the expression passed to it.
    ''' </summary>
    Public Function SearchByExpression(ByVal expr As Expression(Of Func(Of C, Boolean))) As IEnumerable(Of C)
      DeferredLoadIndexIfNotLoaded()
      Dim [property] As String = _indexSet.HasIndexFor(expr)
      If [property] IsNot Nothing AndAlso IndexModeFor([property]) <> IndexModeEnum.IndexModeNever Then

        LoadIndexIfNotLoaded([property])

        Dim listOf As List(Of C) = New List(Of C)
        For Each item As C In _indexSet.Search(expr, [property])
          listOf.Add(item)
        Next

        Return CType(listOf, IEnumerable(Of C))

      Else
        Dim sourceEnum As IEnumerable(Of C) = Me.AsEnumerable()
        Dim result = sourceEnum.Where(expr.Compile())
        Dim listOf As List(Of C) = New List(Of C)
        For Each item As C In result
          listOf.Add(item)
        Next
        
      End If
    End Function

#End Region

#Region " MobileFormatter "

    ''' <summary>
    ''' Override this method to insert your field values
    ''' into the MobileFormatter serialzation stream.
    ''' </summary>
    ''' <param name="info">
    ''' Object containing the data to serialize.
    ''' </param>
    Protected Overrides Sub OnGetState(ByVal info As Serialization.Mobile.SerializationInfo)
      MyBase.OnGetState(info)
      info.AddValue("Csla.Core.ReadOnlyBindingList._isReadOnly", _isReadOnly)
    End Sub

    ''' <summary>
    ''' Override this method to retrieve your field values
    ''' from the MobileFormatter serialzation stream.
    ''' </summary>
    ''' <param name="info">
    ''' Object containing the data to serialize.
    ''' </param>
    Protected Overrides Sub OnSetState(ByVal info As Serialization.Mobile.SerializationInfo)
      MyBase.OnSetState(info)
      _isReadOnly = info.GetValue(Of Boolean)("Csla.Core.ReadOnlyBindingList._isReadOnly")
    End Sub

    ''' <summary>
    ''' Override this method to retrieve your child object
    ''' references from the MobileFormatter serialzation stream.
    ''' </summary>
    ''' <param name="info">
    ''' Object containing the data to serialize.
    ''' </param>
    ''' <param name="formatter">
    ''' Reference to MobileFormatter instance. Use this to
    ''' convert child references to/from reference id values.
    ''' </param>
    Protected Overrides Sub OnSetChildren(ByVal info As Serialization.Mobile.SerializationInfo, ByVal formatter As Serialization.Mobile.MobileFormatter)
      Dim old As Boolean = IsReadOnly
      IsReadOnly = False
      MyBase.OnSetChildren(info, formatter)
      IsReadOnly = old
    End Sub

#End Region

  End Class

  ''' <summary>
  ''' Extension method for implementation of LINQ methods on BusinessListBase
  ''' </summary>
  Public Module ReadOnlyBindingListExtension

    ''' <summary>
    ''' Custom implementation of Where for BusinessListBase - used in LINQ
    ''' </summary>
    Public Function Where(Of C As Core.IEditableBusinessObject)(ByVal source As ReadOnlyBindingList(Of C), ByVal expr As Expression(Of Func(Of C, Boolean))) As IEnumerable(Of C)

      Dim listOf As List(Of C) = New List(Of C)
      For Each item As C In source.SearchByExpression(expr)
        listOf.Add(item)
      Next

      Return CType(listOf, IEnumerable(Of C))

    End Function

  End Module

End Namespace
