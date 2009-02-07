Imports System
Imports System.Linq.Expressions
Imports System.Reflection

Namespace Reflection

  ''' <summary>
  ''' Provides strong-typed reflection of the <typeparamref name="TTarget"/> 
  ''' type.
  ''' </summary>
  ''' <typeparam name="TTarget">Type to reflect.</typeparam>
  Public Class Reflect(Of TTarget)

    ''' <summary>
    ''' Gets the method represented by the lambda expression.
    ''' </summary>
    ''' <exception cref="ArgumentNullException">The <paramref name="method"/> is null.</exception>
    ''' <exception cref="ArgumentException">The <paramref name="method"/> is not a lambda expression or it does not represent a method invocation.</exception>
    Public Shared Function GetMethod(ByVal method As Expression(Of Action(Of TTarget))) As MethodInfo
      Return GetMethodInfo(method)
    End Function

    ''' <summary>
    ''' Gets the method represented by the lambda expression.
    ''' </summary>
    ''' <exception cref="ArgumentNullException">The <paramref name="method"/> is null.</exception>
    ''' <exception cref="ArgumentException">The <paramref name="method"/> is not a lambda expression or it does not represent a method invocation.</exception>
    Public Shared Function GetMethod(Of T1)(ByVal method As Expression(Of Action(Of TTarget, T1))) As MethodInfo
      Return GetMethodInfo(method)
    End Function

    ''' <summary>
    ''' Gets the method represented by the lambda expression.
    ''' </summary>
    ''' <exception cref="ArgumentNullException">The <paramref name="method"/> is null.</exception>
    ''' <exception cref="ArgumentException">The <paramref name="method"/> is not a lambda expression or it does not represent a method invocation.</exception>
    Public Shared Function GetMethod(Of T1, T2)(ByVal method As Expression(Of Action(Of TTarget, T1, T2))) As MethodInfo
      Return GetMethodInfo(method)
    End Function

    ''' <summary>
    ''' Gets the method represented by the lambda expression.
    ''' </summary>
    ''' <exception cref="ArgumentNullException">The <paramref name="method"/> is null.</exception>
    ''' <exception cref="ArgumentException">The <paramref name="method"/> is not a lambda expression or it does not represent a method invocation.</exception>
    Public Shared Function GetMethod(Of T1, T2, T3)(ByVal method As Expression(Of Action(Of TTarget, T1, T2, T3))) As MethodInfo
      Return GetMethodInfo(method)
    End Function

    Private Shared Function GetMethodInfo(ByVal method As Expression) As MethodInfo
      If method Is Nothing Then
        Throw New ArgumentNullException("method")
      End If

      Dim lambda As LambdaExpression = CType(method, LambdaExpression)
      If lambda Is Nothing Then
        Throw New ArgumentException("Not a lambda expression", "method")
      End If

      If lambda.Body.NodeType <> ExpressionType.Call Then
        Throw New ArgumentException("Not a method call", "method")
      End If

      Return CType(lambda.Body, MethodCallExpression).Method
    End Function

    ''' <summary>
    ''' Gets the property represented by the lambda expression.
    ''' </summary>
    ''' <typeparam name="P">Type assigned to the property</typeparam>
    ''' <param name="property">Property Expression</param>
    ''' <returns></returns>
    ''' <exception cref="ArgumentNullException">The <paramref name="method"/> is null.</exception>
    ''' <exception cref="ArgumentException">The <paramref name="method"/> is not a lambda expression or it does not represent a property access.</exception>
    Public Shared Function GetProperty(Of P)(ByVal [property] As Expression(Of Func(Of TTarget, P))) As PropertyInfo
      Dim info As PropertyInfo = CType(GetMemberInfo([property]), PropertyInfo)
      If info Is Nothing Then
        Throw New ArgumentException("Member is not a property")
      End If

      Return info
    End Function

    ''' <summary>
    ''' Gets the field represented by the lambda expression.
    ''' </summary>
    ''' <exception cref="ArgumentNullException">The <paramref name="method"/> is null.</exception>
    ''' <exception cref="ArgumentException">The <paramref name="method"/> is not a lambda expression or it does not represent a field access.</exception>
    Public Shared Function GetField(ByVal field As Expression(Of Func(Of TTarget, Object))) As FieldInfo
      Dim info As FieldInfo = CType(GetMemberInfo(field), FieldInfo)
      If info Is Nothing Then
        Throw New ArgumentException("Member is not a field")
      End If

      Return info
    End Function

    Private Shared Function GetMemberInfo(ByVal member As Expression) As MemberInfo
      If member Is Nothing Then
        Throw New ArgumentNullException("member")
      End If

      Dim lambda As LambdaExpression = CType(member, LambdaExpression)
      If lambda Is Nothing Then
        Throw New ArgumentException("Not a lambda expression", "member")
      End If

      Dim memberExpr As MemberExpression = Nothing

      'The Func<TTarget, object> we use returns an object, so first statement can be either 
      'a cast (if the field/property does not return an object) or the direct member access.
      If lambda.Body.NodeType = ExpressionType.Convert Then

        'The cast is an unary expression, where the operand is the 
        'actual member access expression.
        memberExpr = CType(CType(lambda.Body, UnaryExpression).Operand, MemberExpression)
      ElseIf lambda.Body.NodeType = ExpressionType.MemberAccess Then
        memberExpr = CType(lambda.Body, MemberExpression)
      End If

      If memberExpr Is Nothing Then
        Throw New ArgumentException("Not a member access", "member")
      End If

      Return memberExpr.Member

    End Function

  End Class

End Namespace

