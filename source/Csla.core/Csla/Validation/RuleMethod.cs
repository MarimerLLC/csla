using System;

#if SILVERLIGHT
using Uri = Csla.Utilities;
#endif

namespace Csla.Validation
{
  /// <summary>
  /// Tracks all information for a rule.
  /// </summary>
  internal class RuleMethod : IRuleMethod, IComparable, IComparable<IRuleMethod>
  {
    private RuleHandler _handler;
    private string _ruleName = String.Empty;
    private RuleArgs _args;
    private int _priority;

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
    /// Gets the priority of the rule method.
    /// </summary>
    /// <value>The priority value</value>
    /// <remarks>
    /// Priorities are processed in descending
    /// order, so priority 0 is processed
    /// before priority 1, etc.
    /// </remarks>
    public int Priority
    {
      get { return _priority; }
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
    /// Returns the name of the field, property or column
    /// to which the rule applies.
    /// </summary>
    public RuleArgs RuleArgs
    {
      get { return _args; }
    }

    /// <summary>
    /// Creates and initializes the rule.
    /// </summary>
    /// <param name="handler">The address of the method implementing the rule.</param>
    /// <param name="args">A RuleArgs object.</param>
    public RuleMethod(RuleHandler handler, RuleArgs args)
    {
      _handler = handler;
      _args = args;
      _ruleName = string.Format(@"rule://{0}/{1}/{2}", 
        Uri.EscapeDataString(_handler.Method.DeclaringType.FullName),
        _handler.Method.Name, 
        _args.ToString());
    }

    /// <summary>
    /// Creates and initializes the rule.
    /// </summary>
    /// <param name="handler">The address of the method implementing the rule.</param>
    /// <param name="args">A RuleArgs object.</param>
    /// <param name="priority">
    /// Priority for processing the rule (smaller numbers have higher priority, default=0).
    /// </param>
    public RuleMethod(RuleHandler handler, RuleArgs args, int priority)
      : this(handler, args)
    {
      _priority = priority;
    }

    /// <summary>
    /// Invokes the rule to validate the data.
    /// </summary>
    /// <returns>
    /// <see langword="true" /> if the data is valid, 
    /// <see langword="false" /> if the data is invalid.
    /// </returns>
    public bool Invoke(object target)
    {
      return _handler.Invoke(target, _args);
    }

    #region IComparable

    int IComparable.CompareTo(object obj)
    {
      return Priority.CompareTo(((IRuleMethod)obj).Priority);
    }

    int IComparable<IRuleMethod>.CompareTo(IRuleMethod other)
    {
      return Priority.CompareTo(other.Priority);
    }

    #endregion

  }


  /// <summary>
  /// Tracks all information for a rule.
  /// </summary>
  /// <typeparam name="T">Type of the target object.</typeparam>
  /// <typeparam name="R">Type of the arguments parameter.</typeparam>
  internal class RuleMethod<T, R> 
    : IRuleMethod, IComparable, IComparable<IRuleMethod> 
    where R : RuleArgs
  {
    private RuleHandler<T, R> _handler;
    private string _ruleName = string.Empty;
    private R _args;
    private int _priority;

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
    /// Gets the priority of the rule method.
    /// </summary>
    /// <value>The priority value</value>
    /// <remarks>
    /// Priorities are processed in descending
    /// order, so priority 0 is processed
    /// before priority 1, etc.
    /// </remarks>
    public int Priority
    {
      get { return _priority; }
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
      get {return _ruleName;}
    }

    /// <summary>
    /// Returns the name of the field, property or column
    /// to which the rule applies.
    /// </summary>
    RuleArgs IRuleMethod.RuleArgs
    {
      get {return this.RuleArgs;}
    }

    /// <summary>
    /// Returns the name of the field, property or column
    /// to which the rule applies.
    /// </summary>
    public R RuleArgs
    {
      get { return _args; }
    }

    /// <summary>
    /// Creates and initializes the rule.
    /// </summary>
    /// <param name="handler">The address of the method implementing the rule.</param>
    /// <param name="args">A RuleArgs object.</param>
    public RuleMethod(RuleHandler<T, R> handler, R args)
    {
      _handler = handler;
      _args = args;
      _ruleName = string.Format(@"rule://{0}/{1}/{2}",
        Uri.EscapeDataString(_handler.Method.DeclaringType.FullName),
        _handler.Method.Name,
        _args.ToString());
    }

    /// <summary>
    /// Creates and initializes the rule.
    /// </summary>
    /// <param name="handler">The address of the method implementing the rule.</param>
    /// <param name="args">A RuleArgs object.</param>
    /// <param name="priority">
    /// Priority for processing the rule (smaller numbers have higher priority, default=0).
    /// </param>
    public RuleMethod(RuleHandler<T, R> handler, R args, int priority)
      : this(handler, args)
    {
      _priority = priority;
    }

    /// <summary>
    /// Invokes the rule to validate the data.
    /// </summary>
    /// <returns>True if the data is valid, False if the data is invalid.</returns>
    bool IRuleMethod.Invoke(object target)
    {
      return this.Invoke((T)target);
    }

    /// <summary>
    /// Invokes the rule to validate the data.
    /// </summary>
    /// <returns>
    /// <see langword="true" /> if the data is valid, 
    /// <see langword="false" /> if the data is invalid.
    /// </returns>
    public bool Invoke(T target)
    {
      return _handler.Invoke(target, _args);
    }

    #region IComparable

    int IComparable.CompareTo(object obj)
    {
      return Priority.CompareTo(((IRuleMethod)obj).Priority);
    }

    int IComparable<IRuleMethod>.CompareTo(IRuleMethod other)
    {
      return Priority.CompareTo(other.Priority);
    }

    #endregion

  }
}
