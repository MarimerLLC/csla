Imports System.Linq.Expressions

Namespace Core

  Friend Class ThenByBuilder
    Inherits ExpressionVisitor

    Private exprList As New List(Of Expression)

    Public Function GetThenByExpressions(ByVal expression As Expression) As List(Of Expression)
      Visit(expression)
      Return exprList
    End Function

    Protected Overrides Function VisitMethodCall(ByVal expression As MethodCallExpression) As Expression

      If expression.Method.Name.StartsWith("ThenBy") Then
        exprList.Insert(0, expression)
        Visit(expression.Arguments(0))
      End If

      Return expression
    End Function

  End Class

End Namespace

