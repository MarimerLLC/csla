Imports System.Reflection

Namespace Data

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
    ''' <param name="supressExceptions">If True, any exceptions will be supressed.</param>
    ''' <remarks>
    ''' The key names in the dictionary must match the property names on the target
    ''' object. Target properties may not be readonly or indexed.
    ''' </remarks>
    Public Sub Map(ByVal source As System.Collections.IDictionary, ByVal target As Object, _
      ByVal supressExceptions As Boolean, ByVal ParamArray ignoreList() As String)

      Dim ignore As New List(Of String)(ignoreList)
      Dim targetType As Type = target.GetType
      For Each propertyName As String In source.Keys
        If Not ignore.Contains(propertyName) Then
          Try
            ' get source value
            Dim value As Object = source.Item(propertyName)
            ' set target value
            Dim propertyInfo As PropertyInfo = targetType.GetProperty(propertyName)
            Dim pType As Type = propertyInfo.PropertyType
            Dim vType As Type
            If value IsNot Nothing Then
              vType = value.GetType

            Else
              vType = pType
            End If
            value.GetType()
            If pType.Equals(vType) Then
              ' types match, just copy value
              propertyInfo.SetValue(target, value, Nothing)

            Else
              ' types don't match, try to coerce types
              If pType.Equals(GetType(Guid)) Then
                propertyInfo.SetValue(target, New Guid(value.ToString), Nothing)

              Else
                propertyInfo.SetValue(target, Convert.ChangeType(value, pType), Nothing)
              End If
            End If

          Catch
            If Not supressExceptions Then
              Throw
            End If
          End Try
        End If
      Next

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
    ''' <param name="supressExceptions">If True, any exceptions will be supressed.</param>
    ''' <remarks>
    ''' The property names and types of the source object must match the property names and types
    ''' on the target object. Source properties may not be indexed. 
    ''' Target properties may not be readonly or indexed.
    ''' </remarks>
    Public Sub Map(ByVal source As Object, ByVal target As Object, _
      ByVal supressExceptions As Boolean, ByVal ParamArray ignoreList() As String)

      Dim ignore As New List(Of String)(ignoreList)
      Dim sourceType As Type = source.GetType
      Dim sourceProperties As PropertyInfo() = sourceType.GetProperties
      Dim targetType As Type = target.GetType
      For Each sourceProperty As PropertyInfo In sourceProperties
        Dim propertyName As String = sourceProperty.Name
        If Not ignore.Contains(propertyName) Then
          Try
            Dim propertyInfo As PropertyInfo
            ' get source value
            propertyInfo = sourceType.GetProperty(propertyName)
            Dim value As Object = propertyInfo.GetValue(source, Nothing)
            ' set target value
            propertyInfo = targetType.GetProperty(propertyName)
            propertyInfo.SetValue(target, value, Nothing)

          Catch
            If Not supressExceptions Then
              Throw
            End If
          End Try
        End If
      Next

    End Sub

#End Region

  End Module

End Namespace
