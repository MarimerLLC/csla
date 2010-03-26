using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Properties;

namespace Csla.Rules
{
  /// <summary>
  /// Base class used to create business and validation
  /// rules.
  /// </summary>
  public abstract class BusinessRule : IBusinessRule
  {
    /// <summary>
    /// Gets the default description used by this rule.
    /// </summary>
    public string DefaultDescription { get; protected set; }
    /// <summary>
    /// Gets the default severity for this rule.
    /// </summary>
    public RuleSeverity DefaultSeverity { get; protected set; }
    /// <summary>
    /// Gets the default StopProcessing value for this rule.
    /// </summary>
    public bool DefaultStopProcessing { get; protected set; }
    /// <summary>
    /// Gets the userstate object provided when this rule was
    /// associated with the business object property.
    /// </summary>
    public object UserState { get; protected set; }
    /// <summary>
    /// Gets a list of secondary property values to be supplied to the
    /// rule when it is executed.
    /// </summary>
    public List<Csla.Core.IPropertyInfo> InputProperties { get; protected set; }
    /// <summary>
    /// Gets a value indicating whether the rule will run
    /// on a background thread.
    /// </summary>
    public bool IsAsync { get; protected set; }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    protected BusinessRule()
    {
      DefaultSeverity = RuleSeverity.Error;
    }

    /// <summary>
    /// Business or validation rule implementation.
    /// </summary>
    /// <param name="context">Rule context object.</param>
    protected virtual void Rule(RuleContext context)
    { }

    void IBusinessRule.Rule(RuleContext context)
    {
      Rule(context);
    }

    #region Load/Read Property

    /// <summary>
    /// Loads a property's managed field with the 
    /// supplied value calling PropertyHasChanged 
    /// if the value does change.
    /// </summary>
    /// <typeparam name="P">
    /// Type of the property.
    /// </typeparam>
    /// <param name="obj">
    /// Object on which to call the method. 
    /// </param>
    /// <param name="propertyInfo">
    /// PropertyInfo object containing property metadata.</param>
    /// <param name="newValue">
    /// The new value for the property.</param>
    /// <remarks>
    /// No authorization checks occur when this method is called,
    /// and no PropertyChanging or PropertyChanged events are raised.
    /// Loading values does not cause validation rules to be
    /// invoked.
    /// </remarks>
    protected void LoadProperty<P>(object obj, PropertyInfo<P> propertyInfo, P newValue)
    {
      var target = obj as Core.IManageProperties;
      if (target != null)
        target.LoadProperty<P>(propertyInfo, newValue);
      else
        throw new ArgumentException(Resources.IManagePropertiesRequiredException);
    }

    /// <summary>
    /// Reads a property's managed field value.
    /// </summary>
    /// <typeparam name="P"></typeparam>
    /// <param name="obj">
    /// Object on which to call the method. 
    /// </param>
    /// <param name="propertyInfo">
    /// PropertyInfo object containing property metadata.</param>
    /// <remarks>
    /// No authorization checks occur when this method is called.
    /// </remarks>
    protected P ReadProperty<P>(object obj, PropertyInfo<P> propertyInfo)
    {
      var target = obj as Core.IManageProperties;
      if (target != null)
        return target.ReadProperty(propertyInfo);
      else
        throw new ArgumentException(Resources.IManagePropertiesRequiredException);
    }

    #endregion
  }
}
