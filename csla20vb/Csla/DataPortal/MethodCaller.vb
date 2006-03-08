Imports System.Reflection
Imports Csla.Server

Friend Module MethodCaller

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

    Dim flags As BindingFlags = _
      BindingFlags.FlattenHierarchy Or _
      BindingFlags.Instance Or _
      BindingFlags.Public Or _
      BindingFlags.NonPublic

    Dim result As MethodInfo = Nothing

    ' try to find a strongly typed match
    If parameters.Length > 0 Then
      ' put all param types into an array of Type
      Dim paramsAllNothing As Boolean = True
      Dim types As New List(Of Type)
      For Each item As Object In parameters
        If item Is Nothing Then
          types.Add(GetType(Object))

        Else
          types.Add(item.GetType)
          paramsAllNothing = False
        End If
      Next

      If paramsAllNothing Then
        ' all params are Nothing so we have
        ' no type info to go on
        Dim oneLevelFlags As BindingFlags = _
          BindingFlags.DeclaredOnly Or _
          BindingFlags.Instance Or _
          BindingFlags.Public Or _
          BindingFlags.NonPublic
        Dim typesArray() As Type = types.ToArray

        ' walk up the inheritance hierarchy looking
        ' for a method with the right number of
        ' parameters
        Dim currentType As Type = objectType
        Do
          Dim info As MethodInfo = _
            currentType.GetMethod(method, oneLevelFlags)
          If info IsNot Nothing Then
            If info.GetParameters.Length = parameters.Length Then
              ' got a match so use it
              result = info
              Exit Do
            End If
          End If
          currentType = currentType.BaseType
        Loop Until currentType Is Nothing

      Else
        ' at least one param has a real value
        ' so search for a strongly typed match
        result = objectType.GetMethod(method, flags, Nothing, _
          CallingConventions.Any, types.ToArray, Nothing)
      End If
    End If

    ' no strongly typed match found, get default
    If result Is Nothing Then
      Try
        result = objectType.GetMethod(method, flags)

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

End Module
