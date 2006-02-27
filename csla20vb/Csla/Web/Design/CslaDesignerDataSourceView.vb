Imports System.Web.UI
Imports System.Web.UI.Design
Imports System.ComponentModel
Imports System.Reflection

Namespace Web.Design

  ''' <summary>
  ''' Object responsible for providing details about
  ''' data binding to a specific CSLA .NET object.
  ''' </summary>
  Public Class CslaDesignerDataSourceView
    Inherits DesignerDataSourceView

    Private mOwner As CslaDataSourceDesigner = Nothing

    ''' <summary>
    ''' Creates an instance of the object.
    ''' </summary>
    Public Sub New(ByVal owner As CslaDataSourceDesigner, ByVal viewName As String)

      MyBase.New(owner, viewName)
      mOwner = owner

    End Sub

    ''' <summary>
    ''' Returns a set of sample data used to populate
    ''' controls at design time.
    ''' </summary>
    ''' <param name="minimumRows">Minimum number of sample rows
    ''' to create.</param>
    ''' <param name="isSampleData">Returns True if the data
    ''' is sample data.</param>
    Public Overrides Function GetDesignTimeData( _
        ByVal minimumRows As Integer, _
        ByRef isSampleData As Boolean) As IEnumerable

      Dim schema As IDataSourceViewSchema = Me.Schema
      Dim result As New DataTable

      ' create the columns
      For Each item As IDataSourceFieldSchema In schema.GetFields
        result.Columns.Add(item.Name, item.DataType)
      Next

      ' create sample data
      For index As Integer = 1 To minimumRows
        Dim values(result.Columns.Count - 1) As Object
        Dim colIndex As Integer = 0
        For Each col As DataColumn In result.Columns
          If col.DataType.Equals(GetType(String)) Then
            values(colIndex) = "abc"
          ElseIf col.DataType.Equals(GetType(Date)) Then
            values(colIndex) = Today.ToShortDateString
          ElseIf col.DataType.Equals(GetType(Boolean)) Then
            values(colIndex) = False
          ElseIf col.DataType.IsPrimitive Then
            values(colIndex) = index
          ElseIf col.DataType.Equals(GetType(Guid)) Then
            values(colIndex) = Guid.Empty
          ElseIf col.DataType.IsValueType Then
            values(colIndex) = _
              Activator.CreateInstance(col.DataType)
          Else
            values(colIndex) = Nothing
          End If
          colIndex += 1
        Next
        result.LoadDataRow(values, LoadOption.OverwriteChanges)
      Next

      isSampleData = True
      Return CType(result.DefaultView, IEnumerable)

    End Function

    ''' <summary>
    ''' Returns schema information corresponding to the properties
    ''' of the CSLA .NET business object.
    ''' </summary>
    ''' <remarks>
    ''' All public properties are returned except for those marked
    ''' with the <see cref="BrowsableAttribute">Browsable attribute</see>
    ''' as False.
    ''' </remarks>
    Public Overrides ReadOnly Property Schema() As IDataSourceViewSchema
      Get
        Return New ObjectSchema( _
          mOwner.DataSourceControl.TypeAssemblyName, _
          mOwner.DataSourceControl.TypeName).GetViews(0)
      End Get
    End Property

    ''' <summary>
    ''' Get a value indicating whether data binding can retrieve
    ''' the total number of rows of data.
    ''' </summary>
    Public Overrides ReadOnly Property CanRetrieveTotalRowCount() As Boolean
      Get
        Return True
      End Get
    End Property

    ''' <summary>
    ''' Get a value indicating whether data binding can directly
    ''' delete the object.
    ''' </summary>
    ''' <remarks>
    ''' If this returns true, the web page must handle the
    ''' <see cref="CslaDataSource.DeleteObject">DeleteObject</see>
    ''' event.
    ''' </remarks>
    Public Overrides ReadOnly Property CanDelete() As Boolean
      Get
        Dim objectType As Type = CslaDataSource.GetType( _
            mOwner.DataSourceControl.TypeAssemblyName, mOwner.DataSourceControl.TypeName)
        If GetType(Csla.Core.IUndoableObject).IsAssignableFrom(objectType) Then
          Return True
        ElseIf objectType.GetMethod("Remove") IsNot Nothing Then
          Return True
        Else
          Return False
        End If
      End Get
    End Property

    ''' <summary>
    ''' Get a value indicating whether data binding can directly
    ''' insert an instance of the object.
    ''' </summary>
    ''' <remarks>
    ''' If this returns true, the web page must handle the
    ''' <see cref="CslaDataSource.InsertObject">InsertObject</see>
    ''' event.
    ''' </remarks>
    Public Overrides ReadOnly Property CanInsert() As Boolean
      Get
        If GetType(Csla.Core.IUndoableObject).IsAssignableFrom(CslaDataSource.GetType( _
            mOwner.DataSourceControl.TypeAssemblyName, mOwner.DataSourceControl.TypeName)) Then
          Return True
        Else
          Return False
        End If
      End Get
    End Property

    ''' <summary>
    ''' Get a value indicating whether data binding can directly
    ''' update or edit the object.
    ''' </summary>
    ''' <remarks>
    ''' If this returns true, the web page must handle the
    ''' <see cref="CslaDataSource.UpdateObject">UpdateObject</see>
    ''' event.
    ''' </remarks>
    Public Overrides ReadOnly Property CanUpdate() As Boolean
      Get
        Dim objType As Type = CslaDataSource.GetType( _
          mOwner.DataSourceControl.TypeAssemblyName, _
          mOwner.DataSourceControl.TypeName)
        If GetType(Csla.Core.IUndoableObject).IsAssignableFrom(objType) Then
          Return True

        Else
          Return False
        End If
      End Get
    End Property

    ''' <summary>
    ''' Gets a value indicating whether the data source supports
    ''' paging.
    ''' </summary>
    Public Overrides ReadOnly Property CanPage() As Boolean
      Get
        Return False
      End Get
    End Property

    ''' <summary>
    ''' Gets a value indicating whether the data source supports
    ''' sorting.
    ''' </summary>
    Public Overrides ReadOnly Property CanSort() As Boolean
      Get
        Return False
      End Get
    End Property

  End Class

End Namespace
