Namespace Core.FieldManager

  Friend Module PropertyInfoManager

    Private _cacheLock As New Object
    Private _propertyInfoCache As Dictionary(Of Type, List(Of IPropertyInfo))

    Private ReadOnly Property PropertyInfoCache() As Dictionary(Of Type, List(Of IPropertyInfo))
      Get
        If _propertyInfoCache Is Nothing Then
          SyncLock _cacheLock
            If _propertyInfoCache Is Nothing Then
              _propertyInfoCache = New Dictionary(Of Type, List(Of IPropertyInfo))()
            End If
          End SyncLock
        End If
        Return _propertyInfoCache
      End Get
    End Property

    Public Function GetPropertyListCache(ByVal objectType As Type) As List(Of IPropertyInfo)
      Dim cache = PropertyInfoCache
      Dim list As List(Of IPropertyInfo) = Nothing
      If Not (cache.TryGetValue(objectType, list)) Then
        SyncLock cache
          If Not (cache.TryGetValue(objectType, list)) Then
            list = New List(Of IPropertyInfo)()
            cache.Add(objectType, list)
          End If
        End SyncLock
      End If
      Return list
    End Function

    ''' <summary>
    ''' Indicates that the specified property belongs
    ''' to the type.
    ''' </summary>
    ''' <typeparam name="T">
    ''' Type of property.
    ''' </typeparam>
    ''' <param name="objectType">
    ''' Type of object to which the property belongs.
    ''' </param>
    ''' <param name="info">
    ''' PropertyInfo object for the property.
    ''' </param>
    ''' <returns>
    ''' The provided IPropertyInfo object.
    ''' </returns>
    Public Function RegisterProperty(Of T)(ByVal objectType As Type, ByVal info As PropertyInfo(Of T)) As PropertyInfo(Of T)
      Dim list = GetPropertyListCache(objectType)
      SyncLock list
        list.Add(info)
        list.Sort()
      End SyncLock
      Return info
    End Function

    ''' <summary>
    ''' Returns a copy of the property list for
    ''' a given business object type. Returns
    ''' null if there are no properties registered
    ''' for the type.
    ''' </summary>
    ''' <param name="objectType">
    ''' The business object type.
    ''' </param>
    Public Function GetRegisteredProperties(ByVal objectType As Type) As List(Of IPropertyInfo)
      ' return a copy of the list to avoid
      ' possible locking issues
      Dim list = GetPropertyListCache(objectType)
      SyncLock list
        Return New List(Of IPropertyInfo)(list)
      End SyncLock
    End Function

  End Module

End Namespace
