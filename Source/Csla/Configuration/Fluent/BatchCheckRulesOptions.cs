//-----------------------------------------------------------------------
// <copyright file="BatchCheckRulesOptions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Options for the default (batch) CheckRules() behavior</summary>
//-----------------------------------------------------------------------

namespace Csla.Configuration
{
  /// <summary>
  /// Options for the default (batch) BusinessRules.CheckRules() behavior.
  /// See <see cref="RuleOptions.UseBatchCheckRules"/>.
  /// </summary>
  public class BatchCheckRulesOptions
  {
    private int _maxRerunPasses = 100;

    /// <summary>
    /// Gets or sets the number of rerun passes CheckRules() may make when
    /// CascadeOnDirtyProperties is true and rule output values keep changing
    /// property values. Default is 100.
    /// </summary>
    /// <remarks>
    /// When the limit is reached, CheckRules() throws an
    /// <see cref="InvalidOperationException"/> naming the properties that
    /// were still changing.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">The value is less than 1.</exception>
    public int MaxRerunPasses
    {
      get => _maxRerunPasses;
      set
      {
        if (value < 1)
          throw new ArgumentOutOfRangeException(nameof(value), value, $"{nameof(MaxRerunPasses)} must be at least 1.");
        _maxRerunPasses = value;
      }
    }
  }
}
