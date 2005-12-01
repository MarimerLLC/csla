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
          ElseIf col.DataType.Equals(GetType(Boolean)) Then
            values(colIndex) = False
          ElseIf col.DataType.IsPrimitive Then
            values(colIndex) = index
          Else
            values(colIndex) = ""
          End If
          colIndex += 1
        Next
        result.LoadDataRow(values, LoadOption.OverwriteChanges)
      Next

      isSampleData = True
      Return CType(result, IEnumerable)

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
        'Return New TypeSchema(CslaDataSource.GetType( _
        '  mOwner.DataSourceControl.TypeAssemblyName, mOwner.DataSourceControl.TypeName)).GetViews(0)
        Return New ObjectSchema(mOwner.DataSourceControl.TypeAssemblyName, _
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
        If GetType(Csla.Core.IEditableObject).IsAssignableFrom(objectType) Then
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
        If GetType(Csla.Core.IEditableObject).IsAssignableFrom(CslaDataSource.GetType( _
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
        If GetType(Csla.Core.IEditableObject).IsAssignableFrom(CslaDataSource.GetType( _
            mOwner.DataSourceControl.TypeAssemblyName, mOwner.DataSourceControl.TypeName)) Then
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

  ''' <summary>
  ''' Object providing access to schema information for
  ''' a business object.
  ''' </summary>
  ''' <remarks>
  ''' This object returns only one view, which corresponds
  ''' to the business object used by data binding.
  ''' </remarks>
  Public Class ObjectSchema
    Implements IDataSourceSchema

    Private mTypeAssemblyName As String = ""
    Private mTypeName As String = ""

    Public Sub New(ByVal assemblyName As String, ByVal typeName As String)

      mTypeAssemblyName = assemblyName
      mTypeName = typeName

    End Sub

    ''' <summary>
    ''' Returns a single element array containing the
    ''' schema for the CSLA .NET business object.
    ''' </summary>
    Public Function GetViews() As System.Web.UI.Design.IDataSourceViewSchema() Implements System.Web.UI.Design.IDataSourceSchema.GetViews
      Return New IDataSourceViewSchema() {New ObjectViewSchema(mTypeAssemblyName, mTypeName)}
    End Function

  End Class

  ''' <summary>
  ''' Object providing schema information for a
  ''' business object.
  ''' </summary>
  Public Class ObjectViewSchema
    Implements IDataSourceViewSchema

    Private mTypeAssemblyName As String = ""
    Private mTypeName As String = ""

    ''' <summary>
    ''' Create an instance of the object.
    ''' </summary>
    ''' <param name="assemblyName">The assembly containing
    ''' the business class for which to generate the schema.</param>
    ''' <param name="typeName">The business class for
    ''' which to generate the schema.</param>
    Public Sub New(ByVal assemblyName As String, ByVal typeName As String)

      mTypeAssemblyName = assemblyName
      mTypeName = typeName

    End Sub

    ''' <summary>
    ''' Returns a list of child schemas belonging to the
    ''' object.
    ''' </summary>
    ''' <remarks>This schema object only returns
    ''' schema for the object itself, so GetChildren will
    ''' always return Nothing (null in C#).</remarks>
    Public Function GetChildren() As System.Web.UI.Design.IDataSourceViewSchema() Implements System.Web.UI.Design.IDataSourceViewSchema.GetChildren
      Return Nothing
    End Function

    ''' <summary>
    ''' Returns schema information for each property on
    ''' the object.
    ''' </summary>
    ''' <remarks>All public properties on the object
    ''' will be reflected in this schema list except
    ''' for those properties where the 
    ''' <see cref="BrowsableAttribute">Browsable</see> attribute
    ''' is False.
    ''' </remarks>
    Public Function GetFields() As System.Web.UI.Design.IDataSourceFieldSchema() Implements System.Web.UI.Design.IDataSourceViewSchema.GetFields

      Dim result As New Generic.List(Of ObjectFieldInfo)
      Dim t As Type = CslaDataSource.GetType(mTypeAssemblyName, mTypeName)
      If GetType(IEnumerable).IsAssignableFrom(t) Then
        ' this is a list so get the item type
        t = GetItemType(t)
      End If
      Dim props As PropertyDescriptorCollection = TypeDescriptor.GetProperties(t)
      For Each item As PropertyDescriptor In props
        If item.IsBrowsable Then
          result.Add(New ObjectFieldInfo(item))
        End If
      Next
      Return result.ToArray

    End Function

    Private Function GetItemType(ByVal objectType As Type) As Type

      If objectType.IsArray Then
        Return objectType.GetElementType
      End If
      Dim props As PropertyInfo() = _
        objectType.GetProperties(BindingFlags.FlattenHierarchy Or BindingFlags.Public Or BindingFlags.Instance)
      For Each item As PropertyInfo In props
        If (item.GetIndexParameters.Length > 0) Then
          Return item.PropertyType
        End If
      Next
      Return Nothing

    End Function

    ''' <summary>
    ''' Returns the name of the schema.
    ''' </summary>
    Public ReadOnly Property Name() As String Implements System.Web.UI.Design.IDataSourceViewSchema.Name
      Get
        Return "Default"
      End Get
    End Property

  End Class

  ''' <summary>
  ''' Contains schema information for a single
  ''' object property.
  ''' </summary>
  Public Class ObjectFieldInfo
    Implements System.Web.UI.Design.IDataSourceFieldSchema

    Private mField As PropertyDescriptor
    Private mRetrievedMetaData As Boolean
    Private mPrimaryKey As Boolean
    Private mIsIdentity As Boolean
    Private mIsNullable As Boolean
    Private mLength As Integer

    ''' <summary>
    ''' Creates an instance of the object.
    ''' </summary>
    ''' <param name="field">The PropertyInfo object
    ''' describing the property.</param>
    Public Sub New(ByVal field As PropertyDescriptor)

      mField = field

    End Sub

    Private Sub EnsureMetaData()

      If Not mRetrievedMetaData Then
        Dim attribute1 As DataObjectFieldAttribute = _
          CType(mField.Attributes.Item(GetType(DataObjectFieldAttribute)), DataObjectFieldAttribute)
        If (Not attribute1 Is Nothing) Then
          mPrimaryKey = attribute1.PrimaryKey
          mIsIdentity = attribute1.IsIdentity
          mIsNullable = attribute1.IsNullable
          mLength = attribute1.Length
        End If
        mRetrievedMetaData = True
      End If

    End Sub

    ''' <summary>
    ''' Gets the data type of the property.
    ''' </summary>
    Public ReadOnly Property DataType() As System.Type Implements System.Web.UI.Design.IDataSourceFieldSchema.DataType
      Get
        Dim type1 As Type = Me.mField.PropertyType
        If (type1.IsGenericType AndAlso (type1.GetGenericTypeDefinition Is GetType(Nullable))) Then
          Return type1.GetGenericArguments(0)
        End If
        Return type1
      End Get
    End Property

    ''' <summary>
    ''' Gets a value indicating whether this property
    ''' is an identity key for the object.
    ''' </summary>
    ''' <remarks>
    ''' Returns the optional value provided through
    ''' the <see cref="DataObjectFieldAttribute">DataObjectField</see>
    ''' attribute on the property.
    ''' </remarks>
    Public ReadOnly Property Identity() As Boolean Implements System.Web.UI.Design.IDataSourceFieldSchema.Identity
      Get
        Return mIsIdentity
      End Get
    End Property

    ''' <summary>
    ''' Gets a value indicating whether this property
    ''' is readonly.
    ''' </summary>
    Public ReadOnly Property IsReadOnly() As Boolean Implements System.Web.UI.Design.IDataSourceFieldSchema.IsReadOnly
      Get
        Return mField.IsReadOnly
      End Get
    End Property

    ''' <summary>
    ''' Gets a value indicating whether this property
    ''' must contain a unique value.
    ''' </summary>
    ''' <returns>Always returns False.</returns>
    Public ReadOnly Property IsUnique() As Boolean Implements System.Web.UI.Design.IDataSourceFieldSchema.IsUnique
      Get
        Return False
      End Get
    End Property

    ''' <summary>
    ''' Gets the length of the property value.
    ''' </summary>
    ''' <remarks>
    ''' Returns the optional value provided through
    ''' the <see cref="DataObjectFieldAttribute">DataObjectField</see>
    ''' attribute on the property.
    ''' </remarks>
    Public ReadOnly Property Length() As Integer Implements System.Web.UI.Design.IDataSourceFieldSchema.Length
      Get
        Return mLength
      End Get
    End Property

    ''' <summary>
    ''' Gets the property name.
    ''' </summary>
    Public ReadOnly Property Name() As String Implements System.Web.UI.Design.IDataSourceFieldSchema.Name
      Get
        Return mField.Name
      End Get
    End Property

    ''' <summary>
    ''' Gets a value indicating whether the property
    ''' is nullable
    ''' </summary>
    ''' <remarks>
    ''' Returns True for reference types, and for
    ''' value types wrapped in the Nullable generic.
    ''' The result can also be set to True through
    ''' the <see cref="DataObjectFieldAttribute">DataObjectField</see>
    ''' attribute on the property.
    ''' </remarks>
    Public ReadOnly Property Nullable() As Boolean Implements System.Web.UI.Design.IDataSourceFieldSchema.Nullable
      Get
        Dim t As Type = Me.mField.PropertyType
        If Not t.IsValueType OrElse mIsNullable Then
          Return True
        End If
        If t.IsGenericType Then
          Return (t.GetGenericTypeDefinition Is GetType(Nullable))
        End If
        Return False
      End Get
    End Property

    ''' <summary>
    ''' Gets the property's numeric precision.
    ''' </summary>
    ''' <returns>Always returns -1.</returns>
    Public ReadOnly Property Precision() As Integer Implements System.Web.UI.Design.IDataSourceFieldSchema.Precision
      Get
        Return -1
      End Get
    End Property

    ''' <summary>
    ''' Gets a value indicating whether the property
    ''' is a primary key value.
    ''' </summary>
    ''' <remarks>
    ''' Returns the optional value provided through
    ''' the <see cref="DataObjectFieldAttribute">DataObjectField</see>
    ''' attribute on the property.
    ''' </remarks>
    Public ReadOnly Property PrimaryKey() As Boolean Implements System.Web.UI.Design.IDataSourceFieldSchema.PrimaryKey
      Get
        Return mPrimaryKey
      End Get
    End Property

    ''' <summary>
    ''' Gets the property's scale.
    ''' </summary>
    ''' <returns>Always returns -1.</returns>
    Public ReadOnly Property Scale() As Integer Implements System.Web.UI.Design.IDataSourceFieldSchema.Scale
      Get
        Return -1
      End Get
    End Property

  End Class

End Namespace
