using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Csla.Core
{
  internal class InnermostWhereFinder : ExpressionVisitor
  {
    private MethodCallExpression innermostWhereExpression;

    public MethodCallExpression GetInnermostWhere(Expression expression)
    {
      Visit(expression);
      return innermostWhereExpression;
    }

    protected override Expression VisitMethodCall(MethodCallExpression expression)
    {
      if (expression.Method.Name == "Where")
        innermostWhereExpression = expression;
      //{
      //  innermostWhereExpression = expression;
      //  return expression;
      //}

      Visit(expression.Arguments[0]);

      return expression;
    }
  }
}
