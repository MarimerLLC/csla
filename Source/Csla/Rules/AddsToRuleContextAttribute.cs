//-----------------------------------------------------------------------
// <copyright file="AddsToRuleContextAttribute.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Indicate that a method adds results or values to a rule context</summary>
//-----------------------------------------------------------------------

namespace Csla.Rules
{
  /// <summary>
  /// Indicates that a method adds results, output values or dirty properties
  /// to the <see cref="IRuleContext"/> it is given, by calling one of the
  /// <c>Add</c> methods of the context, such as <c>AddErrorResult</c> or <c>AddOutValue</c>.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Use this attribute on reusable helper methods called from the <c>Execute</c> or
  /// <c>ExecuteAsync</c> method of a business rule, such as an extension method on
  /// <see cref="IRuleContext"/>, a static helper, or a helper method in a rule base class.
  /// The CSLA0017 analyzer then treats a call to the helper like a call to an <c>Add</c>
  /// method of the rule context.
  /// </para>
  /// <para>
  /// The CSLA0031 analyzer reports a method with this attribute that has no
  /// <see cref="IRuleContext"/> parameter, or that calls neither an <c>Add</c> method of that
  /// context nor another method with this attribute that it passes the context to.
  /// The attribute is not inherited, but CSLA0031 also checks overrides and implementations
  /// of a method that has it, because a call through the base or interface method runs them.
  /// </para>
  /// </remarks>
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
  public sealed class AddsToRuleContextAttribute : Attribute;
}
