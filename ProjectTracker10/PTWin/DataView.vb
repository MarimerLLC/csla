Imports System.ComponentModel
Imports System.Reflection

Public Class DataListView
  Inherits System.Windows.Forms.ListView

  Private mDataSource As Object
  Private WithEvents mItems As DataColumnHeaderCollection
  Private mAutoDiscover As Boolean
  Private mFieldsChanged As Boolean = True

#Region " DataColumnHeaderCollection "

  Public Class DataColumnHeaderCollection
    Inherits CollectionBase

    Public Event Invalidate()

    Protected Sub OnInvalidate()
      RaiseEvent Invalidate()
    End Sub

    Default Public ReadOnly Property Item(ByVal Index As Integer) As DataColumnHeader
      Get
        Return CType(list.Item(Index), DataColumnHeader)
      End Get
    End Property

    Public Sub Add(ByVal Field As String)
      Dim col As New DataColumnHeader()
      col.Text = Field
      col.Field = Field
      list.Add(col)
    End Sub

    Public Sub Add(ByVal Field As String, ByVal Width As Integer)
      Dim col As New DataColumnHeader()
      col.Text = Field
      col.Field = Field
      col.Width = Width
      list.Add(col)
    End Sub

    Public Sub Add(ByVal Text As String, ByVal Field As String)
      Dim col As New DataColumnHeader()
      col.Text = Text
      col.Field = Field
      list.Add(col)
    End Sub

    Public Sub Add(ByVal Text As String, ByVal Field As String, ByVal Width As Integer)
      Dim col As New DataColumnHeader()
      col.Text = Text
      col.Field = Field
      col.Width = Width
      list.Add(col)
    End Sub

    Public Sub Add(ByVal Item As DataColumnHeader)
      list.Add(Item)
    End Sub

    Protected Overrides Sub OnRemoveComplete(ByVal index As Integer, ByVal value As Object)
      OnInvalidate()
    End Sub

    Protected Overrides Sub OnInsertComplete(ByVal index As Integer, ByVal value As Object)
      OnInvalidate()
    End Sub

    Protected Overrides Sub OnSetComplete(ByVal index As Integer, ByVal oldValue As Object, ByVal newValue As Object)
      OnInvalidate()
    End Sub

    Protected Overrides Sub OnClearComplete()
      OnInvalidate()
    End Sub
  End Class

#End Region

#Region " DataColumnHeader "

  Public Class DataColumnHeader
    Inherits ColumnHeader

    Private mField As String

    Public Property Field() As String
      Get
        Return mField
      End Get
      Set(ByVal Value As String)
        mField = Value
      End Set
    End Property

  End Class

#End Region

  Public Sub New()
    mItems = New DataColumnHeaderCollection()
    View = View.Details
    FullRowSelect = True
    MultiSelect = False
    Size = New System.Drawing.Size(296, 216)
  End Sub

  <Category("Data"), TypeConverter("System.Windows.Forms.Design.DataSourceConverter")> _
  Public Property DataSource() As Object
    Get
      Return mDataSource
    End Get
    Set(ByVal Value As Object)
      mDataSource = Value
      If mAutoDiscover Then
        DoAutoDiscover(mDataSource)
      End If
      DataBind()
    End Set
  End Property

  <Category("Data")> _
  Public Property DisplayMember() As String
    Get
      If mItems.Count > 0 Then
        Return mItems.Item(0).Text
      Else
        Return ""
      End If
    End Get
    Set(ByVal Value As String)
      If mItems.Count > 0 Then
        With mItems(0)
          .Text = Value
          .Field = Value
        End With

      Else
        mItems.Add(Value)
      End If
    End Set
  End Property

  Public Shadows ReadOnly Property Columns() As DataColumnHeaderCollection
    Get
      Return mItems
    End Get
  End Property

  Public Shadows Sub Clear()
    mItems.Clear()
    MyBase.Clear()
  End Sub

  Private Sub DataBind()
    If mDataSource Is Nothing Then Exit Sub
    If mItems.Count < 1 Then Exit Sub

    If TypeOf mDataSource Is IListSource Then
      DataBindIList(CType(mDataSource, IListSource).GetList)
    Else
      DataBindIList(CType(mDataSource, IList))
    End If
  End Sub

  Private Sub DataBindIList(ByVal ds As IList)
    Dim index As Integer
    Dim fields As Integer
    Dim item As DataColumnHeader

    ' init column headers if needed
    If mFieldsChanged Then
      mFieldsChanged = False
      MyBase.Clear()
      If mItems.Count = 0 Then
        MyBase.Columns.Add(New DataColumnHeader())

      Else
        For fields = 0 To mItems.Count - 1
          item = CType(mItems(fields), DataColumnHeader)
          MyBase.Columns.Add(mItems(fields))
        Next
      End If

    Else
      Items.Clear()
    End If

    ' load the data into the control
    For index = 0 To ds.Count - 1
      Dim l As New ListViewItem()

      ' load the primary field (DataMember)
      item = CType(mItems(0), DataColumnHeader)
      l.Text = GetField(ds.Item(index), item.Field).ToString

      ' load all subfields
      For fields = 1 To mItems.Count - 1
        item = CType(mItems(fields), DataColumnHeader)
        l.SubItems.Add(GetField(ds.Item(index), item.Field).ToString)
      Next
      Items.Add(l)
    Next
  End Sub

  Private Sub DoAutoDiscover(ByVal ds As DataTable)
    Dim dt As DataTable = CType(mDataSource, DataTable)
    Dim field As Integer
    Dim col As DataColumnHeader

    For field = 0 To dt.Columns.Count - 1
      col = New DataColumnHeader()
      col.Text = dt.Columns(field).Caption
      col.Field = dt.Columns(field).ColumnName
      mItems.Add(col)
    Next
  End Sub

  Private Sub DoAutoDiscover(ByVal ds As DataView)
    Dim dv As DataView = CType(mDataSource, DataView)

    Dim field As Integer
    Dim col As DataColumnHeader

    For field = 0 To dv.Table.Columns.Count - 1
      col = New DataColumnHeader()
      col.Text = dv.Table.Columns(field).Caption
      col.Field = dv.Table.Columns(field).ColumnName
      mItems.Add(col)
    Next
  End Sub

  Private Sub DoAutoDiscover(ByVal ds As IListSource)
    DoAutoDiscover(ds.GetList)
  End Sub

  Private Sub DoAutoDiscover(ByVal ds As IList)
    If ds.Count > 0 Then
      EnumerateFields(ds.Item(0))
    End If
  End Sub

  Private Sub EnumerateFields(ByVal obj As ValueType)
    Dim col As DataColumnHeader
    col = New DataColumnHeader()
    mItems.Add(col)
  End Sub

  Private Sub EnumerateFields(ByVal obj As String)
    Dim col As DataColumnHeader
    col = New DataColumnHeader()
    mItems.Add(col)
  End Sub

  Private Sub EnumerateFields(ByVal obj As Object)
    Dim sourcetype As Type = obj.GetType

  End Sub

  Private Function GetField(ByVal obj As DataRow, ByVal FieldName As String) As String
    Return CType(obj, DataRow).Item(FieldName).ToString
  End Function

  Private Function GetField(ByVal obj As DataRowView, ByVal FieldName As String) As String
    Return CType(obj, DataRowView).Item(FieldName).ToString
  End Function

  Private Function GetField(ByVal obj As ValueType, ByVal FieldName As String) As String
    Return obj.ToString
  End Function

  Private Function GetField(ByVal obj As String, ByVal FieldName As String) As String
    Return obj
  End Function

  Private Function GetField(ByVal obj As Object, ByVal FieldName As String) As String
    Try
      Dim sourcetype As Type = obj.GetType

      Dim prop As PropertyInfo = sourcetype.GetProperty(FieldName)
      If prop Is Nothing OrElse Not prop.CanRead Then
        ' no readable property of that name exists - check for a field
        Dim field As FieldInfo = sourcetype.GetField(FieldName)
        If field Is Nothing Then
          ' no field exists either, return the field name
          ' as a debugging indicator
          Return "No such value " & FieldName

        Else
          ' got a field, return its value
          Return field.GetValue(obj).ToString
        End If

      Else
        ' found a property, return its value
        Return prop.GetValue(obj, Nothing).ToString
      End If

    Catch ex As Exception
      Return ex.Message
    End Try
  End Function

  Private Sub mItems_Invalidate() Handles mItems.Invalidate
    mFieldsChanged = True
    DataBind()
  End Sub

  <Category("Data")> _
  Public Property AutoDiscover() As Boolean
    Get
      Return mAutoDiscover
    End Get
    Set(ByVal Value As Boolean)
      Columns.Clear()
      mAutoDiscover = Value
    End Set
  End Property
End Class
