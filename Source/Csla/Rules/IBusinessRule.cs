using System.Collections.Generic;

namespace Csla.Rules
{
  /// <summary>
  /// Interface defining a business/validation
  /// rule implementation.
  /// </summary>
  public interface IBusinessRule
  {
    /// <summary>
    /// Business or validation rule implementation.
    /// </summary>
    /// <param name="context">Rule context object.</param>
    void Rule(RuleContext context);
    /// <summary>
    /// Gets a list of secondary property values to be supplied to the
    /// rule when it is executed.
    /// </summary>
    List<Csla.Core.IPropertyInfo> InputProperties { get; }
    /// <summary>
    /// Gets a value indicating whether the rule will run
    /// on a background thread.
    /// </summary>
    bool IsAsync { get; }
  }
}
