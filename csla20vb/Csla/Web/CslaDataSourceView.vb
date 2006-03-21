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

#Region " Select "

    Protected Overrides Function ExecuteSelect( _
      ByVal arguments As System.Web.UI.DataSourceSelectArguments) As _
      System.Collections.IEnumerable

      ' get the object from the page
      Dim args As New SelectObjectArgs
      mOwner.OnSelectObject(args)
      Dim obj As Object = args.BusinessObject

      Dim result As Object
      If arguments.RetrieveTotalRowCount Then
        If obj Is Nothing Then
          result = 0

        ElseIf TypeOf obj Is IList Then
          result = CType(obj, IList).Count

        ElseIf TypeOf obj Is IEnumerable Then
          Dim temp As IEnumerable = CType(obj, IEnumerable)
          Dim count As Integer = 0
          For Each item As Object In temp
            count += 1
          Next
          result = count

        Else
          result = 1
        End If

      Else
        result = obj
      End If

      ' if the result isn't IEnumerable then
      ' wrap it in a collection
      If Not TypeOf result Is IEnumerable Then
        Dim list As New ArrayList
        list.Add(result)
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
        Return False
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
        Return False
      End Get
    End Property

#End Region

  End Class

End Namespace
