Imports System.ComponentModel

<System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")> _
Public Class SortedBindingList(Of T)
  Inherits BindingList(Of T)

  ' Inspired by code from Michael Weinhardt
  ' http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dnforms/html/winforms02182005.asp

  Private WithEvents mSource As IBindingList

  Public Sub New()
    ' allow normal construction
  End Sub

  Public Sub New(ByVal sourceList As IBindingList)

    mSource = sourceList
    ' copy references into Me for sorting
    ' without impacting the original list
    For Each item As T In mSource
      Me.Add(item)
    Next

  End Sub

#Region " Sorting "

  Private mIsSorted As Boolean
  Private mProperty As PropertyDescriptor
  Private mDirection As ListSortDirection

  Protected Overrides Sub ApplySortCore(ByVal prop As PropertyDescriptor, ByVal direction As ListSortDirection)

    Dim items As List(Of T) = DirectCast(Me.Items, List(Of T))

    If items IsNot Nothing Then
      Dim pc As PropertyComparer = New PropertyComparer(prop, direction)
      items.Sort(pc)
      mProperty = prop
      mDirection = direction
      mIsSorted = True
    Else
      mIsSorted = False
    End If

    OnListChanged(New ListChangedEventArgs(ListChangedType.Reset, -1))

  End Sub

  Protected Overrides Sub RemoveSortCore()

    mIsSorted = False
    If mSource IsNot Nothing Then
      Me.Clear()
      For Each item As T In mSource
        Me.Add(item)
      Next
    End If

  End Sub

  Protected Overrides ReadOnly Property SupportsSortingCore() As Boolean
    Get
      Return True
    End Get
  End Property

  Protected Overrides ReadOnly Property IsSortedCore() As Boolean
    Get
      Return mIsSorted
    End Get
  End Property

  Protected Overrides ReadOnly Property SortDirectionCore() As System.ComponentModel.ListSortDirection
    Get
      Return mDirection
    End Get
  End Property

  Protected Overrides ReadOnly Property SortPropertyCore() As System.ComponentModel.PropertyDescriptor
    Get
      Return mProperty
    End Get
  End Property

#End Region

#Region " PropertyComparer class "

  Public Class PropertyComparer
    Implements System.Collections.Generic.IComparer(Of T)

    Private mProperty As PropertyDescriptor
    Private mDirection As ListSortDirection

    Public Sub New(ByVal [property] As PropertyDescriptor, ByVal direction As ListSortDirection)
      mProperty = [property]
      mDirection = direction
    End Sub

    Public Function Compare(ByVal val1 As T, ByVal val2 As T) As Integer _
      Implements IComparer(Of T).Compare

      Dim prop1 As Object = CallByName(val1, mProperty.Name, CallType.Get)
      Dim prop2 As Object = CallByName(val2, mProperty.Name, CallType.Get)

      If mDirection = ListSortDirection.Ascending Then
        Return CompareValues(prop1, prop2)
      Else
        Return CompareValues(prop1, prop2) * -1
      End If
    End Function

    Private Function CompareValues(ByVal val1 As Object, ByVal val2 As Object) As Integer

      Dim result As Integer

      If TypeOf val1 Is IComparable Then
        result = DirectCast(val1, IComparable).CompareTo(val2)
      ElseIf val1.Equals(val2) Then
        result = 0
      Else
        result = val1.ToString.CompareTo(val2.ToString)
      End If
      Return result
    End Function

  End Class

#End Region

  Private Sub mSource_ListChanged(ByVal sender As Object, ByVal e As System.ComponentModel.ListChangedEventArgs) Handles mSource.ListChanged

    Select Case e.ListChangedType
      Case ListChangedType.ItemAdded
        Me.Add(DirectCast(mSource.Item(e.NewIndex), T))
        Me.ApplySortCore(Me.SortPropertyCore, Me.SortDirectionCore)

      Case ListChangedType.ItemChanged
        Me.ApplySortCore(Me.SortPropertyCore, Me.SortDirectionCore)

      Case ListChangedType.Reset, ListChangedType.ItemDeleted
        Me.Clear()
        For Each item As T In mSource
          Me.Add(item)
        Next
    End Select

  End Sub

End Class
