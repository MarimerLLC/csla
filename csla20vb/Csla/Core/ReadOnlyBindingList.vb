Namespace Core

  ''' <summary>
  ''' A readonly version of BindingList(Of T)
  ''' </summary>
  ''' <typeparam name="T">Type of item contained in the list.</typeparam>
  ''' <remarks>
  ''' This is a subclass of BindingList(Of T) that implements
  ''' a readonly list, preventing adding and removing of items
  ''' from the list. Use the Protected IsReadOnly property
  ''' to unlock the list for loading/unloading data.
  ''' </remarks>
  <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")> _
  <Serializable()> _
  Public MustInherit Class ReadOnlyBindingList(Of T)
    Inherits System.ComponentModel.BindingList(Of T)

    Private mIsReadOnly As Boolean = True

    ''' <summary>
    ''' Gets or sets whether the list is readonly.
    ''' </summary>
    ''' <value>True indicates that the list is readonly.</value>
    Protected Property IsReadOnly() As Boolean
      Get
        Return mIsReadOnly
      End Get
      Set(ByVal value As Boolean)
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
        AllowRemove = True
        MyBase.ClearItems()
        AllowRemove = False
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
    Protected Overrides Sub InsertItem(ByVal index As Integer, ByVal item As T)
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
        AllowRemove = True
        MyBase.RemoveItem(index)
        AllowRemove = False

      Else
        Throw New NotSupportedException(My.Resources.RemoveInvalidException)
      End If
    End Sub

    ''' <summary>
    ''' Replaces the item at the specified index with the 
    ''' specified item if the collection is not in
    ''' readonly mode.
    ''' </summary>
    Protected Overrides Sub SetItem(ByVal index As Integer, ByVal item As T)
      If Not IsReadOnly Then
        MyBase.SetItem(index, item)
      Else
        Throw New NotSupportedException(My.Resources.ChangeInvalidException)
      End If
    End Sub

  End Class

End Namespace
