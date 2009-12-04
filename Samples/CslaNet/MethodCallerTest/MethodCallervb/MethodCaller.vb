Imports System
Imports System.Collections.Generic
Imports System.Reflection
Imports Csla.Server
Imports Csla

Namespace MethodCallervb

  Public Module MethodCaller

    Private Const allLevelFlags As BindingFlags = BindingFlags.FlattenHierarchy Or BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.NonPublic

    Private Const oneLevelFlags As BindingFlags = BindingFlags.DeclaredOnly Or BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.NonPublic

#Region "Dynamic Method Cache"

    Private _methodCache As Dictionary(Of MethodCacheKey, DynamicMethodHandle) = New Dictionary(Of MethodCacheKey, DynamicMethodHandle)()

    Private Function GetCachedMethod(ByVal obj As Object, ByVal info As MethodInfo, ByVal ParamArray parameters() As Object) As DynamicMethodHandle

      Dim key = New MethodCacheKey(obj.GetType().FullName, info.Name, GetParameterTypes(parameters))
      Dim mh As DynamicMethodHandle = Nothing
      If (Not _methodCache.TryGetValue(key, mh)) Then
        mh = New DynamicMethodHandle(info, parameters)
        _methodCache.Add(key, mh)
      End If
      Return mh

    End Function

    Private Function GetCachedMethod(ByVal obj As Object, ByVal method As String, ByVal ParamArray parameters() As Object) As DynamicMethodHandle

      Dim key = New MethodCacheKey(obj.GetType().FullName, method, GetParameterTypes(parameters))
      Dim mh As DynamicMethodHandle = Nothing
      If (Not _methodCache.TryGetValue(key, mh)) Then
        Dim info As MethodInfo = GetMethod(obj.GetType(), method, parameters)
        mh = New DynamicMethodHandle(info, parameters)
        _methodCache.Add(key, mh)
      End If
      Return mh

    End Function

#End Region

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
    Friend Function GetCreateMethod(ByVal objectType As Type, ByVal criteria As Object) As MethodInfo

      Dim method As MethodInfo = Nothing
      If TypeOf criteria Is Integer Then
        ' an "Integer" criteria is a special flag indicating
        ' that criteria is empty and should not be used
        method = MethodCaller.GetMethod(objectType, "DataPortal_Create")

      Else
        method = MethodCaller.GetMethod(objectType, "DataPortal_Create", criteria)
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
    Friend Function GetFetchMethod(ByVal objectType As Type, ByVal criteria As Object) As MethodInfo

      Dim method As MethodInfo = Nothing
      If TypeOf criteria Is Integer Then
        ' an "Integer" criteria is a special flag indicating
        ' that criteria is empty and should not be used
        method = MethodCaller.GetMethod(objectType, "DataPortal_Fetch")

      Else
        method = MethodCaller.GetMethod(objectType, "DataPortal_Fetch", criteria)
      End If
      Return method

    End Function

#End Region

#Region "Call Method"

    ''' <summary>
    ''' Uses reflection to dynamically invoke a method
    ''' if that method is implemented on the target object.
    ''' </summary>
    ''' <param name="obj">
    ''' Object containing method.
    ''' </param>
    ''' <param name="method">
    ''' Name of the method.
    ''' </param>
    ''' <param name="parameters">
    ''' Parameters to pass to method.
    ''' </param>
    Public Function CallMethodIfImplemented(ByVal obj As Object, ByVal method As String, ByVal ParamArray parameters() As Object) As Object

      Dim mh = GetCachedMethod(obj, method, parameters)
      If mh Is Nothing OrElse mh.DynamicMethod Is Nothing Then
        Return Nothing
      End If
      Return CallMethod(obj, mh, parameters)

    End Function

    ''' <summary>
    ''' Uses reflection to dynamically invoke a method,
    ''' throwing an exception if it is not
    ''' implemented on the target object.
    ''' </summary>
    ''' <param name="obj">
    ''' Object containing method.
    ''' </param>
    ''' <param name="method">
    ''' Name of the method.
    ''' </param>
    ''' <param name="parameters">
    ''' Parameters to pass to method.
    ''' </param>
    Public Function CallMethod(ByVal obj As Object, ByVal method As String, ByVal ParamArray parameters() As Object) As Object

      Dim mh = GetCachedMethod(obj, method, parameters)
      If mh Is Nothing OrElse mh.DynamicMethod Is Nothing Then
        Throw New NotImplementedException(method & " " & My.Resources.MethodNotImplemented)
      End If
      Return CallMethod(obj, mh, parameters)

    End Function

    ''' <summary>
    ''' Uses reflection to dynamically invoke a method,
    ''' throwing an exception if it is not
    ''' implemented on the target object.
    ''' </summary>
    ''' <param name="obj">
    ''' Object containing method.
    ''' </param>
    ''' <param name="info">
    ''' MethodInfo for the method.
    ''' </param>
    ''' <param name="parameters">
    ''' Parameters to pass to method.
    ''' </param>
    Public Function CallMethod(ByVal obj As Object, ByVal info As MethodInfo, ByVal ParamArray parameters() As Object) As Object

      Dim mh = GetCachedMethod(obj, info, parameters)
      If mh Is Nothing OrElse mh.DynamicMethod Is Nothing Then
        Throw New NotImplementedException(info.Name & " " & My.Resources.MethodNotImplemented)
      End If
      Return CallMethod(obj, mh, parameters)

    End Function

    ''' <summary>
    ''' Uses reflection to dynamically invoke a method,
    ''' throwing an exception if it is not implemented
    ''' on the target object.
    ''' </summary>
    ''' <param name="obj">
    ''' Object containing method.
    ''' </param>
    ''' <param name="methodHandle">
    ''' MethodHandle for the method.
    ''' </param>
    ''' <param name="parameters">
    ''' Parameters to pass to method.
    ''' </param>
    Private Function CallMethod(ByVal obj As Object, ByVal methodHandle As DynamicMethodHandle, ByVal ParamArray parameters() As Object) As Object

      Dim result As Object = Nothing
      Dim method = methodHandle.DynamicMethod

      Dim inParams() As Object = Nothing
      If parameters Is Nothing Then
        inParams = New Object() {Nothing}

      Else
        inParams = parameters
      End If

      If methodHandle.HasFinalArrayParam Then
        Dim pCount = methodHandle.MethodParamsLength
        ' last param is a param array or only param is an array
        Dim extras = inParams.Length - (pCount - 1)

        ' 1 or more params go in the param array
        ' copy extras into an array
        Dim extraArray() As Object = GetExtrasArray(extras, methodHandle.FinalArrayElementType)
        Array.Copy(inParams, extraArray, extras)

        ' copy items into new array
        Dim paramList() As Object = New Object(pCount - 1) {}
        For pos = 0 To pCount - 2
          paramList(pos) = parameters(pos)
        Next pos
        paramList(paramList.Length - 1) = extraArray

        ' use new array
        inParams = paramList
      End If

      Try
        result = methodHandle.DynamicMethod(obj, inParams)

      Catch ex As Exception
        Dim inner As Exception = Nothing
        If ex.InnerException Is Nothing Then
          inner = ex

        Else
          inner = ex.InnerException
        End If
        Throw New CallMethodException(methodHandle.MethodName & " " & My.Resources.MethodCallFailed, inner)
      End Try
      Return result

    End Function

    Private Function GetExtrasArray(ByVal count As Integer, ByVal arrayType As Type) As Object()

      Return CType(System.Array.CreateInstance(arrayType.GetElementType(), count), Object())

    End Function

#End Region

#Region "Get/Find Method"

    ''' <summary>
    ''' Uses reflection to locate a matching method
    ''' on the target object.
    ''' </summary>
    ''' <param name="objectType">
    ''' Type of object containing method.
    ''' </param>
    ''' <param name="method">
    ''' Name of the method.
    ''' </param>
    ''' <param name="parameters">
    ''' Parameters to pass to method.
    ''' </param>
    Public Function GetMethod(ByVal objectType As Type, ByVal method As String, ByVal ParamArray parameters() As Object) As MethodInfo

      Dim result As MethodInfo = Nothing

      Dim inParams() As Object = Nothing
      If parameters Is Nothing Then

        inParams = New Object() {Nothing}
      Else
        inParams = parameters
      End If

      ' try to find a strongly typed match

      ' first see if there's a matching method
      ' where all params match types
      result = FindMethod(objectType, method, GetParameterTypes(inParams))

      If result Is Nothing Then
        ' no match found - so look for any method
        ' with the right number of parameters
        result = FindMethod(objectType, method, inParams.Length)
      End If

      ' no strongly typed match found, get default
      If result Is Nothing Then
        Try
          result = objectType.GetMethod(method, allLevelFlags)

        Catch e1 As AmbiguousMatchException
          Dim methods() As MethodInfo = objectType.GetMethods()
          For Each m As MethodInfo In methods
            If m.Name = method AndAlso m.GetParameters().Length = inParams.Length Then
              result = m
              Exit For
            End If
          Next m
          If result Is Nothing Then
            Throw
          End If
        End Try
      End If

      Return result
    End Function

    ''' <summary>
    ''' Returns information about the specified
    ''' method, even if the parameter types are
    ''' generic and are located in an abstract
    ''' generic base class.
    ''' </summary>
    ''' <param name="objectType">
    ''' Type of object containing method.
    ''' </param>
    ''' <param name="method">
    ''' Name of the method.
    ''' </param>
    ''' <param name="types">
    ''' Parameter types to pass to method.
    ''' </param>
    Public Function FindMethod(ByVal objectType As Type, ByVal method As String, ByVal types() As Type) As MethodInfo

      Dim info As MethodInfo = Nothing
      Do
        ' find for a strongly typed match
        info = objectType.GetMethod(method, oneLevelFlags, Nothing, types, Nothing)
        If info IsNot Nothing Then
          Exit Do ' match found
        End If

        objectType = objectType.BaseType
      Loop While objectType IsNot Nothing

      Return info

    End Function

    ''' <summary>
    ''' Returns information about the specified
    ''' method, finding the method based purely
    ''' on the method name and number of parameters.
    ''' </summary>
    ''' <param name="objectType">
    ''' Type of object containing method.
    ''' </param>
    ''' <param name="method">
    ''' Name of the method.
    ''' </param>
    ''' <param name="parameterCount">
    ''' Number of parameters to pass to method.
    ''' </param>
    Public Function FindMethod(ByVal objectType As Type, ByVal method As String, ByVal parameterCount As Integer) As MethodInfo

      ' walk up the inheritance hierarchy looking
      ' for a method with the right number of
      ' parameters
      Dim result As MethodInfo = Nothing
      Dim currentType As Type = objectType
      Do
        Dim info As MethodInfo = currentType.GetMethod(method, oneLevelFlags)
        If info IsNot Nothing Then
          Dim infoParams = info.GetParameters()
          Dim pCount = infoParams.Length
          If pCount > 0 AndAlso ((pCount = 1 AndAlso infoParams(0).ParameterType.IsArray) OrElse (infoParams(pCount - 1).GetCustomAttributes(GetType(ParamArrayAttribute), True).Length > 0)) Then
            ' last param is a param array or only param is an array
            If parameterCount >= pCount - 1 Then
              ' got a match so use it
              result = info
              Exit Do
            End If

          ElseIf pCount = parameterCount Then
            ' got a match so use it
            result = info
            Exit Do
          End If
        End If
        currentType = currentType.BaseType
      Loop While currentType IsNot Nothing

      Return result

    End Function

#End Region

    ''' <summary>
    ''' Returns an array of Type objects corresponding
    ''' to the type of parameters provided.
    ''' </summary>
    ''' <param name="parameters">
    ''' Parameter values.
    ''' </param>
    Public Function GetParameterTypes(ByVal parameters() As Object) As Type()

      Dim result As List(Of Type) = New List(Of Type)()

      If parameters Is Nothing Then
        result.Add(GetType(Object))

      Else
        For Each item As Object In parameters
          If item Is Nothing Then
            result.Add(GetType(Object))

          Else
            result.Add(item.GetType())
          End If
        Next item
      End If
      Return result.ToArray()

    End Function

    ''' <summary>
    ''' Returns a business object type based on
    ''' the supplied criteria object.
    ''' </summary>
    ''' <param name="criteria">
    ''' Criteria object.
    ''' </param>
    Public Function GetObjectType(ByVal criteria As Object) As Type

      If criteria.GetType().IsSubclassOf(GetType(CriteriaBase)) Then
        ' get the type of the actual business object
        ' from CriteriaBase 
        Return (CType(criteria, CriteriaBase)).ObjectType

      Else
        ' get the type of the actual business object
        ' based on the nested class scheme in the book
        Return criteria.GetType().DeclaringType
      End If

    End Function

  End Module

End Namespace