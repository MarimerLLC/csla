Imports System.ComponentModel

Namespace Web

  ''' <summary>
  ''' Argument object used in the SelectObject event.
  ''' </summary>
  <Serializable()> _
  Public Class SelectObjectArgs
    Inherits EventArgs

    Private mBusinessObject As Object
    Private mSortExpression As String
    Private mSortProperty As String
    Private mSortDirection As ListSortDirection
    Private mStartRowIndex As Integer
    Private mMaximumRows As Integer
    Private mRetrieveTotalRowCount As Boolean

    ''' <summary>
    ''' Get or set a reference to the business object
    ''' that is created and populated by the SelectObject
    ''' event handler in the web page.
    ''' </summary>
    ''' <value>A reference to a CSLA .NET business object.</value>
    Public Property BusinessObject() As Object
      Get
        Return mBusinessObject
      End Get
      Set(ByVal value As Object)
        mBusinessObject = value
      End Set
    End Property

    ''' <summary>
    ''' Gets the sort expression that should be used to
    ''' sort the data being returned to the data source
    ''' control.
    ''' </summary>
    Public ReadOnly Property SortExpression() As String
      Get
        Return mSortExpression
      End Get
    End Property

    ''' <summary>
    ''' Gets the property name for the sort if only one
    ''' property/column name is specified.
    ''' </summary>
    ''' <remarks>
    ''' If multiple properties/columns are specified
    ''' for the sort, you must parse the value from
    ''' <see cref="SortExpression"/> to find all the
    ''' property names and sort directions for the sort.
    ''' </remarks>
    Public ReadOnly Property SortProperty() As String
      Get
        Return mSortProperty
      End Get
    End Property

    ''' <summary>
    ''' Gets the sort direction for the sort if only
    ''' one property/column name is specified.
    ''' </summary>
    ''' <remarks>
    ''' If multiple properties/columns are specified
    ''' for the sort, you must parse the value from
    ''' <see cref="SortExpression"/> to find all the
    ''' property names and sort directions for the sort.
    ''' </remarks>
    Public ReadOnly Property SortDirection() As ListSortDirection
      Get
        Return mSortDirection
      End Get
    End Property

    ''' <summary>
    ''' Gets the index for the first row that will be
    ''' displayed. This should be the first row in
    ''' the resulting collection set into the
    ''' <see cref="BusinessObject"/> property.
    ''' </summary>
    Public ReadOnly Property StartRowIndex() As Integer
      Get
        Return mStartRowIndex
      End Get
    End Property

    ''' <summary>
    ''' Gets the maximum number of rows that
    ''' should be returned as a result of this
    ''' query. For paged collections, this is the
    ''' page size.
    ''' </summary>
    Public ReadOnly Property MaximumRows() As Integer
      Get
        Return mMaximumRows
      End Get
    End Property

    ''' <summary>
    ''' Gets a value indicating whether the
    ''' query should return the total row count
    ''' through the
    ''' <see cref="Csla.Core.IReportTotalRowCount"/>
    ''' interface.
    ''' </summary>
    Public ReadOnly Property RetrieveTotalRowCount() As Boolean
      Get
        Return mRetrieveTotalRowCount
      End Get
    End Property

    ''' <summary>
    ''' Creates an instance of the object, initializing
    ''' it with values from data binding.
    ''' </summary>
    ''' <param name="args">Values provided from data binding.</param>
    Public Sub New(ByVal args As System.Web.UI.DataSourceSelectArguments)

      mStartRowIndex = args.StartRowIndex
      mMaximumRows = args.MaximumRows
      mRetrieveTotalRowCount = args.RetrieveTotalRowCount

      mSortExpression = args.SortExpression
      If Not String.IsNullOrEmpty(mSortExpression) Then
        If Len(mSortExpression) >= 5 AndAlso Right(mSortExpression, 5) = " DESC" Then
          mSortProperty = Left(mSortExpression, mSortExpression.Length - 5)
          mSortDirection = ListSortDirection.Descending

        Else
          mSortProperty = args.SortExpression
          mSortDirection = ListSortDirection.Ascending
        End If
      End If

    End Sub

  End Class

End Namespace
