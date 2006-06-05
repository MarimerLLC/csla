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

    ' call a Public method on the object
    Dim result As Object
    Try
      result = info.Invoke(obj, parameters)

    Catch e As Exception
      Throw New CallMethodException( _
        info.Name & " " & My.Resources.MethodCallFailed, _
        e.InnerException)
    End Try
    Return result

  End Function

  ''' <summary>
  ''' Uses reflection to locate a matching method
  ''' on the target object.
  ''' </summary>
  Public Function GetMethod(ByVal objectType As Type, _
    ByVal method As String, ByVal ParamArray parameters() As Object) _
    As MethodInfo

    Dim result As MethodInfo = Nothing

    ' try to find a strongly typed match
    If parameters.Length > 0 Then
      ' put all param types into an array of Type
      Dim types As New List(Of Type)
      For Each item As Object In parameters
        If item Is Nothing Then
          types.Add(GetType(Object))

        Else
          types.Add(item.GetType)
        End If
      Next

      ' first see if there's a matching method
      ' where all params match types
      result = FindMethod(objectType, method, types.ToArray)

      If result Is Nothing Then
        ' no match found - so look for any method
        ' with the right number of parameters
        result = FindMethod(objectType, method, parameters.Length)
      End If
    End If

    ' no strongly typed match found, get default
    If result Is Nothing Then
      Try
        result = objectType.GetMethod(method, allLevelFlags)

      Catch ex As AmbiguousMatchException
        Dim methods() As MethodInfo = objectType.GetMethods
        For Each m As MethodInfo In methods
          If m.Name = method AndAlso _
          m.GetParameters.Length = parameters.Length Then
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
        If info.GetParameters.Length = parameterCount Then
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
