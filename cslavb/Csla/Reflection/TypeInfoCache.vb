Imports System.Reflection
Imports System.ComponentModel

Namespace Reflection

  ''' <summary>
  ''' Maintains a cache of property and
  ''' field info about types in the AppDomain.
  ''' </summary>
  Friend Class TypeInfoCache
#Region "PropertyInfo"

    Private Shared _propertyInfoCache As Dictionary(Of Type, List(Of PropertyInfo)) = New Dictionary(Of Type, List(Of PropertyInfo))()

    ''' <summary>
    ''' Gets a list of PropertyInfo objects for
    ''' all properties on the specified type.
    ''' </summary>
    ''' <param name="type">Type of object for which to get properties.</param>
    ''' <returns>
    ''' Value is returned from the cache if possible, otherwise the
    ''' information is loaded using reflection and placed in the
    ''' cache for future use.
    ''' </returns>
    ''' <remarks>
    ''' Only public instance properties are returned by this method, and
    ''' the inheritance hierarchy is flattened.
    ''' </remarks>
    Public Shared Function GetPropertyInfo(ByVal type As Type) As List(Of PropertyInfo)
      If (Not _propertyInfoCache.ContainsKey(type)) Then
        SyncLock _propertyInfoCache
          If (Not _propertyInfoCache.ContainsKey(type)) Then
            _propertyInfoCache.Add(type, New List(Of PropertyInfo)(type.GetProperties(BindingFlags.Public Or BindingFlags.Instance Or BindingFlags.FlattenHierarchy)))
          End If
        End SyncLock
      End If
      Return _propertyInfoCache(type)
    End Function

#End Region

#Region "BrowsablePropertyInfo"

    Private Shared _browsablePropertyInfoCache As Dictionary(Of Type, List(Of PropertyInfo)) = New Dictionary(Of Type, List(Of PropertyInfo))()

    ''' <summary>
    ''' Gets a list of Browsable PropertyInfo objects for
    ''' all properties on the specified type.
    ''' </summary>
    ''' <param name="type">Type of object for which to get properties.</param>
    ''' <returns>
    ''' Value is returned from the cache if possible, otherwise the
    ''' information is loaded using reflection and placed in the
    ''' cache for future use.
    ''' </returns>
    ''' <remarks>
    ''' The Browsable attribute indicates whether a property can
    ''' be used by data binding. Browsable properties can be
    ''' data bound, non-browsable properties can not. This method
    ''' only returns a list of Browsable PropertyDescriptor objects
    ''' for the specified type, so all items returned can
    ''' be used by data binding.
    ''' </remarks>
    Public Shared Function GetBrowsablePropertyInfo(ByVal type As Type) As List(Of PropertyInfo)
      If (Not _browsablePropertyInfoCache.ContainsKey(type)) Then
        SyncLock _browsablePropertyInfoCache
          If (Not _browsablePropertyInfoCache.ContainsKey(type)) Then
            Dim tmp As List(Of PropertyInfo) = New List(Of PropertyInfo)()
            Dim list As PropertyDescriptorCollection = TypeDescriptor.GetProperties(type)
            For Each item As PropertyDescriptor In list
              If item.IsBrowsable Then
                tmp.Add(type.GetProperty(item.Name))
              End If
            Next item
            _browsablePropertyInfoCache.Add(type, tmp)
          End If
        End SyncLock
      End If
      Return _browsablePropertyInfoCache(type)
    End Function

#End Region

#Region "PropertyDescriptor"

    Private Shared _propertyDescriptorCache As Dictionary(Of Type, PropertyDescriptorCollection) = New Dictionary(Of Type, PropertyDescriptorCollection)()

    ''' <summary>
    ''' Gets a list of PropertyDescriptor objects for
    ''' all properties on the specified type.
    ''' </summary>
    ''' <param name="type">Type of object for which to get properties.</param>
    ''' <returns>
    ''' Value is returned from the cache if possible, otherwise the
    ''' information is loaded using reflection and placed in the
    ''' cache for future use.
    ''' </returns>
    Public Shared Function GetPropertyDescriptors(ByVal type As Type) As PropertyDescriptorCollection
      If (Not _propertyDescriptorCache.ContainsKey(type)) Then
        SyncLock _propertyDescriptorCache
          If (Not _propertyDescriptorCache.ContainsKey(type)) Then
            _propertyDescriptorCache.Add(type, TypeDescriptor.GetProperties(type))
          End If
        End SyncLock
      End If
      Return _propertyDescriptorCache(type)
    End Function

#End Region

#Region "BrowsablePropertyDescriptor"

    Private Shared _browsablePropertyDescriptorCache As Dictionary(Of Type, List(Of PropertyDescriptor)) = New Dictionary(Of Type, List(Of PropertyDescriptor))()

    ''' <summary>
    ''' Gets a list of Browsable PropertyDescriptor 
    ''' objects for all properties on the specified type.
    ''' </summary>
    ''' <param name="type">Type of object for which to get properties.</param>
    ''' <returns>
    ''' Value is returned from the cache if possible, otherwise the
    ''' information is loaded using reflection and placed in the
    ''' cache for future use.
    ''' </returns>
    ''' <remarks>
    ''' The Browsable attribute indicates whether a property can
    ''' be used by data binding. Browsable properties can be
    ''' data bound, non-browsable properties can not. This method
    ''' only returns a list of Browsable PropertyDescriptor objects
    ''' for the specified type, so all items returned can
    ''' be used by data binding.
    ''' </remarks>
    Public Shared Function GetBrowsablePropertyDescriptors(ByVal type As Type) As List(Of PropertyDescriptor)
      If (Not _browsablePropertyDescriptorCache.ContainsKey(type)) Then
        SyncLock _browsablePropertyDescriptorCache
          If (Not _browsablePropertyDescriptorCache.ContainsKey(type)) Then
            Dim tmp As List(Of PropertyDescriptor) = New List(Of PropertyDescriptor)()
            Dim list As PropertyDescriptorCollection = TypeDescriptor.GetProperties(type)
            For Each item As PropertyDescriptor In list
              If item.IsBrowsable Then
                tmp.Add(item)
              End If
            Next item
            _browsablePropertyDescriptorCache.Add(type, tmp)
          End If
        End SyncLock
      End If
      Return _browsablePropertyDescriptorCache(type)
    End Function

#End Region

#Region "FieldInfo"

    Private Shared _fieldInfoCache As Dictionary(Of Type, List(Of FieldInfo)) = New Dictionary(Of Type, List(Of FieldInfo))()

    ''' <summary>
    ''' Gets a list of FieldInfo objects for
    ''' all instance fields (public and non-public) on 
    ''' the specified type.
    ''' </summary>
    ''' <param name="type">Type of object for which to get fields.</param>
    ''' <returns>
    ''' Value is returned from the cache if possible, otherwise the
    ''' information is loaded using reflection and placed in the
    ''' cache for future use.
    ''' </returns>
    Public Shared Function GetFieldInfo(ByVal type As Type) As List(Of FieldInfo)
      If (Not _fieldInfoCache.ContainsKey(type)) Then
        SyncLock _fieldInfoCache
          If (Not _fieldInfoCache.ContainsKey(type)) Then
            _fieldInfoCache.Add(type, New List(Of FieldInfo)(type.GetFields(BindingFlags.NonPublic Or BindingFlags.Instance Or BindingFlags.Public)))
          End If
        End SyncLock
      End If
      Return _fieldInfoCache(type)
    End Function

#End Region

  End Class

End Namespace
