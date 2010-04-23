using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Rules
{
  /// <summary>
  /// Defines the interaction between the rules engine and
  /// a business object that hosts the rules.
  /// </summary>
  public interface IHostRules
  {
    /// <summary>
    /// Indicates that a rule has started processing.
    /// </summary>
    /// <param name="property">Property for rule.</param>
    void RuleStart(Csla.Core.IPropertyInfo property);
    /// <summary>
    /// Indicates that a rule has finished processing.
    /// </summary>
    /// <param name="property">Property for rule.</param>
    void RuleComplete(Csla.Core.IPropertyInfo property);
    /// <summary>
    /// Indicates that all rules have finished processing.
    /// </summary>
    void AllRulesComplete();
  }
}
