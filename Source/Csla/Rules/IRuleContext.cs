//-----------------------------------------------------------------------
// <copyright file="IRuleContext.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Context information provided to a business rule</summary>
//-----------------------------------------------------------------------
using Csla.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Rules
{
  /// <summary>
  /// Context information provided to a business rule
  /// when it is invoked.
  /// </summary>
  public interface IRuleContext
  {
    /// <summary>
    /// Gets the rule object.
    /// </summary>
    IBusinessRuleBase Rule { get; }

    /// <summary>
    /// Gets a reference to the target business object.
    /// </summary>
    object Target { get; }

    /// <summary>
    /// Gets a dictionary containing copies of property values from
    /// the target business object.
    /// </summary>
    Dictionary<Csla.Core.IPropertyInfo, object> InputPropertyValues { get; }
    /// <summary>
    /// Gets a list of dirty properties (value was updated).
    /// </summary>
    /// <value>
    /// The dirty properties.
    /// </value>
    List<IPropertyInfo> DirtyProperties { get; }
    /// <summary>
    /// Gets a dictionary containing copies of property values that
    /// should be updated in the target object.
    /// </summary>
    Dictionary<Csla.Core.IPropertyInfo, object> OutputPropertyValues { get; }
    /// <summary>
    /// Gets a list of RuleResult objects containing the
    /// results of the rule.
    /// </summary>
    List<RuleResult> Results { get; }
    /// <summary>
    /// Gets or sets the name of the origin property.
    /// </summary>
    /// <value>The name of the origin property.</value>
    string OriginPropertyName { get; }
    /// <summary>
    /// Executes the inner rule from the outer rules context. 
    /// Creates a chained context and if CanRunRule will execute the inner rule.  
    /// </summary>
    /// <param name="innerRule">The inner rule.</param>
    void ExecuteRule(IBusinessRuleBase innerRule);
    /// <summary>
    /// Gets a value indicating whether this instance is cascade context as a result of AffectedProperties.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is cascade context; otherwise, <c>false</c>.
    /// </value>
    bool IsCascadeContext { get; }
    /// <summary>
    /// Gets a value indicating whether this instance is property changed context.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is property changed context; otherwise, <c>false</c>.
    /// </value>
    bool IsPropertyChangedContext { get; }
    /// <summary>
    /// Gets a value indicating whether this instance is check rules context.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is check rules context; otherwise, <c>false</c>.
    /// </value>
    bool IsCheckRulesContext { get; }
    /// <summary>
    /// Gets a value indicating whether this instance is check object rules context.
    /// </summary>
    /// <value>
    /// 	<c>true</c> if this instance is check object rules context; otherwise, <c>false</c>.
    /// </value>
    bool IsCheckObjectRulesContext { get; }
    /// <summary>
    /// Gets a new RuleContext object for a chained rule.
    /// </summary>
    /// <param name="rule">Chained rule that will use
    /// this new context.</param>
    /// <remarks>
    /// The properties from the existing RuleContext will be
    /// used to create the new context, with the exception
    /// of the Rule property which is set using the supplied
    /// IBusinessRule value.
    /// </remarks>
    IRuleContext GetChainedContext(IBusinessRuleBase rule);
    /// <summary>
    /// Add a Error severity result to the Results list.
    /// </summary>
    /// <param name="description">Human-readable description of
    /// why the rule failed.</param>
    void AddErrorResult(string description);
    /// <summary>
    /// Add a Error severity result to the Results list.
    /// </summary>
    /// <param name="description">Human-readable description of
    /// why the rule failed.</param>
    /// <param name="stopProcessing">True if no further rules should be processed
    /// for the current property.</param>
    void AddErrorResult(string description, bool stopProcessing);
    /// <summary>
    /// Add a Error severity result to the Results list.
    /// This method is only allowed on "object" level rules to allow an object level rule to set warning result on a field. 
    /// </summary>
    /// <param name="property">Property to which the result applies.</param>
    /// <param name="description">Human-readable description of
    /// why the rule failed.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">When property is not defined in AffectedProperties list.</exception>   
    void AddErrorResult(Csla.Core.IPropertyInfo property, string description);
    /// <summary>
    /// Add a Warning severity result to the Results list.
    /// </summary>
    /// <param name="description">Human-readable description of
    /// why the rule failed.</param>
    void AddWarningResult(string description);
    /// <summary>
    /// Add a Warning severity result to the Results list.
    /// </summary>
    /// <param name="description">Human-readable description of
    /// why the rule failed.</param>
    /// <param name="stopProcessing">True if no further rules should be processed
    /// for the current property.</param>
    void AddWarningResult(string description, bool stopProcessing);
    /// <summary>
    /// Add a Warning severity result to the Results list.
    /// This method is only allowed on "object" level rules to allow an object level rule to set warning result on a field. 
    /// </summary>
    /// <param name="property">Property to which the result applies.</param>
    /// <param name="description">Human-readable description of  why the rule failed.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">When property is not defined in AffectedProperties list.</exception>    
    void AddWarningResult(Csla.Core.IPropertyInfo property, string description);
    /// <summary>
    /// Add an Information severity result to the Results list.
    /// </summary>
    /// <param name="description">Human-readable description of
    /// why the rule failed.</param>
    void AddInformationResult(string description);
    /// <summary>
    /// Add an Information severity result to the Results list.
    /// </summary>
    /// <param name="description">Human-readable description of why the rule failed.</param>
    /// <param name="stopProcessing">True if no further rules should be processed for the current property.</param>
    void AddInformationResult(string description, bool stopProcessing);
    /// <summary>
    /// Add an Information severity result to the Results list.
    /// This method is only allowed on "object" level rules to allow an object level rule to set warning result on a field. 
    /// </summary>
    /// <param name="property">Property to which the result applies.</param>
    /// <param name="description">Human-readable description of why the rule failed.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">When property is not defined in AffectedProperties list.</exception>   
    void AddInformationResult(Csla.Core.IPropertyInfo property, string description);
    /// <summary>
    /// Add a Success severity result to the Results list.
    /// </summary>
    /// <param name="stopProcessing">True if no further rules should be processed for the current property.</param>
    void AddSuccessResult(bool stopProcessing);
    /// <summary>
    /// Add an outbound value to update the rule's primary 
    /// property on the business object once the rule is complete.
    /// </summary>
    /// <param name="value">New property value.</param>
    void AddOutValue(object value);
    /// <summary>
    /// Add an outbound value to update a property on the business
    /// object once the rule is complete.
    /// </summary>
    /// <param name="property">Property to update.</param>
    /// <param name="value">New property value.</param>
    /// <exception cref="System.ArgumentOutOfRangeException">When property is not defined in AffectedProperties list.</exception>   
    void AddOutValue(Csla.Core.IPropertyInfo property, object value);
    /// <summary>
    /// Adds a property name as a dirty field (changed value).
    /// </summary>
    /// <param name="property">The property.</param>
    /// <exception cref="System.ArgumentOutOfRangeException"></exception>
    void AddDirtyProperty(Csla.Core.IPropertyInfo property);
    /// <summary>
    /// Indicates that the rule processing is complete, so
    /// CSLA .NET will process the Results list. This method
    /// must be invoked on the UI thread.
    /// </summary>
    void Complete();
    /// <summary>
    /// Gets the value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="propertyInfo">The property info.</param>
    /// <returns></returns>
    T GetInputValue<T>(PropertyInfo<T> propertyInfo);
    /// <summary>
    /// Gets the value with explicit cast
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="propertyInfo">The generic property info.</param>
    /// <returns></returns>
    T GetInputValue<T>(IPropertyInfo propertyInfo);
    /// <summary>
    /// Tries to get the value. Use this method on LazyLoaded properties to test if value has been provided or not.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="propertyInfo">The generic property info.</param>
    /// <param name="value">The value.</param>
    /// <returns>true if value exists else false</returns>
    bool TryGetInputValue<T>(PropertyInfo<T> propertyInfo, ref T value);
    /// <summary>
    /// Tries to get the value with explicit cast. Use this method on LazyLoaded properties to test if value has been provided or not.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="propertyInfo">The generic property info.</param>
    /// <param name="value">The value.</param>
    /// <returns>true if value exists else false</returns>
    bool TryGetInputValue<T>(IPropertyInfo propertyInfo, ref T value);
    /// <summary>
    /// Gets the execution context.
    /// </summary>
    /// <value>The execution context.</value>
    RuleContextModes ExecuteContext { get; }
  }
}
