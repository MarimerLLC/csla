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
    Inherits System.ComponentModel.BindingList(Of C)

    Implements Core.IBusinessObject

    Private mIsReadOnly As Boolean = True

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
        Return mIsReadOnly
      End Get
      Protected Set(ByVal value As Boolean)
        mIsReadOnly = value
      End Set
    End Property

    Protected Sub New()
      AllowEdit = False
      AllowRemove = False
      AllowNew = False
    End Sub

    ''' <summary>
    ''' Prevents clearing the collection.
    ''' </summary>
    Protected Overrides Sub ClearItems()
      If Not IsReadOnly Then
        Dim oldValue As Boolean = AllowRemove
        AllowRemove = True
        MyBase.ClearItems()
        AllowRemove = oldValue
      Else
        Throw New NotSupportedException(My.Resources.ClearInvalidException)
      End If
    End Sub

    ''' <summary>
    ''' Prevents insertion of items into the collection.
    ''' </summary>
    Protected Overrides Function AddNewCore() As Object
      If Not IsReadOnly Then
        Return MyBase.AddNewCore()
      Else
        Throw New NotSupportedException(My.Resources.InsertInvalidException)
      End If
    End Function

    ''' <summary>
    ''' Prevents insertion of items into the collection.
    ''' </summary>
    Protected Overrides Sub InsertItem(ByVal index As Integer, ByVal item As C)
      If Not IsReadOnly Then
        MyBase.InsertItem(index, item)
      Else
        Throw New NotSupportedException(My.Resources.InsertInvalidException)
      End If
    End Sub

    ''' <summary>
    ''' Removes the item at the specified index if the collection is
    ''' not in readonly mode.
    ''' </summary>
    Protected Overrides Sub RemoveItem(ByVal index As Integer)
      If Not IsReadOnly Then
        Dim oldValue As Boolean = AllowRemove
        AllowRemove = True
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
    Protected Overrides Sub SetItem(ByVal index As Integer, ByVal item As C)
      If Not IsReadOnly Then
        MyBase.SetItem(index, item)
      Else
        Throw New NotSupportedException(My.Resources.ChangeInvalidException)
      End If
    End Sub

  End Class

End Namespace
