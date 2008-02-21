Imports System.Web.UI
Imports System.Web.UI.Design
Imports System.ComponentModel
Imports System.Reflection

Namespace Web.Design

  ''' <summary>
  ''' Contains schema information for a single
  ''' object property.
  ''' </summary>
  <Serializable()> _
  Public Class ObjectFieldInfo

    Implements System.Web.UI.Design.IDataSourceFieldSchema

    Private _dataType As Type
    Private _primaryKey As Boolean
    Private _isIdentity As Boolean
    Private _isNullable As Boolean
    Private _length As Integer
    Private _isReadOnly As Boolean
    Private _name As String
    Private _nullable As Boolean

    ''' <summary>
    ''' Creates an instance of the object.
    ''' </summary>
    ''' <param name="field">The PropertyInfo object
    ''' describing the property.</param>
    Public Sub New(ByVal field As PropertyDescriptor)

      Dim attribute As DataObjectFieldAttribute = _
          CType(field.Attributes.Item(GetType(DataObjectFieldAttribute)),  _
          DataObjectFieldAttribute)
      If (Not attribute Is Nothing) Then
        With attribute
          _primaryKey = .PrimaryKey
          _isIdentity = .IsIdentity
          _isNullable = .IsNullable
          _length = .Length
        End With
      End If
      _dataType = Utilities.GetPropertyType(field.PropertyType)
      _isReadOnly = field.IsReadOnly
      _name = field.Name

      Dim t As Type = field.PropertyType
      If Not t.IsValueType OrElse _isNullable Then
        _nullable = True

      ElseIf t.IsGenericType Then
        _nullable = (t.GetGenericTypeDefinition Is GetType(Nullable(Of )))

      Else
        _nullable = False
      End If

    End Sub

    ''' <summary>
    ''' Gets the data type of the property.
    ''' </summary>
    Public ReadOnly Property DataType() As System.Type _
      Implements System.Web.UI.Design.IDataSourceFieldSchema.DataType
      Get
        Return _dataType
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
        Return _isIdentity
      End Get
    End Property

    ''' <summary>
    ''' Gets a value indicating whether this property
    ''' is readonly.
    ''' </summary>
    Public ReadOnly Property IsReadOnly() As Boolean Implements System.Web.UI.Design.IDataSourceFieldSchema.IsReadOnly
      Get
        Return _isReadOnly
      End Get
    End Property

    ''' <summary>
    ''' Gets a value indicating whether this property
    ''' must contain a unique value.
    ''' </summary>
    ''' <returns>
    ''' Always returns True if the property
    ''' is marked as a primary key, otherwise
    ''' returns False.
    ''' </returns>
    Public ReadOnly Property IsUnique() As Boolean Implements System.Web.UI.Design.IDataSourceFieldSchema.IsUnique
      Get
        Return _primaryKey
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
        Return _length
      End Get
    End Property

    ''' <summary>
    ''' Gets the property name.
    ''' </summary>
    Public ReadOnly Property Name() As String _
      Implements System.Web.UI.Design.IDataSourceFieldSchema.Name
      Get
        Return _name
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
    Public ReadOnly Property Nullable() As Boolean _
      Implements System.Web.UI.Design.IDataSourceFieldSchema.Nullable
      Get
        Return _nullable
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
    Public ReadOnly Property PrimaryKey() As Boolean _
      Implements System.Web.UI.Design.IDataSourceFieldSchema.PrimaryKey
      Get
        Return _primaryKey
      End Get
    End Property

    ''' <summary>
    ''' Gets the property's scale.
    ''' </summary>
    ''' <returns>Always returns -1.</returns>
    Public ReadOnly Property Scale() As Integer _
      Implements System.Web.UI.Design.IDataSourceFieldSchema.Scale
      Get
        Return -1
      End Get
    End Property

  End Class

End Namespace