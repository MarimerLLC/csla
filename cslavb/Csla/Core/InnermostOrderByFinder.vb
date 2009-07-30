Imports System.Linq.Expressions

Namespace Core

  Friend Class InnermostOrderByFinder
    Inherits ExpressionVisitor

    Private innermostOrderByExpression As MethodCallExpression

    Public Function GetInnermostOrderBy(ByVal expression As Expression) As MethodCallExpression
      Visit(expression)
      Return innermostOrderByExpression
    End Function

    Protected Overrides Function VisitMethodCall(ByVal expression As MethodCallExpression) As Expression

      If expression.Method.Name.StartsWith("OrderBy") Then
        innermostOrderByExpression = expression
      End If

      Visit(expression.Arguments(0))

      Return expression

    End Function

  End Class

End Namespace

