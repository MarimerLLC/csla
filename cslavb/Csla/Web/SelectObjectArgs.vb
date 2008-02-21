Imports System.ComponentModel

Namespace Web

  ''' <summary>
  ''' Argument object used in the SelectObject event.
  ''' </summary>
  <Serializable()> _
  Public Class SelectObjectArgs
    Inherits EventArgs

    Private _businessObject As Object
    Private _sortExpression As String
    Private _sortProperty As String
    Private _sortDirection As ListSortDirection
    Private _startRowIndex As Integer
    Private _maximumRows As Integer
    Private _retrieveTotalRowCount As Boolean

    ''' <summary>
    ''' Get or set a reference to the business object
    ''' that is created and populated by the SelectObject
    ''' event handler in the web page.
    ''' </summary>
    ''' <value>A reference to a CSLA .NET business object.</value>
    Public Property BusinessObject() As Object
      Get
        Return _businessObject
      End Get
      Set(ByVal value As Object)
        _businessObject = value
      End Set
    End Property

    ''' <summary>
    ''' Gets the sort expression that should be used to
    ''' sort the data being returned to the data source
    ''' control.
    ''' </summary>
    Public ReadOnly Property SortExpression() As String
      Get
        Return _sortExpression
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
        Return _sortProperty
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
        Return _sortDirection
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
        Return _startRowIndex
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
        Return _maximumRows
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
        Return _retrieveTotalRowCount
      End Get
    End Property

    ''' <summary>
    ''' Creates an instance of the object, initializing
    ''' it with values from data binding.
    ''' </summary>
    ''' <param name="args">Values provided from data binding.</param>
    Public Sub New(ByVal args As System.Web.UI.DataSourceSelectArguments)

      _startRowIndex = args.StartRowIndex
      _maximumRows = args.MaximumRows
      _retrieveTotalRowCount = args.RetrieveTotalRowCount

      _sortExpression = args.SortExpression
      If Not String.IsNullOrEmpty(_sortExpression) Then
        If Len(_sortExpression) >= 5 AndAlso Right(_sortExpression, 5) = " DESC" Then
          _sortProperty = Left(_sortExpression, _sortExpression.Length - 5)
          _sortDirection = ListSortDirection.Descending

        Else
          _sortProperty = args.SortExpression
          _sortDirection = ListSortDirection.Ascending
        End If
      End If

    End Sub

  End Class

End Namespace
