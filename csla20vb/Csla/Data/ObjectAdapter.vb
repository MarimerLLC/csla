Imports System.ComponentModel
Imports System.Reflection

Namespace Data

  ''' <summary>
  ''' An ObjectAdapter is used to convert data in an object 
  ''' or collection into a DataTable.
  ''' </summary>
  Public Class ObjectAdapter

    'Private mColumns As New ArrayList

    ''' <summary>
    ''' Fills the DataSet with data from an object or collection.
    ''' </summary>
    ''' <remarks>
    ''' The name of the DataTable being filled is will be the class name of
    ''' the object acting as the data source. The
    ''' DataTable will be inserted if it doesn't already exist in the DataSet.
    ''' </remarks>
    ''' <param name="ds">A reference to the DataSet to be filled.</param>
    ''' <param name="source">A reference to the object or collection acting as a data source.</param>
    Public Sub Fill(ByVal ds As DataSet, ByVal source As Object)

      Dim className As String
      className = TypeName(source)

      Fill(ds, className, source)

    End Sub

    ''' <summary>
    ''' Fills the DataSet with data from an object or collection.
    ''' </summary>
    ''' <remarks>
    ''' The name of the DataTable being filled is specified as a parameter. The
    ''' DataTable will be inserted if it doesn't already exist in the DataSet.
    ''' </remarks>
    ''' <param name="ds">A reference to the DataSet to be filled.</param>
    ''' <param name="TableName"></param>
    ''' <param name="source">A reference to the object or collection acting as a data source.</param>
    Public Sub Fill(ByVal ds As DataSet, _
      ByVal tableName As String, ByVal source As Object)

      Dim dt As DataTable
      Dim exists As Boolean

      dt = ds.Tables(tableName)
      exists = Not dt Is Nothing

      If Not exists Then
        dt = New DataTable(tableName)
      End If

      Fill(dt, source)

      If Not exists Then
        ds.Tables.Add(dt)
      End If

    End Sub

    ''' <summary>
    ''' Fills a DataTable with data values from an object or collection.
    ''' </summary>
    ''' <param name="dt">A reference to the DataTable to be filled.</param>
    ''' <param name="source">A reference to the object or collection acting as a data source.</param>
    Public Sub Fill(ByVal dt As DataTable, ByVal source As Object)

      If source Is Nothing Then
        Throw New ArgumentException(My.Resources.NothingNotValid)
      End If

      Dim columns As List(Of String) = GetColumns(source)
      If columns.Count < 1 Then Exit Sub

      ' create columns if needed
      For Each column As String In columns
        If Not dt.Columns.Contains(column) Then
          dt.Columns.Add(column)
        End If
      Next

      ' get an IList and copy the data
      CopyData(dt, GetIList(source), columns)


    End Sub

#Region " DataCopyIList "

    Private Function GetIList(ByVal source As Object) As IList

      If TypeOf source Is IListSource Then
        Return CType(source, IListSource).GetList

      ElseIf TypeOf source Is IList Then
        Return CType(source, IList)

      Else
        ' they gave us a regular object - create a list
        Dim col As New ArrayList
        col.Add(source)
        Return CType(col, IList)
      End If

    End Function

    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")> _
    Private Sub CopyData(ByVal dt As DataTable, ByVal ds As IList, ByVal columns As List(Of String))

      ' load the data into the DataTable
      dt.BeginLoadData()
      For index As Integer = 0 To ds.Count - 1
        Dim dr As DataRow = dt.NewRow
        For Each column As String In columns
          Try
            dr(column) = GetField(ds(index), column)

          Catch ex As Exception
            dr(column) = ex.Message
          End Try
        Next
        dt.Rows.Add(dr)
      Next
      dt.EndLoadData()

    End Sub

#End Region

#Region " GetColumns "

    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")> _
    Private Function GetColumns(ByVal source As Object) As List(Of String)

      Dim innerSource As Object

      If TypeOf source Is IListSource Then
        innerSource = CType(source, IListSource).GetList

      Else
        innerSource = source
      End If

      If TypeOf innerSource Is DataView Then
        Return ScanDataView(CType(innerSource, DataView))

      ElseIf TypeOf innerSource Is IEnumerable Then
        Dim childType As Type = Utilities.GetChildItemType(innerSource.GetType)
        Return ScanObject(childType)

      Else
        ' they gave us a regular object
        Return ScanObject(innerSource.GetType)
      End If

    End Function

    Private Function ScanDataView(ByVal ds As DataView) As List(Of String)

      Dim result As New List(Of String)

      Dim field As Integer

      For field = 0 To ds.Table.Columns.Count - 1
        result.Add(ds.Table.Columns(field).ColumnName)
      Next
      Return result

    End Function

    'Private Sub ScanIList(ByVal ds As IList)

    '  If ds.Count > 0 Then
    '    ' retrieve the first item from the list
    '    Dim obj As Object = ds.Item(0)

    '    If TypeOf obj Is ValueType AndAlso obj.GetType.IsPrimitive Then
    '      ' the value is a primitive value type
    '      mColumns.Add("Value")

    '    ElseIf TypeOf obj Is String Then
    '      ' the value is a simple string
    '      mColumns.Add("Text")

    '    Else
    '      ' we have a complex Structure or object
    '      ScanObject(obj)
    '    End If
    '  End If

    'End Sub

    Private Function ScanObject(ByVal sourceType As Type) As List(Of String)

      Dim result As New List(Of String)

      ' retrieve a list of all public properties
      Dim props As PropertyInfo() = sourceType.GetProperties()
      If UBound(props) >= 0 Then
        For column As Integer = 0 To UBound(props)
          If props(column).CanRead Then
            result.Add(props(column).Name)
          End If
        Next
      End If

      ' retrieve a list of all public fields
      Dim fields As FieldInfo() = sourceType.GetFields()
      If UBound(fields) >= 0 Then
        For column As Integer = 0 To UBound(fields)
          result.Add(fields(column).Name)
        Next
      End If

      Return result

    End Function

#End Region

#Region " GetField "

    <System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")> _
    Private Shared Function GetField(ByVal obj As Object, ByVal fieldName As String) As String

      If TypeOf obj Is DataRowView Then
        ' this is a DataRowView from a DataView
        Return CType(obj, DataRowView).Item(fieldName).ToString

      ElseIf TypeOf obj Is ValueType AndAlso obj.GetType.IsPrimitive Then
        ' this is a primitive value type
        Return obj.ToString

      ElseIf TypeOf obj Is String Then
        ' this is a simple string
        Return CStr(obj)

      Else
        ' this is an object or Structure
        Try
          Dim sourcetype As Type = obj.GetType

          ' see if the field is a property
          Dim prop As PropertyInfo = sourcetype.GetProperty(fieldName)

          If prop Is Nothing OrElse Not prop.CanRead Then
            ' no readable property of that name exists - check for a field
            Dim field As FieldInfo = sourcetype.GetField(fieldName)

            If field Is Nothing Then
              ' no field exists either, throw an exception
              Throw New System.Data.DataException( _
                My.Resources.NoSuchValueExistsException & " " & fieldName)

            Else
              ' got a field, return its value
              Return field.GetValue(obj).ToString
            End If

          Else
            ' found a property, return its value
            Return prop.GetValue(obj, Nothing).ToString
          End If

        Catch ex As Exception
          Throw New System.Data.DataException( _
            My.Resources.ErrorReadingValueException & " " & fieldName, ex)
        End Try
      End If
    End Function

#End Region

  End Class

End Namespace
