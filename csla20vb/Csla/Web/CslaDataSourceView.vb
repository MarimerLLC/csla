Imports System.Web.UI

Namespace Web

  ''' <summary>
  ''' The object responsible for managing data binding
  ''' to a specific CSLA .NET object.
  ''' </summary>
  Public Class CslaDataSourceView
    Inherits DataSourceView

    Private mOwner As CslaDataSource
    Private mTypeName As String
    Private mTypeAssemblyName As String
    Private mTypeSupportsPaging As Boolean
    Private mTypeSupportsSorting As Boolean

    ''' <summary>
    ''' Creates an instance of the object.
    ''' </summary>
    ''' <param name="owner">The CslaDataSource object
    ''' that owns this view.</param>
    ''' <param name="viewName">The name of the view.</param>
    Public Sub New(ByVal owner As CslaDataSource, ByVal viewName As String)

      MyBase.New(owner, viewName)
      mOwner = owner

    End Sub

    ''' <summary>
    ''' Get or set the full type name of the business object
    ''' class to be used as a data source.
    ''' </summary>
    ''' <value>Full type name of the business class.</value>
    Public Property TypeName() As String
      Get
        Return mTypeName
      End Get
      Set(ByVal value As String)
        mTypeName = value
      End Set
    End Property

    ''' <summary>
    ''' Get or set the name of the assembly containing the 
    ''' business object class to be used as a data source.
    ''' </summary>
    ''' <value>Assembly name containing the business class.</value>
    Public Property TypeAssemblyName() As String
      Get
        Return mTypeAssemblyName
      End Get
      Set(ByVal value As String)
        mTypeAssemblyName = value
      End Set
    End Property

    ''' <summary>
    ''' Get or set a value indicating whether the
    ''' business object data source supports paging.
    ''' </summary>
    ''' <remarks>
    ''' To support paging, the business object
    ''' (collection) must implement 
    ''' <see cref="Csla.Core.IReportTotalRowCount"/>.
    ''' </remarks>
    Public Property TypeSupportsPaging() As Boolean
      Get
        Return mTypeSupportsPaging
      End Get
      Set(ByVal value As Boolean)
        mTypeSupportsPaging = value
      End Set
    End Property

    ''' <summary>
    ''' Get or set a value indicating whether the
    ''' business object data source supports sorting.
    ''' </summary>
    Public Property TypeSupportsSorting() As Boolean
      Get
        Return mTypeSupportsSorting
      End Get
      Set(ByVal value As Boolean)
        mTypeSupportsSorting = value
      End Set
    End Property

#Region " Select "

    ''' <summary>
    ''' Implements the select behavior for
    ''' the control by raising the 
    ''' <see cref="CslaDataSource.SelectObject"/> event.
    ''' </summary>
    ''' <param name="arguments">Arguments object.</param>
    ''' <returns>The data returned from the select.</returns>
    Protected Overrides Function ExecuteSelect( _
      ByVal arguments As System.Web.UI.DataSourceSelectArguments) As _
      System.Collections.IEnumerable

      ' get the object from the page
      Dim args As New SelectObjectArgs
      mOwner.OnSelectObject(args)
      Dim result As Object = args.BusinessObject

      If arguments.RetrieveTotalRowCount Then
        Dim rowCount As Integer
        If result Is Nothing Then
          rowCount = 0

        ElseIf TypeOf result Is Csla.Core.IReportTotalRowCount Then
          rowCount = CType(result, Csla.Core.IReportTotalRowCount).TotalRowCount

        ElseIf TypeOf result Is IList Then
          rowCount = CType(result, IList).Count

        ElseIf TypeOf result Is IEnumerable Then
          Dim temp As IEnumerable = CType(result, IEnumerable)
          Dim count As Integer = 0
          For Each item As Object In temp
            count += 1
          Next
          rowCount = count

        Else
          rowCount = 1
        End If
        arguments.TotalRowCount = rowCount
      End If

      ' if the result isn't IEnumerable then
      ' wrap it in a collection
      If Not TypeOf result Is IEnumerable Then
        Dim list As New ArrayList
        If result IsNot Nothing Then
          list.Add(result)
        End If
        result = list
      End If

      ' now return the object as a result
      Return CType(result, IEnumerable)

    End Function

#End Region

#Region " Insert "

    ''' <summary>
    ''' Gets a value indicating whether the data source can
    ''' insert data.
    ''' </summary>
    Public Overrides ReadOnly Property CanInsert() As Boolean
      Get
        If GetType(Csla.Core.IUndoableObject).IsAssignableFrom( _
          CslaDataSource.GetType(mTypeAssemblyName, mTypeName)) Then
          Return True
        Else
          Return False
        End If
      End Get
    End Property

    ''' <summary>
    ''' Implements the insert behavior for
    ''' the control by raising the 
    ''' <see cref="CslaDataSource.InsertObject"/> event.
    ''' </summary>
    ''' <param name="values">The values from
    ''' the UI that are to be inserted.</param>
    ''' <returns>The number of rows affected.</returns>
    Protected Overrides Function ExecuteInsert( _
      ByVal values As System.Collections.IDictionary) As Integer

      ' tell the page to insert the object
      Dim args As New InsertObjectArgs(values)
      mOwner.OnInsertObject(args)
      Return args.RowsAffected

    End Function

#End Region

#Region " Delete "

    ''' <summary>
    ''' Gets a value indicating whether the data source can
    ''' delete data.
    ''' </summary>
    Public Overrides ReadOnly Property CanDelete() As Boolean
      Get
        If GetType(Csla.Core.IUndoableObject).IsAssignableFrom( _
          CslaDataSource.GetType(mTypeAssemblyName, mTypeName)) Then
          Return True
        Else
          Return False
        End If
      End Get
    End Property

    ''' <summary>
    ''' Implements the delete behavior for
    ''' the control by raising the 
    ''' <see cref="CslaDataSource.DeleteObject"/> event.
    ''' </summary>
    ''' <param name="keys">The key values from
    ''' the UI that are to be deleted.</param>
    ''' <param name="oldValues">The old values
    ''' from the UI.</param>
    ''' <returns>The number of rows affected.</returns>
    Protected Overrides Function ExecuteDelete(ByVal keys As System.Collections.IDictionary, ByVal oldValues As System.Collections.IDictionary) As Integer

      ' tell the page to delete the object
      Dim args As New DeleteObjectArgs(keys, oldValues)
      mOwner.OnDeleteObject(args)
      Return args.RowsAffected

    End Function

#End Region

#Region " Update "

    ''' <summary>
    ''' Gets a value indicating whether the data source can
    ''' update data.
    ''' </summary>
    Public Overrides ReadOnly Property CanUpdate() As Boolean
      Get
        If GetType(Csla.Core.IUndoableObject).IsAssignableFrom( _
          CslaDataSource.GetType(mTypeAssemblyName, mTypeName)) Then
          Return True
        Else
          Return False
        End If
      End Get
    End Property

    ''' <summary>
    ''' Implements the update behavior for
    ''' the control by raising the 
    ''' <see cref="CslaDataSource.UpdateObject"/> event.
    ''' </summary>
    ''' <param name="keys">The key values from the UI
    ''' that identify the object to be updated.</param>
    ''' <param name="values">The values from
    ''' the UI that are to be inserted.</param>
    ''' <param name="oldValues">The old values
    ''' from the UI.</param>
    ''' <returns>The number of rows affected.</returns>
    Protected Overrides Function ExecuteUpdate(ByVal keys As System.Collections.IDictionary, ByVal values As System.Collections.IDictionary, ByVal oldValues As System.Collections.IDictionary) As Integer

      ' tell the page to update the object
      Dim args As New UpdateObjectArgs(keys, values, oldValues)
      mOwner.OnUpdateObject(args)
      Return args.RowsAffected

    End Function

#End Region

#Region " Other Operations "

    ''' <summary>
    ''' Gets a value indicating whether the data source supports
    ''' paging of the data. Always returns <see langword="false"/>.
    ''' </summary>
    Public Overrides ReadOnly Property CanPage() As Boolean
      Get
        Return mTypeSupportsPaging
      End Get
    End Property

    ''' <summary>
    ''' Gets a value indicating whether the data source can
    ''' retrieve the total number of rows of data. Always
    ''' returns <see langword="true"/>.
    ''' </summary>
    Public Overrides ReadOnly Property CanRetrieveTotalRowCount() As Boolean
      Get
        Return True
      End Get
    End Property

    ''' <summary>
    ''' Gets a alue indicating whether the data source supports
    ''' sorting of the data. Always returns <see langword="false"/>.
    ''' </summary>
    Public Overrides ReadOnly Property CanSort() As Boolean
      Get
        Return mTypeSupportsSorting
      End Get
    End Property

#End Region

  End Class

End Namespace
