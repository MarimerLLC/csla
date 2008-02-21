Imports Csla.Reflection

Namespace Server

  Friend Module DataPortalMethodCache

    Private _cache As Dictionary(Of MethodCacheKey, DataPortalMethodInfo) = New Dictionary(Of MethodCacheKey, DataPortalMethodInfo)()

    Public Function GetMethodInfo(ByVal objectType As Type, ByVal methodName As String, ByVal ParamArray parameters() As Object) As DataPortalMethodInfo

      Dim key = New MethodCacheKey(objectType.Name, methodName, MethodCaller.GetParameterTypes(parameters))
      Dim result As DataPortalMethodInfo = Nothing
      If (Not _cache.TryGetValue(key, result)) Then
        SyncLock _cache
          If (Not _cache.TryGetValue(key, result)) Then
            result = New DataPortalMethodInfo(MethodCaller.GetMethod(objectType, methodName, parameters))
            _cache.Add(key, result)
          End If
        End SyncLock
      End If
      Return result

    End Function

#Region "Data Portal Methods"

    ''' <summary>
    ''' Gets a reference to the DataPortal_Create method for
    ''' the specified business object type.
    ''' </summary>
    ''' <param name="objectType">Type of the business object.</param>
    ''' <param name="criteria">Criteria parameter value.</param>
    ''' <remarks>
    ''' If the criteria parameter value is an integer, that is a special
    ''' flag indicating that the parameter should be considered missing
    ''' (not Nothing/null - just not there).
    ''' </remarks>
    Friend Function GetCreateMethod(ByVal objectType As Type, ByVal criteria As Object) As DataPortalMethodInfo

      Dim method As DataPortalMethodInfo = Nothing
      If TypeOf criteria Is Integer Then
        ' an "Integer" criteria is a special flag indicating
        ' that criteria is empty and should not be used
        method = GetMethodInfo(objectType, "DataPortal_Create")

      Else
        method = GetMethodInfo(objectType, "DataPortal_Create", criteria)
      End If
      Return method

    End Function

    ''' <summary>
    ''' Gets a reference to the DataPortal_Fetch method for
    ''' the specified business object type.
    ''' </summary>
    ''' <param name="objectType">Type of the business object.</param>
    ''' <param name="criteria">Criteria parameter value.</param>
    ''' <remarks>
    ''' If the criteria parameter value is an integer, that is a special
    ''' flag indicating that the parameter should be considered missing
    ''' (not Nothing/null - just not there).
    ''' </remarks>
    Friend Function GetFetchMethod(ByVal objectType As Type, ByVal criteria As Object) As DataPortalMethodInfo

      Dim method As DataPortalMethodInfo = Nothing
      If TypeOf criteria Is Integer Then
        ' an "Integer" criteria is a special flag indicating
        ' that criteria is empty and should not be used
        method = GetMethodInfo(objectType, "DataPortal_Fetch")

      Else
        method = GetMethodInfo(objectType, "DataPortal_Fetch", criteria)
      End If
      Return method

    End Function

#End Region

  End Module

End Namespace