using System;

namespace Csla.Validation
{
  /// <summary>
  /// Delegate that defines the method signature for all rule handler methods.
  /// </summary>
  /// <param name="target">
  /// Object containing the data to be validated.
  /// </param>
  /// <param name="e">
  /// Parameter used to pass information to and from
  /// the rule method.
  /// </param>
  /// <returns>
  /// <see langword="true" /> if the rule was satisfied.
  /// </returns>
  /// <remarks>
  /// <para>
  /// When implementing a rule handler, you must conform to the method signature
  /// defined by this delegate. You should also apply the Description attribute
  /// to your method to provide a meaningful description for your rule.
  /// </para><para>
  /// The method implementing the rule must return 
  /// <see langword="true"/> if the data is valid and
  /// return <see langword="false"/> if the data is invalid.
  /// </para>
  /// </remarks>
  public delegate bool RuleHandler(object target, RuleArgs e);

  /// <summary>
  /// Delegate that defines the method signature for all rule handler methods.
  /// </summary>
  /// <typeparam name="T">Type of the target object.</typeparam>
  /// <typeparam name="R">Type of the arguments parameter.</typeparam>
  /// <param name="target">
  /// Object containing the data to be validated.
  /// </param>
  /// <param name="e">
  /// Parameter used to pass information to and from
  /// the rule method.
  /// </param>
  /// <returns>
  /// <see langword="true" /> if the rule was satisfied.
  /// </returns>
  /// <remarks>
  /// <para>
  /// When implementing a rule handler, you must conform to the method signature
  /// defined by this delegate. You should also apply the Description attribute
  /// to your method to provide a meaningful description for your rule.
  /// </para><para>
  /// The method implementing the rule must return 
  /// <see langword="true"/> if the data is valid and
  /// return <see langword="false"/> if the data is invalid.
  /// </para>
  /// </remarks>
  public delegate bool RuleHandler<T, R>(T target, R e) where R : RuleArgs;

}
