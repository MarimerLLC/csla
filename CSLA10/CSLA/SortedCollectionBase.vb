Imports System.ComponentModel

''' <summary>
'''
''' </summary>
Namespace Core

  ''' <summary>
  ''' This class implements sorting functionality for collections.
  ''' </summary>
  ''' <remarks>
  ''' <para>
  ''' This class inhirits from Core.BindableCollectionBase. If you want 
  ''' to create a sorted collection that is safe for data binding and
  ''' also supports sorting, create a subclass of this class. To create
  ''' a fully functional business collection, create a subclass of
  ''' BusinessCollectionBase.
  ''' </para><para>
  ''' The Core.BindableCollectionBase class implements the IBindableList
  ''' interface. However, it doesn't actually implement sorting. Instead
  ''' it delegates the sorting functionality to a set of protected virtual 
  ''' methods. This class provides the actual sorting implementation
  ''' by overriding those methods.
  ''' </para>
  ''' </remarks>
  <Serializable()> _
  Public Class SortableCollectionBase
    Inherits BindableCollectionBase

#Region " Properties "

    <NotUndoable()> _
    Private mIsSorted As Boolean = False
    <NonSerialized(), NotUndoable()> _
    Private mSortProperty As PropertyDescriptor
    <NotUndoable()> _
    Private mSortPropertyName As String
    <NotUndoable()> _
    Private mListSortDirection As ListSortDirection = ListSortDirection.Ascending
    <NotUndoable()> _
    Private mUnsortedList As ArrayList
    <NotUndoable()> _
    Private mActivelySorting As Boolean = False

    ''' <summary>
    ''' Indicates whether the collection is in the process of
    ''' being sorted at this time.
    ''' </summary>
    Protected ReadOnly Property ActivelySorting() As Boolean
      Get
        Return mActivelySorting
      End Get
    End Property

    ''' <summary>
    ''' Only creates the unsorted list on demand.
    ''' </summary>
    Private ReadOnly Property UnsortedList() As ArrayList
      Get
        If mUnsortedList Is Nothing Then
          mUnsortedList = New ArrayList()
        End If
        Return mUnsortedList
      End Get
    End Property

    ''' <summary>
    ''' Returns a value indicating whether the collection is currently
    ''' sorted.
    ''' </summary>
    Protected Overrides ReadOnly Property IBindingList_IsSorted() As Boolean
      Get
        Return mIsSorted
      End Get
    End Property

    ''' <summary>
    ''' Returns the property by which the collection is currently sorted.
    ''' </summary>
    ''' <remarks>
    ''' This method is invoked via the IBindingList interface and is not
    ''' intended for use by code in your business class.
    ''' </remarks>
    Protected Overrides ReadOnly Property IBindingList_SortProperty() As PropertyDescriptor
      Get
        If mSortProperty Is Nothing AndAlso Len(mSortPropertyName) > 0 Then
          Try
            ' we need to recreate the sortproperty value
            Dim childType As Type
            If list.Count > 0 Then
              ' get child type from the first element in the collection
              childType = list.Item(0).GetType

            Else
              ' get child type from Item property
              Try
                childType = Me.GetType.GetProperty("Item", New Type() {GetType(Integer)}).PropertyType

              Catch
                childType = GetType(Object)
              End Try
            End If

            ' now get the property descriptor from the type
            mSortProperty = _
              TypeDescriptor.GetProperties(childType).Item(mSortPropertyName)

          Catch
            ' we failed to recreate it - return nothing
            mSortProperty = Nothing
          End Try
        End If
        Return mSortProperty
      End Get
    End Property

    ''' <summary>
    ''' Returns the current sort direction.
    ''' </summary>
    ''' <remarks>
    ''' This method is invoked via the IBindingList interface and is not
    ''' intended for use by code in your business class.
    ''' </remarks>
    Protected Overrides ReadOnly Property IBindingList_SortDirection() As ListSortDirection
      Get
        Return mListSortDirection
      End Get
    End Property

#End Region

#Region " ApplySort "

    ''' <summary>
    ''' Applies a sort to the collection.
    ''' </summary>
    ''' <remarks>
    ''' This method is invoked via the IBindingList interface and is not
    ''' intended for use by code in your business class.
    ''' </remarks>
    Protected Overrides Sub IBindingList_ApplySort(ByVal [property] As System.ComponentModel.PropertyDescriptor, ByVal direction As System.ComponentModel.ListSortDirection)
      Dim holdList As ArrayList
      Dim count As Integer
      Dim total As Integer
      Dim temp() As Object
      Dim tempObject As Object

      If Not AllowSort Then
        Throw New NotSupportedException("Sorting is not supported by this collection.")
      End If

      mSortProperty = [property]
      mSortPropertyName = mSortProperty.Name
      mListSortDirection = direction

      If list.Count > 1 Then
        Try
          mActivelySorting = True
          holdList = New System.Collections.ArrayList()
          For Each tempObject In list
            holdList.Add(tempObject)
          Next

          ReDim temp(list.Count - 1)
          For count = 0 To list.Count - 1
            temp(count) = CallByName(list.Item(count), mSortPropertyName, CallType.Get)
          Next

          Array.Sort(temp)

          'Ok, rearrang the list.  Do not use clear.  Does not like that in sort for
          'some reason in the sort.  This works fine
          total = list.Count - 1
          For count = total To 0 Step -1
            tempObject = list.Item(count)
            list.Remove(tempObject)
          Next

          If direction = ListSortDirection.Ascending Then
            For count = 0 To UBound(temp)
              For Each tempObject In holdList

                If CallByName(tempObject, mSortPropertyName, CallType.Get).Equals(temp(count)) Then 'Hey, we have a winner, sort this one
                  holdList.Remove(tempObject)
                  Exit For
                End If
              Next
              list.Add(tempObject)
            Next

          Else ' sort decending
            For count = UBound(temp) To 0 Step -1
              For Each tempObject In holdList

                If CallByName(tempObject, mSortPropertyName, CallType.Get).Equals(temp(count)) Then 'Hey, we have a winner, sort this one
                  holdList.Remove(tempObject)
                  Exit For
                End If
              Next
              list.Add(tempObject)
            Next
          End If

          mIsSorted = True

        Catch
          IBindingList_RemoveSort()

        Finally
          mActivelySorting = False
        End Try

      ElseIf list.Count = 1 Then
        mIsSorted = True
      End If

    End Sub

#End Region

#Region " RemoveSort "

    ''' <summary>
    ''' Removes any sort from the collection.
    ''' </summary>
    ''' <remarks>
    ''' This method is invoked via the IBindingList interface and is not
    ''' intended for use by code in your business class.
    ''' </remarks>
    Protected Overrides Sub IBindingList_RemoveSort()
      Dim count As Integer
      Dim total As Integer
      Dim listItem As Object

      If Not AllowSort Then
        Throw New NotSupportedException("Sorting is not supported by this collection.")
      End If

      'Return the list to its unsorted state
      'Ok, rearrange the list.  Do not use clear.  Does not like that in sort for
      'some reason in the sort.  This works fine
      mActivelySorting = True
      total = list.Count - 1
      For count = total To 0 Step -1
        listItem = list.Item(count)
        list.Remove(listItem)
      Next

      For Each listItem In UnsortedList
        list.Add(listItem)
      Next

      mUnsortedList = Nothing

      mIsSorted = False
      mSortProperty = Nothing
      mSortPropertyName = ""
      mListSortDirection = ListSortDirection.Ascending
      mActivelySorting = False

    End Sub

#End Region

#Region " Collection events "

    ''' <summary>
    ''' Ensures that any sort is maintained as a new item is inserted.
    ''' </summary>
    Protected Overrides Sub OnInsertComplete(ByVal index As Integer, ByVal value As Object)
      If AllowSort AndAlso Not ActivelySorting Then
        UnsortedList.Add(value)
      End If
      MyBase.OnInsertComplete(index, value)
    End Sub

    ''' <summary>
    ''' Ensures that any sort is maintained as the list is cleared.
    ''' </summary>
    Protected Overrides Sub OnClearComplete()
      If AllowSort AndAlso Not ActivelySorting Then
        UnsortedList.Clear()
      End If
      MyBase.OnClearComplete()
    End Sub

    ''' <summary>
    ''' Ensures that any sort is maintained as an item is removed.
    ''' </summary>
    Protected Overrides Sub OnRemoveComplete(ByVal index As Integer, ByVal value As Object)
      If AllowSort AndAlso Not ActivelySorting Then
        UnsortedList.Remove(value)
      End If
      MyBase.OnRemoveComplete(index, value)
    End Sub

#End Region

#Region " Search/Find "

    ''' <summary>
    ''' Implements search/find functionality for the collection.
    ''' </summary>
    Protected Overrides Function IBindingList_Find(ByVal [property] As PropertyDescriptor, ByVal key As Object) As Integer

      If Not AllowFind Then
        Throw New NotSupportedException("Searching is not supported by this collection.")
      End If

      Dim index As Integer
      Dim tmp As Object
      Dim prop As String = [property].Name

      For index = 0 To list.Count - 1
        tmp = CallByName(list.Item(index), prop, CallType.Get)
        If tmp.Equals(key) Then
          ' we found a match
          Return index
        End If
      Next

      ' we didn't find anything
      Return -1

    End Function

#End Region

  End Class

End Namespace
