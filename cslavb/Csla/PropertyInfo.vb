''' <summary>
''' Maintains metadata about a property.
''' </summary>
Public Class PropertyInfo(Of T)

  Implements Core.IPropertyInfo

  Friend Sub New(ByVal name As String, ByVal friendlyName As String)

    mName = name
    mFriendlyName = friendlyName
    If TypeOf mDefaultValue Is String Then
      mDefaultValue = DirectCast(DirectCast(String.Empty, Object), T)

    Else
      mDefaultValue = Nothing
    End If

  End Sub

  Friend Sub New(ByVal name As String, ByVal friendlyName As String, ByVal defaultValue As T)

    mName = name
    mDefaultValue = defaultValue
    mFriendlyName = friendlyName

  End Sub

  Private mName As String
  ''' <summary>
  ''' Gets the property name value.
  ''' </summary>
  Public ReadOnly Property Name() As String Implements Core.IPropertyInfo.Name
    Get
      Return mName
    End Get
  End Property

  Private mType As Type
  ''' <summary>
  ''' Gets the type of the property.
  ''' </summary>
  Public ReadOnly Property Type() As Type Implements Core.IPropertyInfo.Type
    Get
      Return GetType(T)
    End Get
  End Property

  Private mFriendlyName As String
  ''' <summary>
  ''' Gets the friendly display name
  ''' for the property.
  ''' </summary>
  ''' <remarks>
  ''' If no friendly name was provided, the
  ''' property name itself is returned as a
  ''' result.
  ''' </remarks>
  Public ReadOnly Property FriendlyName() As String Implements Core.IPropertyInfo.FriendlyName
    Get
      If Not String.IsNullOrEmpty(mFriendlyName) Then
        Return mFriendlyName

      Else
        Return mName
      End If
    End Get
  End Property

  Private mDefaultValue As T
  Public ReadOnly Property DefaultValue() As T
    Get
      Return mDefaultValue
    End Get
  End Property

End Class
