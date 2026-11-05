//-----------------------------------------------------------------------
// <copyright file="RuleOptions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Options for business rules</summary>
//-----------------------------------------------------------------------

namespace Csla.Configuration
{
  /// <summary>
  /// Options for business rules.
  /// </summary>
  public class RuleOptions
  {
    private int _maxCheckRulesPasses = 100;

    /// <summary>
    /// Gets or sets a value indicating whether BusinessRules.CheckRules()
    /// runs rules the way CSLA 10 and earlier did. Default is false.
    /// </summary>
    /// <remarks>
    /// <para>
    /// By default, CheckRules() runs every property rule once, in priority
    /// order across all properties, and then reruns only the rules whose
    /// property or input values were changed by rule output values.
    /// </para>
    /// <para>
    /// When true, CheckRules() runs each property's rules in turn (priority
    /// only applies within a property), recursively cascading to affected
    /// properties and to rules that use the property as an input. This
    /// option is temporary and will be removed in a future version.
    /// </para>
    /// </remarks>
    public bool UseLegacyCheckRules { get; set; }

    /// <summary>
    /// Gets or sets the number of rerun passes BusinessRules.CheckRules()
    /// may make when CascadeOnDirtyProperties is true and rule output values
    /// keep changing property values. Default is 100.
    /// </summary>
    /// <remarks>
    /// When the limit is reached, CheckRules() throws an
    /// <see cref="InvalidOperationException"/> naming the properties that
    /// were still changing. Does not apply when <see cref="UseLegacyCheckRules"/>
    /// is true.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">The value is less than 1.</exception>
    public int MaxCheckRulesPasses
    {
      get => _maxCheckRulesPasses;
      set
      {
        if (value < 1)
          throw new ArgumentOutOfRangeException(nameof(value), value, $"{nameof(MaxCheckRulesPasses)} must be at least 1.");
        _maxCheckRulesPasses = value;
      }
    }
  }
}
