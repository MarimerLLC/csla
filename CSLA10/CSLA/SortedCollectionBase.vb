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
  ''' This class inhirits from Core.BindableCollectionBase and adds
  ''' sorting capability to collections. BusinessCollectionBase inherits
  ''' from this class, and business collections should inherit from
  ''' BusinessCollectionBase. Core.SortedCollectionBase is for internal
  ''' framework use only.
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

#Region " Properties "

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
    ''' Structure to store temporary data for sorting.
    ''' </summary>
    Private Structure SortData
      Private mKey As Object
      Public Value As Object

      Public Sub New(ByVal Key As Object, ByVal Value As Object)
        mKey = Key
        Me.Value = Value
      End Sub

      Public ReadOnly Property Key() As Object
        Get
          If TypeOf mKey Is IComparable Then
            Return mKey
          Else
            Return mKey.ToString
          End If
        End Get
      End Property
    End Structure

    ''' <summary>
    ''' Contains code to compare SortData structures
    ''' </summary>
    ''' <remarks>
    ''' This performs a case sensitive comparison. If you want a case insensitive
    ''' comparison, change the code to use CaseInsensitiveComparer.Default instead.
    ''' </remarks>
    Private Class SortDataCompare
      Implements IComparer

      Public Function Compare(ByVal x As Object, ByVal y As Object) As Integer Implements System.Collections.IComparer.Compare
        Dim item1 As SortData = DirectCast(x, SortData)
        Dim item2 As SortData = DirectCast(y, SortData)

        Return Comparer.Default.Compare(item1.Key, item2.Key)

      End Function
    End Class

    ''' <summary>
    ''' Applies a sort to the collection.
    ''' </summary>
    ''' <remarks>
    ''' This method is invoked via the IBindingList interface and is not
    ''' intended for use by code in your business class.
    ''' </remarks>
    Protected Overrides Sub IBindingList_ApplySort(ByVal [property] As System.ComponentModel.PropertyDescriptor, ByVal direction As System.ComponentModel.ListSortDirection)

      If Not AllowSort Then
        Throw New NotSupportedException(GetResourceString("SortingNotSupportedException"))
      End If

      mSortProperty = [property]
      mSortPropertyName = mSortProperty.Name
      mListSortDirection = direction

      If Not mIsSorted AndAlso list.Count > 0 Then
        ' this is our first time sorting so
        ' make sure to store the original order
        mUnsortedList = New ArrayList()
        Dim item As Object
        For Each item In list
          mUnsortedList.Add(item)
        Next
      End If

      If list.Count > 1 Then
        Try
          Dim count As Integer
          mActivelySorting = True

          ' copy the key/value pairs into a sorted list
          Dim sortList As New ArrayList()
          For count = 0 To list.Count - 1
            sortList.Add(New SortData(CallByName(list.Item(count), mSortPropertyName, CallType.Get), list.Item(count)))
          Next
          sortList.Sort(New SortDataCompare())

          list.Clear()

          If direction = ListSortDirection.Ascending Then
            Dim item As SortData
            For Each item In sortList
              list.Add(item.Value)
            Next

          Else ' direction = ListSortDirection.Descending
            Dim item As SortData
            For count = sortList.Count - 1 To 0 Step -1
              item = DirectCast(sortList(count), SortData)
              list.Add(item.value)
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

      If Not AllowSort Then
        Throw New NotSupportedException(GetResourceString("SortingNotSupportedException"))
      End If

      If mIsSorted Then
        mActivelySorting = True

        'Return the list to its unsorted state
        list.Clear()

        Dim item As Object
        For Each item In mUnsortedList
          list.Add(item)
        Next

        mUnsortedList = Nothing

        mIsSorted = False
        mSortProperty = Nothing
        mSortPropertyName = ""
        mListSortDirection = ListSortDirection.Ascending
        mActivelySorting = False
      End If

    End Sub

#End Region

#Region " Collection events "

    ''' <summary>
    ''' Ensures that any sort is maintained as a new item is inserted.
    ''' </summary>
    Protected Overrides Sub OnInsertComplete(ByVal index As Integer, ByVal value As Object)
      If mIsSorted AndAlso Not ActivelySorting Then
        mUnsortedList.Add(value)
      End If
      MyBase.OnInsertComplete(index, value)
    End Sub

    ''' <summary>
    ''' Ensures that any sort is maintained as the list is cleared.
    ''' </summary>
    Protected Overrides Sub OnClearComplete()
      If mIsSorted AndAlso Not ActivelySorting Then
        mUnsortedList.Clear()
      End If
      MyBase.OnClearComplete()
    End Sub

    ''' <summary>
    ''' Ensures that any sort is maintained as an item is removed.
    ''' </summary>
    Protected Overrides Sub OnRemoveComplete(ByVal index As Integer, ByVal value As Object)
      If mIsSorted AndAlso Not ActivelySorting Then
        mUnsortedList.Remove(value)
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
        Throw New NotSupportedException(GetResourceString("SearchingNotSupportedException"))
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
