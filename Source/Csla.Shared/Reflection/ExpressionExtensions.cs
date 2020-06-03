//-----------------------------------------------------------------------
// <copyright file="ExpressionExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Extension methods for Expression types</summary>
//-----------------------------------------------------------------------
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Csla.Reflection
{
  /// <summary>
  /// Extension methods for Expression types.
  /// </summary>
  public static class ExpressionExtensions
  {
    /// <summary>
    /// Gets a string key name value based
    /// on an expression.
    /// </summary>
    /// <typeparam name="T">Type of value</typeparam>
    /// <param name="expression">Expression</param>
    /// <returns></returns>
    public static string GetKey<T>(this Expression<T> expression)
    {
      var list = new List<string>();
      var member = expression.Body as MemberExpression;
      while (member != null)
      {
        list.Insert(0, member.Member.Name);
        member = member.Expression as MemberExpression;
      }
      return string.Join(".", list);
    }
  }
}
