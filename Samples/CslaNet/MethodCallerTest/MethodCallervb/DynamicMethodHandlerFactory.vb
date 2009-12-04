Imports System.Reflection
Imports System.Reflection.Emit

Namespace MethodCallervb

  Public Delegate Function DynamicCtorDelegate() As Object
  Public Delegate Function DynamicMethodDelegate(ByVal target As Object, ByVal args() As Object) As Object

  Friend Module DynamicMethodHandlerFactory

    Public Function CreateConstructor(ByVal constructor As ConstructorInfo) As DynamicCtorDelegate

      If constructor Is Nothing Then
        Throw New ArgumentNullException("constructor")
      End If
      If constructor.GetParameters().Length > 0 Then
        Throw New NotSupportedException("Constructor with parameter(s) is not supported")
      End If

      Dim dm As DynamicMethod = New DynamicMethod("ctor", constructor.DeclaringType, Type.EmptyTypes, True)

      Dim il As ILGenerator = dm.GetILGenerator()
      il.Emit(OpCodes.Nop)
      il.Emit(OpCodes.Newobj, constructor)
      il.Emit(OpCodes.Ret)

      Return CType(dm.CreateDelegate(GetType(DynamicCtorDelegate)), DynamicCtorDelegate)

    End Function

    Public Function CreateMethod(ByVal method As MethodInfo) As DynamicMethodDelegate

      Dim pi() As ParameterInfo = method.GetParameters()

      Dim dm As DynamicMethod = _
        New DynamicMethod("dm", GetType(Object), New Type() {GetType(Object), GetType(Object())}, GetType(DynamicMethodHandlerFactory), True)

      Dim il As ILGenerator = dm.GetILGenerator()

      il.Emit(OpCodes.Ldarg_0)

      For index As Integer = 0 To pi.Length - 1
        il.Emit(OpCodes.Ldarg_1)
        il.Emit(OpCodes.Ldc_I4, index)

        Dim parameterType As Type = pi(index).ParameterType
        If parameterType.IsByRef Then
          parameterType = parameterType.GetElementType()
          If parameterType.IsValueType Then
            il.Emit(OpCodes.Ldelem_Ref)
            il.Emit(OpCodes.Unbox, parameterType)

          Else
            il.Emit(OpCodes.Ldelema, parameterType)
          End If

        Else
          il.Emit(OpCodes.Ldelem_Ref)

          If parameterType.IsValueType Then
            il.Emit(OpCodes.Unbox, parameterType)
            il.Emit(OpCodes.Ldobj, parameterType)
          End If
        End If
      Next index

      If (method.IsAbstract OrElse method.IsVirtual) AndAlso (Not method.IsFinal) AndAlso (Not method.DeclaringType.IsSealed) Then
        il.Emit(OpCodes.Callvirt, method)
      Else
        il.Emit(OpCodes.Call, method)
      End If

      If method.ReturnType Is GetType(Void) Then
        il.Emit(OpCodes.Ldnull)
      ElseIf method.ReturnType.IsValueType Then
        il.Emit(OpCodes.Box, method.ReturnType)
      End If
      il.Emit(OpCodes.Ret)

      Return CType(dm.CreateDelegate(GetType(DynamicMethodDelegate)), DynamicMethodDelegate)

    End Function

  End Module

End Namespace