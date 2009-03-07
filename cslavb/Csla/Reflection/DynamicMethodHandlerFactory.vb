Imports System
Imports System.Reflection
Imports System.Reflection.Emit

Namespace Reflection
  ''' <summary> 
  ''' Delegate for a dynamic constructor method. 
  ''' </summary> 
  Public Delegate Function DynamicCtorDelegate() As Object
  ''' <summary> 
  ''' Delegate for a dynamic method. 
  ''' </summary> 
  ''' <param name="target"> 
  ''' Object containg method to invoke. 
  ''' </param> 
  ''' <param name="args"> 
  ''' Parameters passed to method. 
  ''' </param> 
  Public Delegate Function DynamicMethodDelegate(ByVal target As Object, ByVal args As Object()) As Object
  ''' <summary>
  ''' Delegate for getting a value.
  ''' </summary>
  ''' <param name="target">Target object.</param>
  ''' <returns></returns>
  Public Delegate Function DynamicMemberGetDelegate(ByVal target As Object) As Object
  ''' <summary>
  ''' Delegate for setting a value.
  ''' </summary>
  ''' <param name="target">Target object.</param>
  ''' <param name="arg">Argument value.</param>
  Public Delegate Sub DynamicMemberSetDelegate(ByVal target As Object, ByVal arg As Object)

  Friend Module DynamicMethodHandlerFactory

    Public Function CreateConstructor(ByVal constructor As ConstructorInfo) As DynamicCtorDelegate
      If constructor Is Nothing Then
        Throw New ArgumentNullException("constructor")
      End If
      If constructor.GetParameters().Length > 0 Then
        Throw New NotSupportedException(My.Resources.ConstructorsWithParametersNotSupported)
      End If

      Dim dm As New DynamicMethod("ctor", constructor.DeclaringType, Type.EmptyTypes, True)

      Dim il As ILGenerator = dm.GetILGenerator()
      il.Emit(OpCodes.Nop)
      il.Emit(OpCodes.Newobj, constructor)
      il.Emit(OpCodes.Ret)

      Return DirectCast(dm.CreateDelegate(GetType(DynamicCtorDelegate)), DynamicCtorDelegate)
    End Function

    Public Function CreateMethod(ByVal method As MethodInfo) As DynamicMethodDelegate
      Dim pi As ParameterInfo() = method.GetParameters()

      Dim dm As New DynamicMethod("dm", GetType(Object), New Type() {GetType(Object), GetType(Object())}, GetType(DynamicMethodHandlerFactory), True)

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
      Next

      If (method.IsAbstract OrElse method.IsVirtual) AndAlso Not method.IsFinal AndAlso Not method.DeclaringType.IsSealed Then
        il.Emit(OpCodes.Callvirt, method)
      Else
        il.Emit(OpCodes.Callvirt, method)
      End If

      If method.ReturnType Is GetType(Void) Then
        il.Emit(OpCodes.Ldnull)
      ElseIf method.ReturnType.IsValueType Then
        il.Emit(OpCodes.Box, method.ReturnType)
      End If
      il.Emit(OpCodes.Ret)

      Return DirectCast(dm.CreateDelegate(GetType(DynamicMethodDelegate)), DynamicMethodDelegate)
    End Function

    Public Function CreatePropertyGetter(ByVal [property] As PropertyInfo) As DynamicMemberGetDelegate
      If [property] Is Nothing Then
        Throw New ArgumentNullException("property")
      End If

      If Not [property].CanRead Then
        Return Nothing
      End If

      Dim getMethod As MethodInfo = [property].GetGetMethod()
      If getMethod Is Nothing Then
        'maybe is private 
        getMethod = [property].GetGetMethod(True)
      End If

      Dim dm As New DynamicMethod("propg", GetType(Object), New Type() {GetType(Object)}, [property].DeclaringType, True)

      Dim il As ILGenerator = dm.GetILGenerator()

      If Not getMethod.IsStatic Then
        il.Emit(OpCodes.Ldarg_0)
        il.EmitCall(OpCodes.Callvirt, getMethod, Nothing)
      Else
        il.EmitCall(OpCodes.[Call], getMethod, Nothing)
      End If

      If [property].PropertyType.IsValueType Then
        il.Emit(OpCodes.Box, [property].PropertyType)
      End If

      il.Emit(OpCodes.Ret)

      Return DirectCast(dm.CreateDelegate(GetType(DynamicMemberGetDelegate)), DynamicMemberGetDelegate)
    End Function

    Public Function CreatePropertySetter(ByVal [property] As PropertyInfo) As DynamicMemberSetDelegate
      If [property] Is Nothing Then
        Throw New ArgumentNullException("property")
      End If

      If Not [property].CanWrite Then
        Return Nothing
      End If

      Dim setMethod As MethodInfo = [property].GetSetMethod()
      If setMethod Is Nothing Then
        'maybe is private 
        setMethod = [property].GetSetMethod(True)
      End If

      Dim dm As New DynamicMethod("props", Nothing, New Type() {GetType(Object), GetType(Object)}, [property].DeclaringType, True)

      Dim il As ILGenerator = dm.GetILGenerator()

      If Not setMethod.IsStatic Then
        il.Emit(OpCodes.Ldarg_0)
      End If
      il.Emit(OpCodes.Ldarg_1)

      EmitCastToReference(il, [property].PropertyType)
      If Not setMethod.IsStatic AndAlso Not [property].DeclaringType.IsValueType Then
        il.EmitCall(OpCodes.Callvirt, setMethod, Nothing)
      Else
        il.EmitCall(OpCodes.[Call], setMethod, Nothing)
      End If

      il.Emit(OpCodes.Ret)

      Return DirectCast(dm.CreateDelegate(GetType(DynamicMemberSetDelegate)), DynamicMemberSetDelegate)
    End Function

    Public Function CreateFieldGetter(ByVal field As FieldInfo) As DynamicMemberGetDelegate
      If field Is Nothing Then
        Throw New ArgumentNullException("field")
      End If

      Dim dm As New DynamicMethod("fldg", GetType(Object), New Type() {GetType(Object)}, field.DeclaringType, True)

      Dim il As ILGenerator = dm.GetILGenerator()

      If Not field.IsStatic Then
        il.Emit(OpCodes.Ldarg_0)

        EmitCastToReference(il, field.DeclaringType)
        'to handle struct object 
        il.Emit(OpCodes.Ldfld, field)
      Else
        il.Emit(OpCodes.Ldsfld, field)
      End If

      If field.FieldType.IsValueType Then
        il.Emit(OpCodes.Box, field.FieldType)
      End If

      il.Emit(OpCodes.Ret)

      Return DirectCast(dm.CreateDelegate(GetType(DynamicMemberGetDelegate)), DynamicMemberGetDelegate)
    End Function

    Public Function CreateFieldSetter(ByVal field As FieldInfo) As DynamicMemberSetDelegate
      If field Is Nothing Then
        Throw New ArgumentNullException("field")
      End If

      Dim dm As New DynamicMethod("flds", Nothing, New Type() {GetType(Object), GetType(Object)}, field.DeclaringType, True)

      Dim il As ILGenerator = dm.GetILGenerator()

      If Not field.IsStatic Then
        il.Emit(OpCodes.Ldarg_0)
      End If
      il.Emit(OpCodes.Ldarg_1)

      EmitCastToReference(il, field.FieldType)

      If Not field.IsStatic Then
        il.Emit(OpCodes.Stfld, field)
      Else
        il.Emit(OpCodes.Stsfld, field)
      End If
      il.Emit(OpCodes.Ret)

      Return DirectCast(dm.CreateDelegate(GetType(DynamicMemberSetDelegate)), DynamicMemberSetDelegate)
    End Function

    Private Sub EmitCastToReference(ByVal il As ILGenerator, ByVal type As Type)
      If type.IsValueType Then
        il.Emit(OpCodes.Unbox_Any, type)
      Else
        il.Emit(OpCodes.Castclass, type)
      End If
    End Sub

  End Module
End Namespace