Imports System.Web.UI
Imports System.Web.UI.Design
Imports System.ComponentModel
Imports System.Reflection

Namespace Web.Design

  ''' <summary>
  ''' Contains schema information for a single
  ''' object property.
  ''' </summary>
  Public Class ObjectFieldInfo
    Implements System.Web.UI.Design.IDataSourceFieldSchema

    Private mField As PropertyDescriptor
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
      GetDataObjectAttribute()

    End Sub

    Private Sub GetDataObjectAttribute()

      Dim attribute As DataObjectFieldAttribute = _
          CType(mField.Attributes.Item(GetType(DataObjectFieldAttribute)), _
          DataObjectFieldAttribute)
      If (Not attribute Is Nothing) Then
        With attribute
          mPrimaryKey = .PrimaryKey
          mIsIdentity = .IsIdentity
          mIsNullable = .IsNullable
          mLength = .Length
        End With
      End If

    End Sub

    ''' <summary>
    ''' Gets the data type of the property.
    ''' </summary>
    Public ReadOnly Property DataType() As System.Type _
      Implements System.Web.UI.Design.IDataSourceFieldSchema.DataType
      Get
        Return Utilities.GetPropertyType(mField.PropertyType)
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
    ''' <returns>
    ''' Always returns True if the property
    ''' is marked as a primary key, otherwise
    ''' returns False.
    ''' </returns>
    Public ReadOnly Property IsUnique() As Boolean Implements System.Web.UI.Design.IDataSourceFieldSchema.IsUnique
      Get
        Return mPrimaryKey
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
    Public ReadOnly Property Name() As String _
      Implements System.Web.UI.Design.IDataSourceFieldSchema.Name
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
    Public ReadOnly Property Nullable() As Boolean _
      Implements System.Web.UI.Design.IDataSourceFieldSchema.Nullable
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
    Public ReadOnly Property PrimaryKey() As Boolean _
      Implements System.Web.UI.Design.IDataSourceFieldSchema.PrimaryKey
      Get
        Return mPrimaryKey
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