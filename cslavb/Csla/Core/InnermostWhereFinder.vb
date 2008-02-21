Imports System.Linq.Expressions

Namespace Core

  Friend Class InnermostWhereFinder
    Inherits ExpressionVisitor

    Private _innermostWhereExpression As MethodCallExpression

    Public Function GetInnermostWhere(ByVal expression As Expression) As MethodCallExpression
      Visit(expression)
      Return _innermostWhereExpression
    End Function

    Protected Overrides Function VisitMethodCall(ByVal expression As MethodCallExpression) As Expression
      If expression.Method.Name = "Where" Then
        _innermostWhereExpression = expression
      End If
      '{
      '  innermostWhereExpression = expression;
      '  return expression;
      '}

      Visit(expression.Arguments(0))

      Return expression
    End Function

  End Class

End Namespace