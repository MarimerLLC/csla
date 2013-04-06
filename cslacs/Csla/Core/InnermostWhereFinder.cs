using System;
using System.Linq.Expressions;
using System.Collections.Generic;

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
     
      Visit(expression.Arguments[0]);

      return expression;
    }
  }

  internal class InnermostOrderByFinder : ExpressionVisitor
  {
    private MethodCallExpression innermostOrderByExpression;

    public MethodCallExpression GetInnermostOrderBy(Expression expression)
    {
      Visit(expression);
      return innermostOrderByExpression;
    }

    protected override Expression VisitMethodCall(MethodCallExpression expression)
    {
      if (expression.Method.Name.StartsWith("OrderBy"))
        innermostOrderByExpression = expression;
     
      Visit(expression.Arguments[0]);

      return expression;
    }
  }

  internal class ThenByBuilder : ExpressionVisitor
  {
    private List<Expression> exprList = new List<Expression>();

    public List<Expression> GetThenByExpressions(Expression expression)
    {
      Visit(expression);
      return exprList;
    }

    //JF 3/19/09 - By recursively calling Visit on the 0 Argument index the expression is parsed completely.
    //Simply insert each piece of the Then By expressions into the 0 index position as your recurse through this method.
    //When it is done the Then By expressions are in the exprList in the right order.
    //Full expression is:  .Where( => (.Lnno = 1)).OrderBy( => .Lnno).ThenBy( => .Imcode).ThenBy( => .Costcode).ThenBy( => .Acctcode)
    //On 2nd pass exp is:  .Where( => (.Lnno = 1)).OrderBy( => .Lnno).ThenBy( => .Imcode).ThenBy( => .Costcode)
    //On 3rd pass exp is:  .Where( => (.Lnno = 1)).OrderBy( => .Lnno).ThenBy( => .Imcode)
    //Only call Visit while the method name starts with "ThenBy" to short circuit the recursion.
    //Assumes that ThenBy is the final method name in the list. Is that always true?
    //By inserting to index 0 each time the final list is in the right order.
    protected override Expression VisitMethodCall(MethodCallExpression expression)
    {
      if (expression.Method.Name.StartsWith("ThenBy"))
      {
        exprList.Insert(0, expression);
        Visit(expression.Arguments[0]);
      }
      return expression;
    }
  }
}
