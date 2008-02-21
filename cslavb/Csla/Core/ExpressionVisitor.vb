Imports System.Collections.ObjectModel
Imports System.Linq.Expressions

Namespace Core
  Friend MustInherit Class ExpressionVisitor
    Protected Sub New()
    End Sub

    Protected Overridable Function Visit(ByVal exp As Expression) As Expression
      If exp Is Nothing Then
        Return exp
      End If
      Select Case exp.NodeType
        Case ExpressionType.Negate, ExpressionType.NegateChecked, ExpressionType.Not, ExpressionType.Convert, ExpressionType.ConvertChecked, ExpressionType.ArrayLength, ExpressionType.Quote, ExpressionType.TypeAs
          Return Me.VisitUnary(CType(exp, UnaryExpression))
        Case ExpressionType.Add, ExpressionType.AddChecked, ExpressionType.Subtract, ExpressionType.SubtractChecked, ExpressionType.Multiply, ExpressionType.MultiplyChecked, ExpressionType.Divide, ExpressionType.Modulo, ExpressionType.And, ExpressionType.AndAlso, ExpressionType.Or, ExpressionType.OrElse, ExpressionType.LessThan, ExpressionType.LessThanOrEqual, ExpressionType.GreaterThan, ExpressionType.GreaterThanOrEqual, ExpressionType.Equal, ExpressionType.NotEqual, ExpressionType.Coalesce, ExpressionType.ArrayIndex, ExpressionType.RightShift, ExpressionType.LeftShift, ExpressionType.ExclusiveOr
          Return Me.VisitBinary(CType(exp, BinaryExpression))
        Case ExpressionType.TypeIs
          Return Me.VisitTypeIs(CType(exp, TypeBinaryExpression))
        Case ExpressionType.Conditional
          Return Me.VisitConditional(CType(exp, ConditionalExpression))
        Case ExpressionType.Constant
          Return Me.VisitConstant(CType(exp, ConstantExpression))
        Case ExpressionType.Parameter
          Return Me.VisitParameter(CType(exp, ParameterExpression))
        Case ExpressionType.MemberAccess
          Return Me.VisitMemberAccess(CType(exp, MemberExpression))
        Case ExpressionType.Call
          Return Me.VisitMethodCall(CType(exp, MethodCallExpression))
        Case ExpressionType.Lambda
          Return Me.VisitLambda(CType(exp, LambdaExpression))
        Case ExpressionType.New
          Return Me.VisitNew(CType(exp, NewExpression))
        Case ExpressionType.NewArrayInit, ExpressionType.NewArrayBounds
          Return Me.VisitNewArray(CType(exp, NewArrayExpression))
        Case ExpressionType.Invoke
          Return Me.VisitInvocation(CType(exp, InvocationExpression))
        Case ExpressionType.MemberInit
          Return Me.VisitMemberInit(CType(exp, MemberInitExpression))
        Case ExpressionType.ListInit
          Return Me.VisitListInit(CType(exp, ListInitExpression))
        Case Else
          Throw New Exception(String.Format("Unhandled expression type: '{0}'", exp.NodeType))
      End Select
    End Function

    Protected Overridable Function VisitBinding(ByVal binding As MemberBinding) As MemberBinding
      Select Case binding.BindingType
        Case MemberBindingType.Assignment
          Return Me.VisitMemberAssignment(CType(binding, MemberAssignment))
        Case MemberBindingType.MemberBinding
          Return Me.VisitMemberMemberBinding(CType(binding, MemberMemberBinding))
        Case MemberBindingType.ListBinding
          Return Me.VisitMemberListBinding(CType(binding, MemberListBinding))
        Case Else
          Throw New Exception(String.Format("Unhandled binding type '{0}'", binding.BindingType))
      End Select
    End Function

    Protected Overridable Function VisitElementInitializer(ByVal initializer As ElementInit) As ElementInit
      Dim arguments As ReadOnlyCollection(Of Expression) = Me.VisitExpressionList(initializer.Arguments)
      If arguments IsNot initializer.Arguments Then
        Return Expression.ElementInit(initializer.AddMethod, arguments)
      End If
      Return initializer
    End Function

    Protected Overridable Function VisitUnary(ByVal u As UnaryExpression) As Expression
      Dim operand As Expression = Me.Visit(u.Operand)
      If operand IsNot u.Operand Then
        Return Expression.MakeUnary(u.NodeType, operand, u.Type, u.Method)
      End If
      Return u
    End Function

    Protected Overridable Function VisitBinary(ByVal b As BinaryExpression) As Expression
      Dim left As Expression = Me.Visit(b.Left)
      Dim right As Expression = Me.Visit(b.Right)
      Dim conversion As Expression = Me.Visit(b.Conversion)
      If left IsNot b.Left OrElse right IsNot b.Right OrElse conversion IsNot b.Conversion Then
        If b.NodeType = ExpressionType.Coalesce AndAlso b.Conversion IsNot Nothing Then
          Return Expression.Coalesce(left, right, TryCast(conversion, LambdaExpression))
        Else
          Return Expression.MakeBinary(b.NodeType, left, right, b.IsLiftedToNull, b.Method)
        End If
      End If
      Return b
    End Function

    Protected Overridable Function VisitTypeIs(ByVal b As TypeBinaryExpression) As Expression
      Dim expr As Expression = Me.Visit(b.Expression)
      If expr IsNot b.Expression Then
        Return Expression.TypeIs(expr, b.TypeOperand)
      End If
      Return b
    End Function

    Protected Overridable Function VisitConstant(ByVal c As ConstantExpression) As Expression
      Return c
    End Function

    Protected Overridable Function VisitConditional(ByVal c As ConditionalExpression) As Expression
      Dim test As Expression = Me.Visit(c.Test)
      Dim ifTrue As Expression = Me.Visit(c.IfTrue)
      Dim ifFalse As Expression = Me.Visit(c.IfFalse)
      If test IsNot c.Test OrElse ifTrue IsNot c.IfTrue OrElse ifFalse IsNot c.IfFalse Then
        Return Expression.Condition(test, ifTrue, ifFalse)
      End If
      Return c
    End Function

    Protected Overridable Function VisitParameter(ByVal p As ParameterExpression) As Expression
      Return p
    End Function

    Protected Overridable Function VisitMemberAccess(ByVal m As MemberExpression) As Expression
      Dim exp As Expression = Me.Visit(m.Expression)
      If exp IsNot m.Expression Then
        Return Expression.MakeMemberAccess(exp, m.Member)
      End If
      Return m
    End Function

    Protected Overridable Function VisitMethodCall(ByVal m As MethodCallExpression) As Expression
      Dim obj As Expression = Me.Visit(m.Object)
      Dim args As IEnumerable(Of Expression) = Me.VisitExpressionList(m.Arguments)
      If obj IsNot m.Object OrElse args IsNot m.Arguments Then
        Return Expression.Call(obj, m.Method, args)
      End If
      Return m
    End Function

    Protected Overridable Function VisitExpressionList(ByVal original As ReadOnlyCollection(Of Expression)) As ReadOnlyCollection(Of Expression)
      Dim list As List(Of Expression) = Nothing
      Dim i As Integer = 0
      Dim n As Integer = original.Count
      Do While i < n
        Dim p As Expression = Me.Visit(original(i))
        If list IsNot Nothing Then
          list.Add(p)
        ElseIf p IsNot original(i) Then
          list = New List(Of Expression)(n)
          For j As Integer = 0 To i - 1
            list.Add(original(j))
          Next j
          list.Add(p)
        End If
        i += 1
      Loop
      If list IsNot Nothing Then
        Return list.AsReadOnly()
      End If
      Return original
    End Function

    Protected Overridable Function VisitMemberAssignment(ByVal assignment As MemberAssignment) As MemberAssignment
      Dim e As Expression = Me.Visit(assignment.Expression)
      If e IsNot assignment.Expression Then
        Return Expression.Bind(assignment.Member, e)
      End If
      Return assignment
    End Function

    Protected Overridable Function VisitMemberMemberBinding(ByVal binding As MemberMemberBinding) As MemberMemberBinding
      Dim bindings As IEnumerable(Of MemberBinding) = Me.VisitBindingList(binding.Bindings)
      If bindings IsNot binding.Bindings Then
        Return Expression.MemberBind(binding.Member, bindings)
      End If
      Return binding
    End Function

    Protected Overridable Function VisitMemberListBinding(ByVal binding As MemberListBinding) As MemberListBinding
      Dim initializers As IEnumerable(Of ElementInit) = Me.VisitElementInitializerList(binding.Initializers)
      If initializers IsNot binding.Initializers Then
        Return Expression.ListBind(binding.Member, initializers)
      End If
      Return binding
    End Function

    Protected Overridable Function VisitBindingList(ByVal original As ReadOnlyCollection(Of MemberBinding)) As IEnumerable(Of MemberBinding)
      Dim list As List(Of MemberBinding) = Nothing
      Dim i As Integer = 0
      Dim n As Integer = original.Count
      Do While i < n
        Dim b As MemberBinding = Me.VisitBinding(original(i))
        If list IsNot Nothing Then
          list.Add(b)
        ElseIf b IsNot original(i) Then
          list = New List(Of MemberBinding)(n)
          For j As Integer = 0 To i - 1
            list.Add(original(j))
          Next j
          list.Add(b)
        End If
        i += 1
      Loop
      If list IsNot Nothing Then
        Return list
      End If
      Return original
    End Function

    Protected Overridable Function VisitElementInitializerList(ByVal original As ReadOnlyCollection(Of ElementInit)) As IEnumerable(Of ElementInit)
      Dim list As List(Of ElementInit) = Nothing
      Dim i As Integer = 0
      Dim n As Integer = original.Count
      Do While i < n
        Dim init As ElementInit = Me.VisitElementInitializer(original(i))
        If list IsNot Nothing Then
          list.Add(init)
        ElseIf init IsNot original(i) Then
          list = New List(Of ElementInit)(n)
          For j As Integer = 0 To i - 1
            list.Add(original(j))
          Next j
          list.Add(init)
        End If
        i += 1
      Loop
      If list IsNot Nothing Then
        Return list
      End If
      Return original
    End Function

    Protected Overridable Function VisitLambda(ByVal lambda As LambdaExpression) As Expression
      Dim body As Expression = Me.Visit(lambda.Body)
      If body IsNot lambda.Body Then
        Return Expression.Lambda(lambda.Type, body, lambda.Parameters)
      End If
      Return lambda
    End Function

    Protected Overridable Function VisitNew(ByVal nex As NewExpression) As NewExpression
      Dim args As IEnumerable(Of Expression) = Me.VisitExpressionList(nex.Arguments)
      If args IsNot nex.Arguments Then
        If nex.Members IsNot Nothing Then
          Return Expression.[New](nex.Constructor, args, nex.Members)
        Else
          Return Expression.[New](nex.Constructor, args)
        End If
      End If
      Return nex
    End Function

    Protected Overridable Function VisitMemberInit(ByVal init As MemberInitExpression) As Expression
      Dim n As NewExpression = Me.VisitNew(init.NewExpression)
      Dim bindings As IEnumerable(Of MemberBinding) = Me.VisitBindingList(init.Bindings)
      If n IsNot init.NewExpression OrElse bindings IsNot init.Bindings Then
        Return Expression.MemberInit(n, bindings)
      End If
      Return init
    End Function

    Protected Overridable Function VisitListInit(ByVal init As ListInitExpression) As Expression
      Dim n As NewExpression = Me.VisitNew(init.NewExpression)
      Dim initializers As IEnumerable(Of ElementInit) = Me.VisitElementInitializerList(init.Initializers)
      If n IsNot init.NewExpression OrElse initializers IsNot init.Initializers Then
        Return Expression.ListInit(n, initializers)
      End If
      Return init
    End Function

    Protected Overridable Function VisitNewArray(ByVal na As NewArrayExpression) As Expression
      Dim exprs As IEnumerable(Of Expression) = Me.VisitExpressionList(na.Expressions)
      If exprs IsNot na.Expressions Then
        If na.NodeType = ExpressionType.NewArrayInit Then
          Return Expression.NewArrayInit(na.Type.GetElementType(), exprs)
        Else
          Return Expression.NewArrayBounds(na.Type.GetElementType(), exprs)
        End If
      End If
      Return na
    End Function

    Protected Overridable Function VisitInvocation(ByVal iv As InvocationExpression) As Expression
      Dim args As IEnumerable(Of Expression) = Me.VisitExpressionList(iv.Arguments)
      Dim expr As Expression = Me.Visit(iv.Expression)
      If args IsNot iv.Arguments OrElse expr IsNot iv.Expression Then
        Return Expression.Invoke(expr, args)
      End If
      Return iv
    End Function
  End Class
End Namespace