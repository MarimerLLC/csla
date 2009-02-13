Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports Csla.Reflection

Namespace Data
  ''' <summary>
  ''' Map data from a source into a target object
  ''' by copying public property values.
  ''' </summary>
  ''' <remarks></remarks>
  Public Module DataMapper

#Region " Map from IDictionary "

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
    Public Sub Map(ByVal source As System.Collections.IDictionary, ByVal target As Object, ByVal ParamArray ignoreList() As String)
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
    Public Sub Map(ByVal source As System.Collections.IDictionary, ByVal target As Object, ByVal suppressExceptions As Boolean, ByVal ParamArray ignoreList() As String)
      Dim ignore As List(Of String) = New List(Of String)(ignoreList)
      For Each propertyName As String In source.Keys
        If (Not ignore.Contains(propertyName)) Then
          Try
            SetPropertyValue(target, propertyName, source(propertyName))
          Catch ex As Exception
            If (Not suppressExceptions) Then
              Throw New ArgumentException(String.Format("{0} ({1})", My.Resources.PropertyCopyFailed, propertyName), ex)
            End If
          End Try
        End If
      Next propertyName
    End Sub

#End Region

#Region " Map to Dictionary "

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
      For Each propertyName In GetPropertyNames(source.GetType())
        If (Not ignore.Contains(propertyName)) Then
          Try
            target.Add(propertyName, MethodCaller.CallPropertyGetter(source, propertyName))
          Catch ex As Exception
            If (Not suppressExceptions) Then
              Throw New ArgumentException(String.Format("{0} ({1})", My.Resources.PropertyCopyFailed, propertyName), ex)
            End If
          End Try
        End If
      Next 'prop
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
    Public Sub Map(ByVal source As Object, ByVal target As Object, ByVal ParamArray ignoreList() As String)
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
    Public Sub Map(ByVal source As Object, ByVal target As Object, ByVal suppressExceptions As Boolean, ByVal ParamArray ignoreList() As String)
      Dim ignore As List(Of String) = New List(Of String)(ignoreList)
      For Each propertyName In GetPropertyNames(source.GetType())
        If (Not ignore.Contains(propertyName)) Then
          Try
            Dim value As Object = MethodCaller.CallPropertyGetter(source, propertyName)
            SetPropertyValue(target, propertyName, value)
          Catch ex As Exception
            If (Not suppressExceptions) Then
              Throw New ArgumentException(String.Format("{0} ({1})", My.Resources.PropertyCopyFailed, propertyName), ex)
            End If
          End Try
        End If
      Next 'sourceProperty

    End Sub

    ''' <summary>
    ''' Copies values from the source into the
    ''' properties of the target.
    ''' </summary>
    ''' <param name="source">An object containing the source values.</param>
    ''' <param name="target">An object with properties to be set from the dictionary.</param>
    ''' <param name="map">A DataMap object containing the mappings to use during the copy process.</param>
    ''' <remarks>
    ''' The property names and types of the source object must match the property names and types
    ''' on the target object. Source properties may not be indexed. 
    ''' Target properties may not be readonly or indexed.
    ''' </remarks>
    Public Sub Map(ByVal source As Object, ByVal target As Object, ByVal map As DataMap)
      DataMapper.Map(source, target, map, False)
    End Sub

    ''' <summary>
    ''' Copies values from the source into the
    ''' properties of the target.
    ''' </summary>
    ''' <param name="source">An object containing the source values.</param>
    ''' <param name="target">An object with properties to be set from the dictionary.</param>
    ''' <param name="suppressExceptions">If <see langword="true" />, any exceptions will be supressed.</param>
    ''' <param name="map">A DataMap object containing the mappings to use during the copy process.</param>
    ''' <remarks>
    ''' The property names and types of the source object must match the property names and types
    ''' on the target object. Source properties may not be indexed. 
    ''' Target properties may not be readonly or indexed.
    ''' </remarks>
    Public Sub Map(ByVal source As Object, ByVal target As Object, ByVal map As DataMap, ByVal suppressExceptions As Boolean)
      For Each mapping As DataMap.MemberMapping In map.GetMap()
        Try
          Dim value As Object = mapping.FromMemberHandle.DynamicMemberGet(source)
          SetValueWithCoercion(target, mapping.ToMemberHandle, value)
        Catch ex As Exception
          If (Not suppressExceptions) Then
            Throw New ArgumentException(String.Format("{0} ({1})", My.Resources.PropertyCopyFailed, mapping.FromMemberHandle.MemberName), ex)
          End If
        End Try
      Next mapping
    End Sub

    Private Function GetPropertyNames(ByVal sourceType As Type) As IList(Of String)
      Dim result As List(Of String) = New List(Of String)
      Dim props As PropertyDescriptorCollection = TypeDescriptor.GetProperties(sourceType)
      For Each item As PropertyDescriptor In props
        If item.IsBrowsable Then
          result.Add(item.Name)
        End If
      Next

      Return result
    End Function

#End Region

#Region " Load from IDictionary"

    ''' <summary>
    ''' Copies values from the source into the
    ''' target.
    ''' </summary>
    ''' <param name="source">
    ''' Dictionary containing the source values.
    ''' </param>
    ''' <param name="target">
    ''' Business object with managed fields that
    ''' will contain the copied values.
    ''' </param>
    ''' <param name="nameMapper">
    ''' A function that translates the target
    ''' property names into key values for the
    ''' source dictionary.
    ''' </param>
    Public Sub Load(ByVal source As System.Collections.IDictionary, ByVal target As Object, ByVal nameMapper As Func(Of String, Object))
      Dim validTarget = TryCast(target, Core.IManageProperties)
      If validTarget Is Nothing Then
        Throw New NotSupportedException()
      End If
      Dim propertyList = validTarget.GetManagedProperties()
      For Each p In propertyList
        validTarget.LoadProperty(p, source(nameMapper(p.Name)))
      Next p
    End Sub

#End Region

#Region " Load to IDictionary"

    ''' <summary>
    ''' Copies values from the source into the
    ''' target.
    ''' </summary>
    ''' <param name="source">
    ''' Business object with managed fields that
    ''' contain the source values.
    ''' </param>
    ''' <param name="target">
    ''' Dictionary that will contain the resulting values.
    ''' </param>
    ''' <param name="nameMapper">
    ''' A function that translates the source
    ''' property names into key values for the
    ''' target dictionary.
    ''' </param>
    Public Sub Load(ByVal source As Object, ByVal target As System.Collections.IDictionary, ByVal nameMapper As Func(Of String, Object))
      Dim validSource = TryCast(source, Core.IManageProperties)
      If validSource Is Nothing Then
        Throw New NotSupportedException()
      End If
      Dim propertyList = validSource.GetManagedProperties()
      For Each p In propertyList
        target(nameMapper(p.Name)) = validSource.ReadProperty(p)
      Next p
    End Sub

#End Region

#Region " SetValue "

    ''' <summary>
    ''' Sets an object's property with the specified value,
    ''' coercing that value to the appropriate type if possible.
    ''' </summary>
    ''' <param name="target">Object containing the property to set.</param>
    ''' <param name="propertyName">Name of the property to set.</param>
    ''' <param name="value">Value to set into the property.</param>
    Public Sub SetPropertyValue(ByVal target As Object, ByVal propertyName As String, ByVal value As Object)
      Dim handle As DynamicMemberHandle = MethodCaller.GetCachedProperty(target.GetType(), propertyName)
      SetValueWithCoercion(target, handle, value)
    End Sub


    Private Sub SetValueWithCoercion(ByVal target As Object, ByVal handle As DynamicMemberHandle, ByVal value As Object)
      Dim oldValue = handle.DynamicMemberGet(target)

      Dim pType As Type = handle.MemberType

      If Not pType.IsGenericType _
              Or (pType.IsGenericType AndAlso pType.GetGenericTypeDefinition() IsNot GetType(Nullable())) Then
        If (pType.IsValueType AndAlso (pType.IsPrimitive Or pType Is GetType(Decimal)) AndAlso value Is Nothing) Then
          value = 0
        End If
      End If

      If value IsNot Nothing Then
        Dim vType As Type = Utilities.GetPropertyType(value.GetType())
        value = Utilities.CoerceValue(pType, vType, oldValue, value)
      End If

      handle.DynamicMemberSet.Invoke(target, value)

    End Sub

    ''' <summary>
    ''' Sets an object's field with the specified value,
    ''' coercing that value to the appropriate type if possible.
    ''' </summary>
    ''' <param name="target">Object containing the field to set.</param>
    ''' <param name="fieldName">Name of the field (public or non-public) to set.</param>
    ''' <param name="value">Value to set into the field.</param>
    Public Sub SetFieldValue(ByVal target As Object, ByVal fieldName As String, ByVal value As Object)
      Dim handle As DynamicMemberHandle = MethodCaller.GetCachedField(target.GetType(), fieldName)
      SetValueWithCoercion(target, handle, value)
    End Sub

#End Region

  End Module

End Namespace
