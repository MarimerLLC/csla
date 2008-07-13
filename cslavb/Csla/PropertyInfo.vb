''' <summary>
''' Maintains metadata about a property.
''' </summary>
''' <typeparam name="T">
''' Data type of the property.
''' </typeparam>
Public Class PropertyInfo(Of T)

  Implements Core.IPropertyInfo
  Implements IComparable

  ''' <summary>
  ''' Creates a new instance of this class.
  ''' </summary>
  ''' <param name="name">Name of the property.</param>
  Public Sub New(ByVal name As String)

    Me.New(name, "")

  End Sub

  ''' <summary>
  ''' Creates a new instance of this class.
  ''' </summary>
  ''' <param name="name">Name of the property.</param>
  ''' <param name="friendlyName">
  ''' Friendly display name for the property.
  ''' </param>
  Public Sub New(ByVal name As String, ByVal friendlyName As String)

    _name = name
    _friendlyName = friendlyName
    If GetType(T).Equals(GetType(String)) Then
      _defaultValue = DirectCast(DirectCast(String.Empty, Object), T)

    Else
      _defaultValue = Nothing
    End If

  End Sub

  ''' <summary>
  ''' Creates a new instance of this class.
  ''' </summary>
  ''' <param name="name">Name of the property.</param>
  ''' <param name="friendlyName">
  ''' Friendly display name for the property.
  ''' </param>
  ''' <param name="defaultValue">
  ''' Default value for the property.
  ''' </param>
  Public Sub New(ByVal name As String, ByVal friendlyName As String, ByVal defaultValue As T)

    _name = name
    _defaultValue = defaultValue
    _friendlyName = friendlyName

  End Sub

  Private _name As String
  ''' <summary>
  ''' Gets the property name value.
  ''' </summary>
  Public ReadOnly Property Name() As String Implements Core.IPropertyInfo.Name
    Get
      Return _name
    End Get
  End Property

  ''' <summary>
  ''' Gets the type of the property.
  ''' </summary>
  Public ReadOnly Property Type() As Type Implements Core.IPropertyInfo.Type
    Get
      Return GetType(T)
    End Get
  End Property

  Private _friendlyName As String
  ''' <summary>
  ''' Gets the friendly display name
  ''' for the property.
  ''' </summary>
  ''' <remarks>
  ''' If no friendly name was provided, the
  ''' property name itself is returned as a
  ''' result.
  ''' </remarks>
  Public Overridable ReadOnly Property FriendlyName() As String Implements Core.IPropertyInfo.FriendlyName
    Get
      If Not String.IsNullOrEmpty(_friendlyName) Then
        Return _friendlyName

      Else
        Return _name
      End If
    End Get
  End Property

  Private _defaultValue As T
  ''' <summary>
  ''' Gets the default initial value for the property.
  ''' </summary>
  ''' <remarks>
  ''' This value is used to initialize the property's
  ''' value, and is returned from a property get
  ''' if the user is not authorized to 
  ''' read the property.
  ''' </remarks>
  Public Overridable ReadOnly Property DefaultValue() As T
    Get
      Return _defaultValue
    End Get
  End Property

  Private ReadOnly Property IPropertyInfo_DefaultValue() As Object Implements Core.IPropertyInfo.DefaultValue
    Get
      Return _defaultValue
    End Get
  End Property

  ''' <summary>
  ''' Create and return a new IFieldData object
  ''' to store an instance value for this
  ''' property.
  ''' </summary>
  ''' <param name="name">
  ''' Property name.
  ''' </param>
  Protected Overridable Function NewFieldData(ByVal name As String) As Core.FieldManager.IFieldData Implements Core.IPropertyInfo.NewFieldData
    Return New Core.FieldManager.FieldData(Of T)(name)
  End Function

  Private _index As Integer = -1

  ''' <summary>
  ''' Gets or sets the index position for the managed
  ''' field storage behind the property. FOR
  ''' INTERNAL CSLA .NET USE ONLY.
  ''' </summary>
  Public Property Index() As Integer Implements Core.IPropertyInfo.Index
    Get
      Return _index
    End Get
    Set(ByVal value As Integer)
      _index = value
    End Set
  End Property

  Private Function CompareTo(ByVal obj As Object) As Integer Implements System.IComparable.CompareTo

    Return _name.CompareTo(DirectCast(obj, Core.IPropertyInfo).Name)

  End Function

End Class
