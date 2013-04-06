using System;
using System.Linq.Expressions;
using Csla.Properties;

namespace Csla.Core
{
  internal class ExpressionTreeHelpers
  {
    internal static bool IsMemberEqualsValueExpression(Expression exp, Type declaringType, string memberName)
    {
      if (exp.NodeType != ExpressionType.Equal)
        return false;

      BinaryExpression be = (BinaryExpression)exp;

      // Assert.
      if (ExpressionTreeHelpers.IsSpecificMemberExpression(be.Left, declaringType, memberName) &&
          ExpressionTreeHelpers.IsSpecificMemberExpression(be.Right, declaringType, memberName))
        throw new InvalidOperationException(Resources.CannotHaveMemberEqualsMemberInAnExpression);

      return (ExpressionTreeHelpers.IsSpecificMemberExpression(be.Left, declaringType, memberName) ||
          ExpressionTreeHelpers.IsSpecificMemberExpression(be.Right, declaringType, memberName));
    }

    internal static bool IsSpecificMemberExpression(Expression exp, Type declaringType, string memberName)
    {
      return ((exp is MemberExpression) &&
          (((MemberExpression)exp).Member.DeclaringType == declaringType) &&
          (((MemberExpression)exp).Member.Name == memberName));
    }

    internal static string GetValueFromEqualsExpression(BinaryExpression be, Type memberDeclaringType, string memberName)
    {
      if (be.NodeType != ExpressionType.Equal)
        throw new InvalidProgramException();

      if (be.Left.NodeType == ExpressionType.MemberAccess)
      {
        MemberExpression me = (MemberExpression)be.Left;

        if (me.Member.DeclaringType == memberDeclaringType && me.Member.Name == memberName)
        {
          return GetValueFromExpression(be.Right);
        }
      }
      else if (be.Right.NodeType == ExpressionType.MemberAccess)
      {
        MemberExpression me = (MemberExpression)be.Right;

        if (me.Member.DeclaringType == memberDeclaringType && me.Member.Name == memberName)
        {
          return GetValueFromExpression(be.Left);
        }
      }

      // We should have returned by now.
      throw new InvalidProgramException();
    }

    internal static string GetValueFromExpression(Expression expression)
    {
      if (expression.NodeType == ExpressionType.Constant)
        return (string)(((ConstantExpression)expression).Value);
      else
        throw new InvalidQueryException(
            String.Format(Resources.ExpressionTypeNotSupportedToObtainValue, expression.NodeType));
    }
  }
}
