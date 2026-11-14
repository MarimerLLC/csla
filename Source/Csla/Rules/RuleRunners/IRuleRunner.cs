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
    /// Gets a value indicating whether a stream collects the affected
    /// properties of the async rules it starts, so CheckRulesAsync
    /// returns them.
    /// </summary>
    bool CollectsAsyncResults { get; }

    /// <summary>
    /// Gets the run of a rule stream: CheckRules(), CheckRules(property)
    /// or CheckObjectRules(), as given by <see cref="RuleStream.Kind"/>
    /// and <see cref="RuleStream.Root"/>.
    /// </summary>
    /// <param name="host">The rule runner host.</param>
    /// <param name="stream">
    /// The stream. The run adds the names of the affected properties
    /// with <see cref="RuleStream.AddAffected(IEnumerable{string})"/>.
    /// </param>
    /// <returns>
    /// The run as an iterator. It yields where the stream must wait for
    /// async rules to complete; the host resumes it once they are.
    /// </returns>
    IEnumerable<RuleStreamPause> Run(IRuleRunnerHost host, RuleStream stream);
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
