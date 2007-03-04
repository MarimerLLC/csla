using System;

namespace Csla.Validation
{

  /// <summary>
  /// Object providing extra information to methods that
  /// implement business rules.
  /// </summary>
  public class RuleArgs
  {
    private string _propertyName;
    private string _description;
    private RuleSeverity _severity = RuleSeverity.Error;
    private bool _stopProcessing;

    /// <summary>
    /// The name of the property to be validated.
    /// </summary>
    public string PropertyName
    {
      get { return _propertyName; }
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
      get { return _description; }
      set { _description = value; }
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
      get { return _severity; }
      set { _severity = value; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether this
    /// broken rule should stop the processing of subsequent
    /// rules for this property.
    /// </summary>
    /// <value><see langword="true" /> if no further
    /// rules should be process for this property.</value>
    /// <remarks>
    /// Setting this property only has an effect if
    /// the rule method returns <see langword="false" />.
    /// </remarks>
    public bool StopProcessing
    {
      get { return _stopProcessing; }
      set { _stopProcessing = value; }
    }

    /// <summary>
    /// Creates an instance of RuleArgs.
    /// </summary>
    /// <param name="propertyName">The name of the property to be validated.</param>
    public RuleArgs(string propertyName)
    {
      _propertyName = propertyName;
    }

    /// <summary>
    /// Creates an instance of RuleArgs.
    /// </summary>
    /// <param name="propertyName">The name of the property to be validated.</param>
    /// <param name="severity">Initial default severity for the rule.</param>
    /// <remarks>
    /// <para>
    /// The <b>severity</b> parameter defines only the initial default 
    /// severity value. If the rule changes this value by setting
    /// e.Severity, then that new value will become the default for all
    /// subsequent rule invocations.
    /// </para><para>
    /// To avoid confusion, it is recommended that the 
    /// <b>severity</b> constructor parameter 
    /// only be used for rule methods that do not explicitly set
    /// e.Severity.
    /// </para>
    /// </remarks>
    public RuleArgs(string propertyName, RuleSeverity severity)
      : this(propertyName)
    {
      _severity = severity;
    }

    /// <summary>
    /// Creates an instance of RuleArgs.
    /// </summary>
    /// <param name="propertyName">The name of the property to be validated.</param>
    /// <param name="severity">The default severity for the rule.</param>
    /// <param name="stopProcessing">
    /// Initial default value for the StopProcessing property.
    /// </param>
    /// <remarks>
    /// <para>
    /// The <b>severity</b> and <b>stopProcessing</b> parameters 
    /// define only the initial default values. If the rule 
    /// changes these values by setting e.Severity or
    /// e.StopProcessing, then the new values will become 
    /// the default for all subsequent rule invocations.
    /// </para><para>
    /// To avoid confusion, It is recommended that the 
    /// <b>severity</b> and <b>stopProcessing</b> constructor 
    /// parameters only be used for rule methods that do 
    /// not explicitly set e.Severity or e.StopProcessing.
    /// </para>
    /// </remarks>
    public RuleArgs(string propertyName, RuleSeverity severity, bool stopProcessing)
      : this(propertyName, severity)
    {
      _stopProcessing = stopProcessing;
    }

    /// <summary>
    /// Return a string representation of the object.
    /// </summary>
    public override string ToString()
    {
      return _propertyName;
    }
  }
}
