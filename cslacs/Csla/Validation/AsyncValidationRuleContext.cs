using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Validation
{
  /// <summary>
  /// Provides context information to an asynchronous
  /// validation rule.
  /// </summary>
  public class AsyncValidationRuleContext
  {
    private Dictionary<string, object> _propertyValues;
    private AsyncRuleArgs _inargs;
    private AsyncRuleResult _outargs;
    private AsyncRuleResultHandler _result;

    /// <summary>
    /// Gets a Dictionary containing the values of all properties
    /// associated with this rule.
    /// </summary>
    /// <remarks>
    /// The values provided by this property are copies of the original
    /// values. This helps provide thread safety, allowing a rule method
    /// to interact with the values safely, even though the code is running
    /// on a background thread.
    /// </remarks>
    public Dictionary<string, object> PropertyValues { get { return _propertyValues; } }
    /// <summary>
    /// Gets the input arguments to the validation rule. 
    /// </summary>
    /// <remarks>
    /// This
    /// property provides much of the same information as the
    /// RuleArgs parameter does to a synchronous rule method.
    /// </remarks>
    public AsyncRuleArgs InArgs { get { return _inargs; } }
    /// <summary>
    /// Gets the output arguments for the validation rule. The rule
    /// should set properties on this object for return to the
    /// validation subsystem.
    /// </summary>
    public AsyncRuleResult OutArgs { get { return _outargs; } }    

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="propertyValues">
    /// Dictionary containing copies of the business object
    /// property values for the properties associated with this rule.
    /// </param>
    /// <param name="inargs">
    /// Input arguments for use by the rule method.
    /// </param>
    /// <param name="outargs">
    /// Default output arguments that can be changed by the rule
    /// method.
    /// </param>
    /// <param name="result">
    /// Async result handler for the async callback on completion
    /// of the rule.
    /// </param>
    public AsyncValidationRuleContext(Dictionary<string, object> propertyValues, AsyncRuleArgs inargs, AsyncRuleResult outargs, AsyncRuleResultHandler result)
    {
      _propertyValues = propertyValues;
      _inargs = inargs;
      _outargs = outargs;
      _result = result;
    }
    
    /// <summary>
    /// Method that notifies the validation subsystem 
    /// when the async rule method is complete. This
    /// method <b>must be called</b>!
    /// </summary>
    /// <remarks>
    /// The async rule method <b>must</b> call this
    /// Complete() method when it is done (successfully
    /// or unsuccessfully). This includes the case
    /// where an exception occurred in the rule method.
    /// <b>This method must be called no matter what
    /// happens.</b>
    /// </remarks>
    public void Complete()
    {
      _result(_outargs);
    }
  }
}
