Imports System.Reflection
Imports Csla.Server

Friend Module MethodCaller

  Private Const allLevelFlags As BindingFlags = _
    BindingFlags.FlattenHierarchy Or _
    BindingFlags.Instance Or _
    BindingFlags.Public Or _
    BindingFlags.NonPublic

  Private Const oneLevelFlags As BindingFlags = _
      BindingFlags.DeclaredOnly Or _
      BindingFlags.Instance Or _
      BindingFlags.Public Or _
      BindingFlags.NonPublic


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
  Public Function GetCreateMethod(ByVal objectType As Type, ByVal criteria As Object) As MethodInfo

    Dim method As MethodInfo
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
  Public Function GetFetchMethod(ByVal objectType As Type, ByVal criteria As Object) As MethodInfo

    Dim method As MethodInfo
    If TypeOf criteria Is Integer Then
      ' an "Integer" criteria is a special flag indicating
      ' that criteria is empty and should not be used
      method = MethodCaller.GetMethod(objectType, "DataPortal_Fetch")

    Else
      method = MethodCaller.GetMethod(objectType, "DataPortal_Fetch", criteria)
    End If
    Return method

  End Function

  ''' <summary>
  ''' Uses reflection to dynamically invoke a method
  ''' if that method is implemented on the target object.
  ''' </summary>
  Public Function CallMethodIfImplemented(ByVal obj As Object, _
    ByVal method As String, ByVal ParamArray parameters() As Object) As Object

    Dim info As MethodInfo = _
      GetMethod(obj.GetType, method, parameters)
    If info IsNot Nothing Then
      Return CallMethod(obj, info, parameters)

    Else
      Return Nothing
    End If

  End Function

  ''' <summary>
  ''' Uses reflection to dynamically invoke a method,
  ''' throwing an exception if it is not
  ''' implemented on the target object.
  ''' </summary>
  Public Function CallMethod(ByVal obj As Object, _
    ByVal method As String, ByVal ParamArray parameters() As Object) As Object

    Dim info As MethodInfo = _
      GetMethod(obj.GetType, method, parameters)
    If info Is Nothing Then
      Throw New NotImplementedException( _
        method & " " & My.Resources.MethodNotImplemented)
    End If

    Return CallMethod(obj, info, parameters)

  End Function

  ''' <summary>
  ''' Uses reflection to dynamically invoke a method,
  ''' throwing an exception if it is not implemented
  ''' on the target object.
  ''' </summary>
  Public Function CallMethod(ByVal obj As Object, _
    ByVal info As MethodInfo, ByVal ParamArray parameters() As Object) _
    As Object

    Dim result As Object
    Dim infoParams = info.GetParameters
    Dim inParams() As Object
    If parameters Is Nothing Then
      inParams = New Object() {Nothing}

    Else
      inParams = parameters
    End If
    If infoParams.Length > 0 AndAlso infoParams(infoParams.Length - 1).GetCustomAttributes(GetType(ParamArrayAttribute), True).Length > 0 Then
      ' last param is a param array
      Dim extras = inParams.Length - (infoParams.Length - 1)
      ' 1 or more parameters go in the param array
      ' copy extras into an array
      Dim extraArray() As Object = GetExtrasArray(extras)
      For pos = 0 To extras - 1
        extraArray(pos) = inParams(infoParams.Length - 1 + pos)
      Next

      ' copy items into new array
      Dim paramList(infoParams.Length - 1) As Object
      For pos = 0 To infoParams.Length - 2
        paramList(pos) = parameters(pos)
      Next
      paramList(paramList.Length - 1) = extraArray

      ' use new array
      inParams = paramList
    End If
    Try
      result = info.Invoke(obj, inParams)

    Catch e As Exception
      Dim inner As Exception
      If e.InnerException Is Nothing Then
        inner = e

      Else
        inner = e.InnerException
      End If
      Throw New CallMethodException( _
        info.Name & " " & My.Resources.MethodCallFailed, inner)
    End Try
    Return result

  End Function

  Private Function GetExtrasArray(ByVal count As Integer) As Object()

    If count > 0 Then
      Dim result(count - 1) As Object
      Return result

    Else
      Dim result() As Object = {}
      Return result
    End If

  End Function

  ''' <summary>
  ''' Uses reflection to locate a matching method
  ''' on the target object.
  ''' </summary>
  Public Function GetMethod(ByVal objectType As Type, _
    ByVal method As String, ByVal ParamArray parameters() As Object) _
    As MethodInfo

    Dim result As MethodInfo = Nothing

    Dim inParams() As Object
    If parameters Is Nothing Then
      inParams = New Object() {Nothing}

    Else
      inParams = parameters
    End If

    ' try to find a strongly typed match

    ' first see if there's a matching method
    ' where all parameters match types
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

      Catch ex As AmbiguousMatchException
        Dim methods() As MethodInfo = objectType.GetMethods
        For Each m As MethodInfo In methods
          If m.Name = method AndAlso _
          m.GetParameters.Length = inParams.Length Then
            result = m
            Exit For
          End If
        Next
        If result Is Nothing Then
          Throw
        End If
      End Try
    End If

    Return result

  End Function

  Friend Function GetParameterTypes(ByVal parameters As Object()) As Type()

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
      Next
    End If
    Return result.ToArray()

  End Function

  ''' <summary>
  ''' Returns a business object type based on
  ''' the supplied criteria object.
  ''' </summary>
  Public Function GetObjectType(ByVal criteria As Object) As Type

    If criteria.GetType.IsSubclassOf(GetType(CriteriaBase)) Then
      ' get the type of the actual business object
      ' from CriteriaBase 
      Return CType(criteria, CriteriaBase).ObjectType

    Else
      ' get the type of the actual business object
      ' based on the nested class scheme in the book
      Return criteria.GetType.DeclaringType
    End If

  End Function

  ''' <summary>
  ''' Returns information about the specified
  ''' method, even if the parameter types are
  ''' generic and are located in an abstract
  ''' generic base class.
  ''' </summary>
  Public Function FindMethod(ByVal objType As Type, ByVal method As String, ByVal types As Type()) As MethodInfo

    Dim info As MethodInfo = Nothing
    Do
      ' find for a strongly typed match
      info = objType.GetMethod(method, oneLevelFlags, Nothing, types, Nothing)
      If info IsNot Nothing Then
        Exit Do ' match found
      End If

      objType = objType.BaseType
    Loop While objType IsNot Nothing

    Return info

  End Function

  ''' <summary>
  ''' Returns information about the specified
  ''' method, finding the method based purely
  ''' on the method name and number of parameters.
  ''' </summary>
  Public Function FindMethod(ByVal objType As Type, ByVal method As String, ByVal parameterCount As Integer) As MethodInfo

    ' walk up the inheritance hierarchy looking
    ' for a method with the right number of
    ' parameters
    Dim result As MethodInfo = Nothing
    Dim currentType As Type = objType
    Do
      Dim info As MethodInfo = _
        currentType.GetMethod(method, oneLevelFlags)
      If info IsNot Nothing Then
        Dim infoParams = info.GetParameters
        Dim pCount = infoParams.Length
        If pcount > 0 AndAlso infoParams(pCount - 1).GetCustomAttributes(GetType(ParamArrayAttribute), True).Length > 0 Then
          ' last param is a param array
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
    Loop Until currentType Is Nothing

    Return result

  End Function

End Module
