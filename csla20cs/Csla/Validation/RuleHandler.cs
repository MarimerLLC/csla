using System;

namespace Csla.Validation
{

  /// <summary>
  /// Delegate that defines the method signature for all rule handler methods.
  /// </summary>
  /// <remarks>
  /// <para>
  /// When implementing a rule handler, you must conform to the method signature
  /// defined by this delegate. You should also apply the Description attribute
  /// to your method to provide a meaningful description for your rule.
  /// </para><para>
  /// The method implementing the rule must return True if the data is valid and
  /// return False if the data is invalid.
  /// </para>
  /// </remarks>
  public delegate bool RuleHandler(object target, RuleArgs e);
}
