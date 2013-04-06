using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Core;

#if SILVERLIGHT
using Uri = Csla.Utilities;
#endif

namespace Csla.Validation
{
  internal class AsyncRuleMethod : IAsyncRuleMethod, IComparable, IComparable<IRuleMethod>
  {
    private AsyncRuleHandler _handler;
    private AsyncRuleArgs _args;
    private string _ruleName = String.Empty;
    private RuleSeverity _severity = RuleSeverity.Error;
    private int _priority = 0;

    /// <summary>
    /// Returns the name of the method implementing the rule
    /// and the property, field or column name to which the
    /// rule applies.
    /// </summary>
    public override string ToString()
    {
      return _ruleName;
    }

    /// <summary>
    /// Gets the name of the rule.
    /// </summary>
    /// <remarks>
    /// The rule's name must be unique and is used
    /// to identify a broken rule in the BrokenRules
    /// collection.
    /// </remarks>
    public string RuleName
    {
      get { return _ruleName; }
    }

    /// <summary>
    /// Create a temporary args object that
    /// just contains the property name. This
    /// should only occur if the async rule
    /// method throws an unhandled exception,
    /// which shouldn't really ever happen.
    /// </summary>
    RuleArgs IRuleMethod.RuleArgs
    {
      get { return new RuleArgs(_args.Properties[0].Name); }
    }

    public AsyncRuleArgs AsyncRuleArgs
    {
      get { return _args; }
    }

    int IRuleMethod.Priority
    {
      get { return _priority; }
    }

    public RuleSeverity Severity { get { return _severity; } }

    /// <summary>
    /// Creates and initializes the rule.
    /// </summary>
    /// <param name="handler">The address of the method implementing the rule.</param>
    /// <param name="args">A RuleArgs object.</param>
    /// <param name="severity">Severity of the rule.</param>
    public AsyncRuleMethod(AsyncRuleHandler handler, AsyncRuleArgs args, RuleSeverity severity)
    {
      _handler = handler;
      _args = args;
      _severity = severity;
      _ruleName = string.Format(@"rule://{0}/{1}/{2}",
        UriUtilities.FormatHostName(_handler.Method.DeclaringType), 
        UriUtilities.EncodeString(_handler.Method.Name), 
        _args.Properties[0].Name);
    }

    /// <summary>
    /// Creates and initializes the rule.
    /// </summary>
    /// <param name="handler">The address of the method implementing the rule.</param>
    /// <param name="args">A RuleArgs object.</param>
    /// <param name="priority">Priority of the rule.</param>
    public AsyncRuleMethod(AsyncRuleHandler handler, AsyncRuleArgs args, int priority)
    {
      _handler = handler;
      _args = args;
      _priority = priority;
      _ruleName = string.Format(@"rule://{0}/{1}/{2}",
        UriUtilities.FormatHostName(_handler.Method.DeclaringType),
        UriUtilities.EncodeString(_handler.Method.Name),
        _args.Properties[0].Name);
    }

    /// <summary>
    /// You must call the IAsyncRuleMethod overload of Invoke.
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    bool IRuleMethod.Invoke(object target)
    {
      throw new NotSupportedException();
    }

    /// <summary>
    /// Invokes the asynchronous rule to validate the data.
    /// </summary>
    /// <returns>
    /// <see langword="true" /> if the data is valid, 
    /// <see langword="false" /> if the data is invalid.
    /// </returns>
    public void Invoke(object target, AsyncRuleCompleteHandler result)
    {
      Dictionary<string, object> propertyValues = GetPropertyValues(target, _args.Properties);

      _handler.Invoke(new AsyncValidationRuleContext(
        propertyValues, 
        _args, 
        new AsyncRuleResult(this),
        r => result(this, r)));
    }

    #region IComparable

    int IComparable.CompareTo(object obj)
    {
      return _priority.CompareTo(((IRuleMethod)obj).Priority);
    }

    int IComparable<IRuleMethod>.CompareTo(IRuleMethod other)
    {
      return _priority.CompareTo(other.Priority);
    }

    #endregion

    private static Dictionary<string, object> GetPropertyValues(object target, params IPropertyInfo[] properties)
    {
      Dictionary<string, object> propertyValues = new Dictionary<string, object>();
      foreach (IPropertyInfo property in properties)
        propertyValues.Add(property.Name, Utilities.CallByName(target, property.Name, CallType.Get));

      return propertyValues;
    }
  }
}
