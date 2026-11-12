//-----------------------------------------------------------------------
// <copyright file="IRuleRunner.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Decides which business rules run and in what order</summary>
//-----------------------------------------------------------------------

using Csla.Core;

namespace Csla.Rules
{
  /// <summary>
  /// Decides which business rules run, in what order, when rules rerun
  /// and when to stop. Runs each rule through an <see cref="IRuleRunnerHost"/>.
  /// </summary>
  /// <remarks>
  /// Implementations must not keep state between calls: one instance
  /// serves every business object.
  /// </remarks>
  internal interface IRuleRunner
  {
    /// <summary>
    /// Runs all rules for the business object (BusinessRules.CheckRules()).
    /// </summary>
    /// <returns>Names of the affected properties. May contain duplicates.</returns>
    List<string> CheckRules(IRuleRunnerHost host);

    /// <summary>
    /// Runs the rules for a property after its value changed
    /// (BusinessRules.CheckRules(property)).
    /// </summary>
    /// <returns>Names of the affected properties. May contain duplicates.</returns>
    List<string> CheckRules(IRuleRunnerHost host, IPropertyInfo property);

    /// <summary>
    /// Runs the object-level rules (BusinessRules.CheckObjectRules()).
    /// </summary>
    /// <returns>Names of the affected properties. May contain duplicates.</returns>
    List<string> CheckObjectRules(IRuleRunnerHost host);
  }

  /// <summary>
  /// The synchronous results of running a list of rules.
  /// </summary>
  internal class RunRulesResult
  {
    public RunRulesResult(List<string> affectedProperties, List<string> dirtyProperties)
    {
      AffectedProperties = affectedProperties;
      DirtyProperties = dirtyProperties;
    }

    public List<string> AffectedProperties { get; }
    public List<string> DirtyProperties { get; }
  }
}
