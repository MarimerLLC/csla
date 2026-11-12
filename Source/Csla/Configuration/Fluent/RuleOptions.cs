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
  /// <remarks>
  /// Choose how BusinessRules.CheckRules() runs rules with
  /// <see cref="UseBatchCheckRules"/> (the default) or
  /// <see cref="UseLegacyCheckRules"/>. Each method takes only the settings
  /// that apply to that behavior. If both are called, the last call wins.
  /// </remarks>
  public class RuleOptions
  {
    /// <summary>
    /// Gets a value indicating whether CheckRules() uses the CSLA 10 behavior.
    /// </summary>
    internal bool LegacyCheckRules { get; private set; }

    /// <summary>
    /// Gets the settings for the default (batch) CheckRules() behavior.
    /// </summary>
    internal BatchCheckRulesOptions BatchCheckRules { get; private set; } = new();

    /// <summary>
    /// Makes BusinessRules.CheckRules() run every property rule once, in
    /// priority order across all properties, and then rerun only the rules
    /// whose property or input values were changed by rule output values.
    /// This is the default.
    /// </summary>
    /// <param name="options">Optional action that configures the batch behavior.</param>
    /// <returns>This instance.</returns>
    public RuleOptions UseBatchCheckRules(Action<BatchCheckRulesOptions>? options = null)
    {
      var batchOptions = new BatchCheckRulesOptions();
      options?.Invoke(batchOptions);
      BatchCheckRules = batchOptions;
      LegacyCheckRules = false;
      return this;
    }

    /// <summary>
    /// Makes BusinessRules.CheckRules() run rules the way CSLA 10 and earlier
    /// did: each property's rules in turn (priority only applies within a
    /// property), recursively cascading to affected properties and to rules
    /// that use the property as an input.
    /// </summary>
    /// <remarks>
    /// This option is temporary and will be removed in a future version.
    /// </remarks>
    /// <returns>This instance.</returns>
    public RuleOptions UseLegacyCheckRules()
    {
      LegacyCheckRules = true;
      return this;
    }
  }
}
