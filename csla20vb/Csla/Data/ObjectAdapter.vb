Imports System.ComponentModel
Imports System.Reflection

Namespace Data

  ''' <summary>
  ''' An ObjectAdapter is used to convert data in an object 
  ''' or collection into a DataTable.
  ''' </summary>
  Public Class ObjectAdapter

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
    Private Sub CopyData( _
      ByVal dt As DataTable, _
      ByVal ds As IList, ByVal columns As List(Of String))

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

    Private Function GetColumns( _
      ByVal source As Object) As List(Of String)

      Dim result As List(Of String)
      ' first handle DataSet/DataTable
      Dim innerSource As Object
      Dim iListSource As IListSource = TryCast(source, IListSource)
      If iListSource IsNot Nothing Then
        innerSource = iListSource.GetList

      Else
        innerSource = source
      End If

      Dim dataView As DataView = TryCast(innerSource, DataView)
      If dataView IsNot Nothing Then
        result = ScanDataView(CType(innerSource, DataView))
      End If

      ' now handle lists/arrays/collections
      Dim iEnumerable As IEnumerable = _
        TryCast(innerSource, IEnumerable)
      If iEnumerable IsNot Nothing Then
        Dim childType As Type = _
          Utilities.GetChildItemType(innerSource.GetType)
        result = ScanObject(childType)

      Else
        ' the source is a regular object
        result = ScanObject(innerSource.GetType)
      End If
      Return result

    End Function

    Private Function ScanDataView(ByVal ds As DataView) As List(Of String)

      Dim result As New List(Of String)

      Dim field As Integer

      For field = 0 To ds.Table.Columns.Count - 1
        result.Add(ds.Table.Columns(field).ColumnName)
      Next
      Return result

    End Function

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

    Private Shared Function GetField( _
      ByVal obj As Object, ByVal fieldName As String) As String

      Dim result As String
      Dim dataRowView As DataRowView = TryCast(obj, DataRowView)
      If dataRowView IsNot Nothing Then
        ' this is a DataRowView from a DataView
        result = dataRowView.Item(fieldName).ToString

      ElseIf TypeOf obj Is ValueType AndAlso obj.GetType.IsPrimitive Then
        ' this is a primitive value type
        result = obj.ToString

      Else
        Dim tmp As String = TryCast(obj, String)
        If tmp IsNot Nothing Then
          ' this is a simple string
          result = obj.ToString

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
                result = field.GetValue(obj).ToString
              End If

            Else
              ' found a property, return its value
              result = prop.GetValue(obj, Nothing).ToString
            End If

          Catch ex As Exception
            Throw New System.Data.DataException( _
              My.Resources.ErrorReadingValueException & " " & fieldName, ex)
          End Try
        End If
      End If
      Return result

    End Function

#End Region

  End Class

End Namespace
