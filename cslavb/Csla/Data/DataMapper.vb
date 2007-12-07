Imports System.Reflection
Imports System.ComponentModel

Namespace Data

  ''' <summary>
  ''' Map data from a source into a target object
  ''' by copying public property values.
  ''' </summary>
  ''' <remarks></remarks>
  Public Module DataMapper

#Region " Map From IDictionary "

    ''' <summary>
    ''' Copies values from the source into the
    ''' properties of the target.
    ''' </summary>
    ''' <param name="source">A name/value dictionary containing the source values.</param>
    ''' <param name="target">An object with properties to be set from the dictionary.</param>
    ''' <remarks>
    ''' The key names in the dictionary must match the property names on the target
    ''' object. Target properties may not be readonly or indexed.
    ''' </remarks>
    Public Sub Map(ByVal source As System.Collections.IDictionary, ByVal target As Object)

      Map(source, target, False)

    End Sub

    ''' <summary>
    ''' Copies values from the source into the
    ''' properties of the target.
    ''' </summary>
    ''' <param name="source">A name/value dictionary containing the source values.</param>
    ''' <param name="target">An object with properties to be set from the dictionary.</param>
    ''' <param name="ignoreList">A list of property names to ignore. 
    ''' These properties will not be set on the target object.</param>
    ''' <remarks>
    ''' The key names in the dictionary must match the property names on the target
    ''' object. Target properties may not be readonly or indexed.
    ''' </remarks>
    Public Sub Map(ByVal source As System.Collections.IDictionary, ByVal target As Object, _
      ByVal ParamArray ignoreList() As String)

      Map(source, target, False, ignoreList)

    End Sub

    ''' <summary>
    ''' Copies values from the source into the
    ''' properties of the target.
    ''' </summary>
    ''' <param name="source">A name/value dictionary containing the source values.</param>
    ''' <param name="target">An object with properties to be set from the dictionary.</param>
    ''' <param name="ignoreList">A list of property names to ignore. 
    ''' These properties will not be set on the target object.</param>
    ''' <param name="suppressExceptions">If <see langword="true" />, any exceptions will be supressed.</param>
    ''' <remarks>
    ''' The key names in the dictionary must match the property names on the target
    ''' object. Target properties may not be readonly or indexed.
    ''' </remarks>
    Public Sub Map( _
      ByVal source As System.Collections.IDictionary, _
      ByVal target As Object, _
      ByVal suppressExceptions As Boolean, _
      ByVal ParamArray ignoreList() As String)

      Dim ignore As New List(Of String)(ignoreList)
      For Each propertyName As String In source.Keys
        If Not ignore.Contains(propertyName) Then
          Try
            SetPropertyValue(target, propertyName, source.Item(propertyName))

          Catch ex As Exception
            If Not suppressExceptions Then
              Throw New ArgumentException( _
                String.Format("{0} ({1})", _
                My.Resources.PropertyCopyFailed, propertyName), ex)
            End If
          End Try
        End If
      Next

    End Sub

#End Region

#Region "Map to Dictionary"

    ''' <summary>
    ''' Copies values from the source into the target.
    ''' </summary>
    ''' <param name="source">An object with properties to be loaded into the dictionary.</param>
    ''' <param name="target">A name/value dictionary containing the source values.</param>
    Public Sub Map(ByVal source As Object, ByVal target As Dictionary(Of String, Object))
      Map(source, target, False)
    End Sub

    ''' <summary>
    ''' Copies values from the source into the target.
    ''' </summary>
    ''' <param name="source">An object with properties to be loaded into the dictionary.</param>
    ''' <param name="target">A name/value dictionary containing the source values.</param>
    ''' <param name="ignoreList">A list of property names to ignore. 
    ''' These properties will not be set on the target object.</param>
    Public Sub Map(ByVal source As Object, ByVal target As Dictionary(Of String, Object), ByVal ParamArray ignoreList() As String)
      Map(source, target, False, ignoreList)
    End Sub

    ''' <summary>
    ''' Copies values from the source into the target.
    ''' </summary>
    ''' <param name="source">An object with properties to be loaded into the dictionary.</param>
    ''' <param name="target">A name/value dictionary containing the source values.</param>
    ''' <param name="ignoreList">A list of property names to ignore. 
    ''' These properties will not be set on the target object.</param>
    ''' <param name="suppressExceptions">If <see langword="true" />, any exceptions will be supressed.</param>
    Public Sub Map(ByVal source As Object, ByVal target As Dictionary(Of String, Object), ByVal suppressExceptions As Boolean, ByVal ParamArray ignoreList() As String)
      Dim ignore As List(Of String) = New List(Of String)(ignoreList)
      For Each prop As PropertyInfo In Reflection.TypeInfoCache.GetPropertyInfo(source.GetType())
        Dim propertyName As String = prop.Name
        If (Not ignore.Contains(propertyName)) Then
          Try
            target.Add(propertyName, prop.GetValue(source, Nothing))
          Catch ex As Exception
            If (Not suppressExceptions) Then
              Throw New ArgumentException(String.Format("{0} ({1})", My.Resources.PropertyCopyFailed, propertyName), ex)
            End If
          End Try
        End If
      Next prop
    End Sub

#End Region

#Region " Map from Object "

    ''' <summary>
    ''' Copies values from the source into the
    ''' properties of the target.
    ''' </summary>
    ''' <param name="source">An object containing the source values.</param>
    ''' <param name="target">An object with properties to be set from the dictionary.</param>
    ''' <remarks>
    ''' The property names and types of the source object must match the property names and types
    ''' on the target object. Source properties may not be indexed. 
    ''' Target properties may not be readonly or indexed.
    ''' </remarks>
    Public Sub Map(ByVal source As Object, ByVal target As Object)

      Map(source, target, False)

    End Sub

    ''' <summary>
    ''' Copies values from the source into the
    ''' properties of the target.
    ''' </summary>
    ''' <param name="source">An object containing the source values.</param>
    ''' <param name="target">An object with properties to be set from the dictionary.</param>
    ''' <param name="ignoreList">A list of property names to ignore. 
    ''' These properties will not be set on the target object.</param>
    ''' <remarks>
    ''' The property names and types of the source object must match the property names and types
    ''' on the target object. Source properties may not be indexed. 
    ''' Target properties may not be readonly or indexed.
    ''' </remarks>
    Public Sub Map(ByVal source As Object, ByVal target As Object, _
      ByVal ParamArray ignoreList() As String)

      Map(source, target, False, ignoreList)

    End Sub

    ''' <summary>
    ''' Copies values from the source into the
    ''' properties of the target.
    ''' </summary>
    ''' <param name="source">An object containing the source values.</param>
    ''' <param name="target">An object with properties to be set from the dictionary.</param>
    ''' <param name="ignoreList">A list of property names to ignore. 
    ''' These properties will not be set on the target object.</param>
    ''' <param name="suppressExceptions">If <see langword="true" />, any exceptions will be supressed.</param>
    ''' <remarks>
    ''' <para>
    ''' The property names and types of the source object must match the property names and types
    ''' on the target object. Source properties may not be indexed. 
    ''' Target properties may not be readonly or indexed.
    ''' </para><para>
    ''' Properties to copy are determined based on the source object. Any properties
    ''' on the source object marked with the <see cref="BrowsableAttribute"/> equal
    ''' to false are ignored.
    ''' </para>
    ''' </remarks>
    Public Sub Map( _
      ByVal source As Object, _
      ByVal target As Object, _
      ByVal suppressExceptions As Boolean, _
      ByVal ParamArray ignoreList() As String)

      Dim ignore As New List(Of String)(ignoreList)
      Dim sourceProperties As PropertyInfo() = _
        GetSourceProperties(source.GetType)
      For Each sourceProperty As PropertyInfo In sourceProperties
        Dim propertyName As String = sourceProperty.Name
        If Not ignore.Contains(propertyName) Then
          Try
            SetPropertyValue(target, propertyName, _
              sourceProperty.GetValue(source, Nothing))

          Catch ex As Exception
            If Not suppressExceptions Then
              Throw New ArgumentException( _
                String.Format("{0} ({1})", _
                My.Resources.PropertyCopyFailed, propertyName), ex)
            End If
          End Try
        End If
      Next

    End Sub

    Private Function GetSourceProperties( _
      ByVal sourceType As Type) As PropertyInfo()

      Dim result As New Generic.List(Of PropertyInfo)
      Dim props As PropertyDescriptorCollection = _
        TypeDescriptor.GetProperties(sourceType)
      For Each item As PropertyDescriptor In props
        If item.IsBrowsable Then
          result.Add(sourceType.GetProperty(item.Name))
        End If
      Next
      Return result.ToArray

    End Function

#End Region

#Region "GetValue"

    Private Function GetValue(ByVal member As MemberInfo, ByVal source As Object) As Object
      If member.MemberType = MemberTypes.Property Then
        Return (CType(member, PropertyInfo)).GetValue(source, Nothing)
      Else
        Return (CType(member, FieldInfo)).GetValue(source)
      End If
    End Function

#End Region

#Region "SetValue"

    ''' <summary>
    ''' Sets an object's property with the specified value,
    ''' coercing that value to the appropriate type if possible.
    ''' </summary>
    ''' <param name="target">Object containing the property to set.</param>
    ''' <param name="propertyName">Name of the property to set.</param>
    ''' <param name="value">Value to set into the property.</param>
    Public Sub SetPropertyValue(ByVal target As Object, ByVal propertyName As String, ByVal value As Object)
      Dim propertyInfo As PropertyInfo = target.GetType().GetProperty(propertyName, BindingFlags.Public Or BindingFlags.Instance Or BindingFlags.FlattenHierarchy)
      SetValue(target, propertyInfo, value)
    End Sub

    ''' <summary>
    ''' Sets an object's field with the specified value,
    ''' coercing that value to the appropriate type if possible.
    ''' </summary>
    ''' <param name="target">Object containing the field to set.</param>
    ''' <param name="fieldName">Name of the field (public or non-public) to set.</param>
    ''' <param name="value">Value to set into the field.</param>
    Public Sub SetFieldValue(ByVal target As Object, ByVal fieldName As String, ByVal value As Object)
      Dim fieldInfo As FieldInfo = target.GetType().GetField(fieldName, BindingFlags.Public Or BindingFlags.NonPublic Or BindingFlags.Instance)
      SetValue(target, fieldInfo, value)
    End Sub

    ''' <summary>
    ''' Sets an object's property or field with the specified value,
    ''' coercing that value to the appropriate type if possible.
    ''' </summary>
    ''' <param name="target">Object containing the member to set.</param>
    ''' <param name="memberInfo">MemberInfo object for the member to set.</param>
    ''' <param name="value">Value to set into the member.</param>
    Public Sub SetValue(ByVal target As Object, ByVal memberInfo As MemberInfo, ByVal value As Object)
      If value IsNot Nothing Then
        Dim pType As Type
        If memberInfo.MemberType = MemberTypes.Property Then
          pType = (CType(memberInfo, PropertyInfo)).PropertyType
        Else
          pType = (CType(memberInfo, FieldInfo)).FieldType
        End If
        Dim vType As Type = Utilities.GetPropertyType(value.GetType())
        value = CoerceValue(pType, vType, value)
      End If
      If memberInfo.MemberType = MemberTypes.Property Then
        CType(memberInfo, PropertyInfo).SetValue(target, value, Nothing)
      Else
        CType(memberInfo, FieldInfo).SetValue(target, value)
      End If
    End Sub

    Private Function CoerceValue(ByVal propertyType As Type, ByVal valueType As Type, ByVal value As Object) As Object
      If propertyType.Equals(valueType) Then
        ' types match, just return value
        Return value
      Else
        If propertyType.IsGenericType Then
          If propertyType.GetGenericTypeDefinition() Is GetType(Nullable(Of )) Then
            If value Is Nothing Then
              Return Nothing
            ElseIf valueType.Equals(GetType(String)) AndAlso CStr(value) = String.Empty Then
              Return Nothing
            End If
          End If
          propertyType = Utilities.GetPropertyType(propertyType)
        End If

        If propertyType.IsEnum AndAlso valueType.Equals(GetType(String)) Then
          Return System.Enum.Parse(propertyType, value.ToString())
        End If

        If propertyType.IsPrimitive AndAlso valueType.Equals(GetType(String)) AndAlso String.IsNullOrEmpty(CStr(value)) Then
          value = 0
        End If

        Try
          Return Convert.ChangeType(value, Utilities.GetPropertyType(propertyType))
        Catch
          Dim cnv As TypeConverter = TypeDescriptor.GetConverter(Utilities.GetPropertyType(propertyType))
          If cnv IsNot Nothing AndAlso cnv.CanConvertFrom(value.GetType()) Then
            Return cnv.ConvertFrom(value)
          Else
            Throw
          End If
        End Try
      End If
    End Function

#End Region

  End Module

End Namespace
