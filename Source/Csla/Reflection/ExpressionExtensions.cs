﻿//-----------------------------------------------------------------------
// <copyright file="ExpressionExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Extension methods for Expression types</summary>
//-----------------------------------------------------------------------

using System.Linq.Expressions;

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
    /// <exception cref="ArgumentNullException"><paramref name="expression"/> is <see langword="null"/>.</exception>
    public static string GetKey<T>(this Expression<T> expression)
    {
      if (expression is null)
        throw new ArgumentNullException(nameof(expression));

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
