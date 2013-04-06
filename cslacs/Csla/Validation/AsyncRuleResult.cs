using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Validation
{
  /// <summary>
  /// Object containing the results from 
  /// an asynchronous validation rule method.
  /// </summary>
  public class AsyncRuleResult
  {
    private bool _result = true;
    private string _description;
    private RuleSeverity _severity = RuleSeverity.Error;
    
    /// <summary>
    /// Result value for the validation
    /// rule method, where true indicates
    /// the rule was not violated.
    /// </summary>
    public bool Result
    {
      get { return _result; }
      set { _result = value; }
    }

    /// <summary>
    /// Set by the rule handler method to describe the broken
    /// rule.
    /// </summary>
    /// <value>A human-readable description of
    /// the broken rule.</value>
    /// <remarks>
    /// Setting this property only has an effect if
    /// the rule method returns <see langword="false" />.
    /// </remarks>
    public string Description
    {
      get
      {
        return _description;
      }
      set
      {
        _description = value;
      }
    }

    /// <summary>
    /// Gets or sets the severity of the broken rule.
    /// </summary>
    /// <value>The severity of the broken rule.</value>
    /// <remarks>
    /// Setting this property only has an effect if
    /// the rule method returns <see langword="false" />.
    /// </remarks>
    public RuleSeverity Severity
    {
      get
      {
        return _severity;
      }
      set
      {
        _severity = value;
      }
    }

    internal AsyncRuleResult(IAsyncRuleMethod rule)
    {
      _severity = rule.Severity;
    }

    /// <summary>
    /// Returns a string representation of the object.
    /// </summary>
    public override string ToString()
    {
      return _description;
    }
  }
}
