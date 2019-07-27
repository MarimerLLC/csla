﻿//-----------------------------------------------------------------------
// <copyright file="PropertyHelper.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>internal implementation to get a registered property</summary>
//----------------------------------------------------------------------
using System;
using System.Linq;
using System.Text;
using Csla.Core;
using Csla.Core.FieldManager;
using Csla.Rules;
using Csla.Rules.CommonRules;

namespace Csla.Validation
{

  /// <summary>
  /// The delegate definition for CSLA 3.8 rule handler
  /// </summary>
  /// <param name="target">Target object.</param>
  /// <param name="e">RuleArgs parameter.</param>
  /// <returns>false if broken, true if passed</returns>
  public delegate bool RuleHandler(object target, RuleArgs e);

  /// <summary>
  /// The delegate definition for CSLA 3.8 generic rule handler
  /// </summary>
  /// <param name="target">Target object.</param>
  /// <param name="e">RuleArgs parameter.</param>
  /// <returns>false if broken, true if passed</returns>
  public delegate bool RuleHandler<T, R>(T target, R e) where R : RuleArgs;

  /// <summary>
  /// Helper class for wrapping old style (Csla 3.8x and earlier) rules with a lambda rule in Csla 4.x to simplify migration of older style apps.
  /// </summary>
  //[Obsolete("For migration of older apps to Csla 4.x only")]
  public static class RuleExtensions
  {
    /// <summary>
    /// Adds a lambda property level rule to business rules.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="businessRules">Business rules instance.</param>
    /// <param name="ruleHandler">Rule method.</param>
    /// <param name="primaryProperty">Primary property.</param>
    /// <param name="ruleArgs">Rule args object.</param>
    /// <param name="priority">Priority.</param>
    //[Obsolete("For migration of older apps to Csla 4.x only")]
    public static void AddRule<T>(this BusinessRules businessRules, RuleHandler<T,RuleArgs> ruleHandler, Csla.Core.IPropertyInfo primaryProperty, RuleArgs ruleArgs, int priority) where T : BusinessBase<T>
    {
      var rule = new Csla.Rules.CommonRules.Lambda(primaryProperty, (o) =>
                  {

                    var target = (T)o.Target;
                    using (new ObjectAccessor().BypassPropertyChecks(target))
                    {
                      if (!ruleHandler(target, ruleArgs))
                      {
                        o.Results.Add(new RuleResult(o.Rule.RuleName, o.Rule.PrimaryProperty, ruleArgs.Description) { Severity = ruleArgs.Severity, StopProcessing = ruleArgs.StopProcessing });
                      }
                      else if (ruleArgs.StopProcessing)
                      {
                        o.AddSuccessResult(true);
                      }
                      else
                      {
                        o.AddSuccessResult(false);
                      }
                      ruleArgs.StopProcessing = false;
                    }
                  });

#if NETFX_CORE
      var methodName = Guid.NewGuid().ToString();
#else
      var methodName = ruleHandler.Method.ToString();
#endif
      rule.AddQueryParameter("r", Convert.ToBase64String(Encoding.Unicode.GetBytes(methodName)));
      rule.Priority = priority;
      businessRules.AddRule(rule);
    }


    /// <summary>
    /// Adds a lambda property level rule to business rules.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="businessRules">Business rules.</param>
    /// <param name="ruleHandler">Rule method.</param>
    /// <param name="primaryProperty">Primary property.</param>
    //[Obsolete("For migration of older apps to Csla 4.x only")]
    public static void AddRule<T>(this BusinessRules businessRules, RuleHandler<T, RuleArgs> ruleHandler, Csla.Core.IPropertyInfo primaryProperty) where T : BusinessBase<T>
    {
      AddRule(businessRules, ruleHandler,  primaryProperty, new RuleArgs(primaryProperty), 0);
    }

    /// <summary>
    /// Adds a lambda property level rule to business rules.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="businessRules">Business rules.</param>
    /// <param name="ruleHandler">Rule method.</param>
    /// <param name="primaryProperty">Primary property.</param>
    /// <param name="priority">Priority.</param>
    //[Obsolete("For migration of older apps to Csla 4.x only")]
    public static void AddRule<T>(this BusinessRules businessRules,  RuleHandler<T,RuleArgs> ruleHandler , Csla.Core.IPropertyInfo primaryProperty, int priority) where T : BusinessBase<T>
    {
      AddRule(businessRules, ruleHandler, primaryProperty, new RuleArgs(primaryProperty), priority);
    }

    /// <summary>
    /// Adds a lambda property level rule to business rules.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="businessRules">Business rules</param>
    /// <param name="ruleHandler">Rule method</param>
    /// <param name="primaryPropertyName">Primary Property name</param>
    /// <param name="priority">Priority</param>
    //[Obsolete("For migration of older apps to Csla 4.x only")]
    public static void AddRule<T>(this BusinessRules businessRules, RuleHandler<T, RuleArgs> ruleHandler, string primaryPropertyName, int priority) where T : BusinessBase<T>
    {
        if (string.IsNullOrEmpty(primaryPropertyName)) throw new ArgumentException("primaryPropertyName");

        var primaryProperty = PropertyHelper.GetRegisteredProperty(businessRules, primaryPropertyName);
        AddRule(businessRules, ruleHandler, primaryProperty, new RuleArgs(primaryProperty), priority);
    }

    /// <summary>
    /// Adds a lambda property level rule to business rules.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="businessRules">Business rules</param>
    /// <param name="ruleHandler">Rule method</param>
    /// <param name="args">RuleArgs argument.</param>
    //[Obsolete("For migration of older apps to Csla 4.x only")]
    public static void AddRule<T>(this BusinessRules businessRules, RuleHandler<T, RuleArgs> ruleHandler, RuleArgs args) where T : BusinessBase<T>
    {
      var primaryProperty = PropertyHelper.GetRegisteredProperty(businessRules, args.PropertyName);
      AddRule(businessRules, ruleHandler, primaryProperty, args, 0);
    }

    /// <summary>
    /// Adds a lambda property level rule to business rules.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="businessRules">Business rules</param>
    /// <param name="ruleHandler">Rule method</param>
    /// <param name="args">RuleArgs argument.</param>
    /// <param name="priority">Priority</param>
    //[Obsolete("For migration of older apps to Csla 4.x only")]
    public static void AddRule<T>(this BusinessRules businessRules, RuleHandler<T, RuleArgs> ruleHandler, RuleArgs args, int priority) where T : BusinessBase<T>
    {
      var primaryProperty = PropertyHelper.GetRegisteredProperty(businessRules, args.PropertyName);
      AddRule(businessRules, ruleHandler, primaryProperty, args, priority);
    }

    /// <summary>
    /// Adds a lambda property level rule to business rules.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="businessRules">Business rules</param>
    /// <param name="ruleHandler">Rule method</param>
    /// <param name="primaryPropertyName">Name of the primary property.</param>
    ///// <param name="primaryPropertyName">Primary Property name</param>
    //[Obsolete("For migration of older apps to Csla 4.x only")]
    public static void AddRule<T>(this BusinessRules businessRules, RuleHandler<T, RuleArgs> ruleHandler, string primaryPropertyName) where T : BusinessBase<T>
    {
        if (string.IsNullOrEmpty(primaryPropertyName)) throw new ArgumentException("primaryPropertyName");

        var primaryProperty = PropertyHelper.GetRegisteredProperty(businessRules, primaryPropertyName);
        AddRule(businessRules, ruleHandler, primaryProperty, new RuleArgs(primaryProperty), 0);
    }


    /// <summary>
    /// Adds a lambda property level rule to business rules.
    /// </summary>
    /// <param name="businessRules">Business rules</param>
    /// <param name="ruleHandler">Rule method</param>
    /// <param name="primaryProperty">The primary property.</param>
    /// <param name="ruleArgs">The rule args object.</param>
    /// <param name="priority">Priority</param>
    //[Obsolete("For migration of older apps to Csla 4.x only")]
    public static void AddRule(this BusinessRules businessRules, RuleHandler ruleHandler, Csla.Core.IPropertyInfo primaryProperty, RuleArgs ruleArgs, int priority) 
    {
      var rule = new Csla.Rules.CommonRules.Lambda(primaryProperty, (o) =>
                  {

                    var target = (Csla.Core.BusinessBase)o.Target;
                    using (new ObjectAccessor().BypassPropertyChecks(target))
                    {
                      if (!ruleHandler(target, ruleArgs))
                      {
                        o.Results.Add(new RuleResult(o.Rule.RuleName, o.Rule.PrimaryProperty, ruleArgs.Description) { Severity = ruleArgs.Severity, StopProcessing = ruleArgs.StopProcessing });
                      }
                      else if (ruleArgs.StopProcessing)
                      {
                        o.AddSuccessResult(true);
                      }
                      else
                      {
                        o.AddSuccessResult(false);
                      }
                      ruleArgs.StopProcessing = false;
                    }
                  });

#if NETFX_CORE
      var methodName = Guid.NewGuid().ToString();
#else
      var methodName = ruleHandler.Method.ToString();
#endif
      rule.AddQueryParameter("r", Convert.ToBase64String(Encoding.Unicode.GetBytes(methodName)));
      rule.Priority = priority;
      businessRules.AddRule(rule);
    }


    /// <summary>
    /// Adds a lambda property level rule to business rules.
    /// </summary>
    /// <param name="businessRules">Business rules</param>
    /// <param name="ruleHandler">Rule method</param>
    /// <param name="primaryProperty">The primary property.</param>
    //[Obsolete("For migration of older apps to Csla 4.x only")]
    public static void AddRule(this BusinessRules businessRules, RuleHandler ruleHandler, Csla.Core.IPropertyInfo primaryProperty) 
    {
      AddRule(businessRules, ruleHandler, primaryProperty, new RuleArgs(primaryProperty), 0);
    }

    /// <summary>
    /// Adds a lambda property level rule to business rules.
    /// </summary>
    /// <param name="businessRules">Business rules</param>
    /// <param name="ruleHandler">Rule method</param>
    /// <param name="primaryProperty">The primary property.</param>
    /// <param name="priority">Priority</param>
    //[Obsolete("For migration of older apps to Csla 4.x only")]
    public static void AddRule(this BusinessRules businessRules, RuleHandler ruleHandler, Csla.Core.IPropertyInfo primaryProperty, int priority) 
    {
      AddRule(businessRules, ruleHandler, primaryProperty, new RuleArgs(primaryProperty), priority);
    }

    /// <summary>
    /// Adds a lambda property level rule to business rules.
    /// </summary>
    /// <param name="businessRules">Business rules</param>
    /// <param name="ruleHandler">Rule method</param>
    /// <param name="primaryPropertyName">Primary Property name</param>
    /// <param name="priority">Priority</param>
    //[Obsolete("For migration of older apps to Csla 4.x only")]
    public static void AddRule(this BusinessRules businessRules, RuleHandler ruleHandler, string primaryPropertyName, int priority)
    {
        if (string.IsNullOrEmpty(primaryPropertyName)) throw new ArgumentException("primaryPropertyName");
        var primaryProperty = PropertyHelper.GetRegisteredProperty(businessRules, primaryPropertyName);
        AddRule(businessRules, ruleHandler, primaryProperty, new RuleArgs(primaryProperty), priority);
    }


    /// <summary>
    /// Adds a lambda property level rule to business rules.
    /// </summary>
    /// <param name="businessRules">Business rules</param>
    /// <param name="ruleHandler">Rule method</param>
    /// <param name="primaryPropertyName">Primary Property name</param>
    //[Obsolete("For migration of older apps to Csla 4.x only")]
    public static void AddRule(this BusinessRules businessRules, RuleHandler ruleHandler, string primaryPropertyName)
    {
        if (string.IsNullOrEmpty(primaryPropertyName)) throw new ArgumentException("primaryPropertyName");

        var primaryProperty = PropertyHelper.GetRegisteredProperty(businessRules, primaryPropertyName);
        AddRule(businessRules, ruleHandler, primaryProperty, new RuleArgs(primaryProperty), 0);
    }

    /// <summary>
    /// Adds a lambda property level rule to business rules.
    /// </summary>
    /// <param name="businessRules">Business rules</param>
    /// <param name="ruleHandler">Rule method</param>
    /// <param name="args">RuleArgs argument.</param>
    //[Obsolete("For migration of older apps to Csla 4.x only")]
    public static void AddRule(this BusinessRules businessRules, RuleHandler ruleHandler, RuleArgs args) 
    {
      var primaryProperty = PropertyHelper.GetRegisteredProperty(businessRules, args.PropertyName);
      AddRule(businessRules, ruleHandler, primaryProperty, args, 0);
    }

    /// <summary>
    /// Adds a lambda property level rule to business rules.
    /// </summary>
    /// <param name="businessRules">Business rules</param>
    /// <param name="ruleHandler">Rule method</param>
    /// <param name="args">RuleArgs argument.</param>
    /// <param name="priority">Priority</param>
    //[Obsolete("For migration of older apps to Csla 4.x only")]
    public static void AddRule(this BusinessRules businessRules, RuleHandler ruleHandler, RuleArgs args, int priority) 
    {
      var primaryProperty = PropertyHelper.GetRegisteredProperty(businessRules, args.PropertyName);
      AddRule(businessRules, ruleHandler, primaryProperty, args, priority);
    }


    /// <summary>
    /// Adds a dependency between two properties.
    /// </summary>
    /// <param name="businessRules">Business rules</param>
    /// <param name="propertyInfo">The property.</param>
    /// <param name="dependentPropertyInfo">The dependent property.</param>
    /// <param name="isBidirectional">if set to <c>true</c> then dependency is bidirectional.</param>
    //[Obsolete("For migration of older apps to Csla 4.x only")]
    public static void AddDependentProperty(this BusinessRules businessRules, IPropertyInfo propertyInfo, IPropertyInfo dependentPropertyInfo, bool isBidirectional)
    {
      if (propertyInfo == null) throw new ArgumentNullException("propertyInfo");
      if (dependentPropertyInfo == null) throw new ArgumentNullException("dependentPropertyInfo");

      businessRules.AddRule(new Dependency(propertyInfo, dependentPropertyInfo));
      if (isBidirectional)
        businessRules.AddRule(new Dependency(dependentPropertyInfo, propertyInfo));
    }

    /// <summary>
    /// Adds a dependency between two properties from first property to second proerty
    /// </summary>
    /// <param name="businessRules">Business rules</param>
    /// <param name="propertyInfo">The property.</param>
    /// <param name="dependentPropertyInfo">The dependent property.</param>
    //[Obsolete("For migration of older apps to Csla 4.x only")]
    public static void AddDependentProperty(this BusinessRules businessRules, IPropertyInfo propertyInfo, IPropertyInfo dependentPropertyInfo)
    {
      AddDependentProperty(businessRules, propertyInfo, dependentPropertyInfo, false);
    }

    /// <summary>
    /// Adds a dependency between two properties from first property to second proerty
    /// </summary>
    /// <param name="businessRules">Business rules</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="dependentPropertyName">Name of the dependent property.</param>
    //[Obsolete("For migration of older apps to Csla 4.x only")]
    public static void AddDependentProperty(this BusinessRules businessRules, string propertyName, string dependentPropertyName)
    {
        AddDependentProperty(businessRules, propertyName, dependentPropertyName, false);
    }

    /// <summary>
    /// Adds a dependency between two properties from first property to second proerty
    /// </summary>
    /// <param name="businessRules">Business rules</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="dependentPropertyName">Name of the dependent property.</param>
    /// <param name="isBidirectional">if set to <c>true</c> [is bidirectional].</param>
    //[Obsolete("For migration of older apps to Csla 4.x only")]
    public static void AddDependentProperty(this BusinessRules businessRules, string propertyName, string dependentPropertyName, bool isBidirectional)
    {
      var propertyInfo = PropertyHelper.GetRegisteredProperty(businessRules, propertyName);
      var dependentPropertyInfo = PropertyHelper.GetRegisteredProperty(businessRules, dependentPropertyName);
      
      AddDependentProperty(businessRules, propertyInfo, dependentPropertyInfo, isBidirectional);
    }

    #region  Add Per-Type Roles

    /// <summary>
    /// Specify the roles allowed to read a given
    /// property.
    /// </summary>
    /// <param name="businessRules">Business rules</param>
    /// <param name="propertyInfo">PropertyInfo for the property.</param>
    /// <param name="roles">List of roles granted read access.</param>
    /// <remarks>
    /// This method may be called multiple times, with the roles in
    /// each call being added to the end of the list of allowed roles.
    /// In other words, each call is cumulative, adding more roles
    /// to the list.
    /// </remarks>
    public static void AllowRead(this BusinessRules businessRules, Core.IPropertyInfo propertyInfo, params string[] roles)
    {
      businessRules.AddRule(new IsInRole(AuthorizationActions.ReadProperty, propertyInfo, roles));
    }

    /// <summary>
    /// Specify the roles allowed to read a given
    /// property.
    /// </summary>
    /// <param name="businessRules">Business rules</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="roles">List of roles granted read access.</param>
    /// <remarks>
    /// This method may be called multiple times, with the roles in
    /// each call being added to the end of the list of allowed roles.
    /// In other words, each call is cumulative, adding more roles
    /// to the list.
    /// </remarks>
    public static void AllowRead(this BusinessRules businessRules, string propertyName, params string[] roles)
    {
      var propertyInfo = PropertyHelper.GetRegisteredProperty(businessRules, propertyName);
      businessRules.AddRule(new IsInRole(AuthorizationActions.ReadProperty, propertyInfo, roles));
    }

    /// <summary>
    /// Specify the roles denied read access to
    /// a given property.
    /// </summary>
    /// <param name="businessRules">Business rules</param>
    /// <param name="propertyInfo">PropertyInfo for the property.</param>
    /// <param name="roles">List of roles denied read access.</param>
    /// <remarks>
    /// This method may be called multiple times, with the roles in
    /// each call being added to the end of the list of denied roles.
    /// In other words, each call is cumulative, adding more roles
    /// to the list.
    /// </remarks>
    public static void DenyRead(this BusinessRules businessRules, Core.IPropertyInfo propertyInfo, params string[] roles)
    {
      businessRules.AddRule(new IsNotInRole(AuthorizationActions.ReadProperty, propertyInfo, roles));
    }

    /// <summary>
    /// Specify the roles denied read access to
    /// a given property.
    /// </summary>
    /// <param name="businessRules">Business rules</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="roles">List of roles denied read access.</param>
    /// <remarks>
    /// This method may be called multiple times, with the roles in
    /// each call being added to the end of the list of denied roles.
    /// In other words, each call is cumulative, adding more roles
    /// to the list.
    /// </remarks>
    public static void DenyRead(this BusinessRules businessRules, string propertyName, params string[] roles)
    {
      var propertyInfo = PropertyHelper.GetRegisteredProperty(businessRules, propertyName);
      businessRules.AddRule(new IsNotInRole(AuthorizationActions.ReadProperty, propertyInfo, roles));
    }

    /// <summary>
    /// Specify the roles allowed to write a given
    /// property.
    /// </summary>
    /// <param name="businessRules">Business rules</param>
    /// <param name="propertyInfo">PropertyInfo for the property.</param>
    /// <param name="roles">List of roles granted write access.</param>
    /// <remarks>
    /// This method may be called multiple times, with the roles in
    /// each call being added to the end of the list of allowed roles.
    /// In other words, each call is cumulative, adding more roles
    /// to the list.
    /// </remarks>
    public static void AllowWrite(this BusinessRules businessRules, Core.IPropertyInfo propertyInfo, params string[] roles)
    {
      businessRules.AddRule(new IsInRole(AuthorizationActions.WriteProperty, propertyInfo, roles));
    }

    /// <summary>
    /// Specify the roles allowed to write a given
    /// property.
    /// </summary>
    /// <param name="businessRules">Business rules</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="roles">List of roles granted write access.</param>
    /// <remarks>
    /// This method may be called multiple times, with the roles in
    /// each call being added to the end of the list of allowed roles.
    /// In other words, each call is cumulative, adding more roles
    /// to the list.
    /// </remarks>
    public static void AllowWrite(this BusinessRules businessRules, string propertyName, params string[] roles)
    {
      var propertyInfo = PropertyHelper.GetRegisteredProperty(businessRules, propertyName);
      businessRules.AddRule(new IsInRole(AuthorizationActions.WriteProperty, propertyInfo, roles));
    }

    /// <summary>
    /// Specify the roles denied write access to
    /// a given property.
    /// </summary>
    /// <param name="businessRules">Business rules</param>
    /// <param name="propertyInfo">PropertyInfo for the property.</param>
    /// <param name="roles">List of roles denied write access.</param>
    /// <remarks>
    /// This method may be called multiple times, with the roles in
    /// each call being added to the end of the list of denied roles.
    /// In other words, each call is cumulative, adding more roles
    /// to the list.
    /// </remarks>
    public static void DenyWrite(this BusinessRules businessRules, Core.IPropertyInfo propertyInfo, params string[] roles)
    {
      businessRules.AddRule(new IsNotInRole(AuthorizationActions.WriteProperty, propertyInfo, roles));
    }

    /// <summary>
    /// Specify the roles denied write access to
    /// a given property.
    /// </summary>
    /// <param name="businessRules">Business rules</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="roles">List of roles denied write access.</param>
    /// <remarks>
    /// This method may be called multiple times, with the roles in
    /// each call being added to the end of the list of denied roles.
    /// In other words, each call is cumulative, adding more roles
    /// to the list.
    /// </remarks>
    public static void DenyWrite(this BusinessRules businessRules, string propertyName, params string[] roles)
    {
      var propertyInfo = PropertyHelper.GetRegisteredProperty(businessRules, propertyName);
      businessRules.AddRule(new IsNotInRole(AuthorizationActions.WriteProperty, propertyInfo, roles));
    }

    /// <summary>
    /// Specify the roles allowed to execute a given
    /// method.
    /// </summary>
    /// <param name="businessRules">Business rules</param>
    /// <param name="memberInfo">The member info.</param>
    /// <param name="roles">List of roles granted execute access.</param>
    /// <remarks>
    /// This method may be called multiple times, with the roles in
    /// each call being added to the end of the list of allowed roles.
    /// In other words, each call is cumulative, adding more roles
    /// to the list.
    /// </remarks>
    public static void AllowExecute(this BusinessRules businessRules, Core.IMemberInfo memberInfo, params string[] roles)
    {
      businessRules.AddRule(new IsInRole(AuthorizationActions.ExecuteMethod, memberInfo, roles));
    }

    /// <summary>
    /// Specify the roles allowed to execute a given
    /// method.
    /// </summary>
    /// <param name="businessRules">Business rules</param>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="roles">List of roles granted execute access.</param>
    /// <remarks>
    /// This method may be called multiple times, with the roles in
    /// each call being added to the end of the list of allowed roles.
    /// In other words, each call is cumulative, adding more roles
    /// to the list.
    /// </remarks>
    public static void AllowExecute(this BusinessRules businessRules, string methodName, params string[] roles)
    {
      var methodInfo = new MethodInfo(methodName);
      businessRules.AddRule(new IsInRole(AuthorizationActions.ReadProperty, methodInfo, roles));
    }

    /// <summary>
    /// Specify the roles denied the right to execute
    /// a given method.
    /// </summary>
    /// <param name="businessRules">Business rules.</param>
    /// <param name="methodInfo">Method info for themethod.</param>
    /// <param name="roles">List of roles denied execute access.</param>
    /// <remarks>
    /// This method may be called multiple times, with the roles in
    /// each call being added to the end of the list of denied roles.
    /// In other words, each call is cumulative, adding more roles
    /// to the list.
    /// </remarks>
    public static void DenyExecute(this BusinessRules businessRules, Core.IMemberInfo methodInfo, params string[] roles)
    {
      businessRules.AddRule(new IsNotInRole(AuthorizationActions.ReadProperty, methodInfo, roles));
    }

    /// <summary>
    /// Specify the roles denied the right to execute
    /// a given method.
    /// </summary>
    /// <param name="businessRules">Business rules</param>
    /// <param name="methodName">Name of the method.</param>
    /// <param name="roles">List of roles denied execute access.</param>
    /// <remarks>
    /// This method may be called multiple times, with the roles in
    /// each call being added to the end of the list of denied roles.
    /// In other words, each call is cumulative, adding more roles
    /// to the list.
    /// </remarks>
    public static void DenyExecute(this BusinessRules businessRules, string methodName, params string[] roles)
    {
      var methodInfo = new MethodInfo(methodName);
      businessRules.AddRule(new IsNotInRole(AuthorizationActions.ReadProperty, methodInfo, roles));
    }

    #endregion

    #region Check Roles

    /// <summary>
    /// Indicates whether the property has a list
    /// of roles granted read access.
    /// </summary>
    /// <param name="businessRules">Business rules</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns>
    ///   <c>true</c> if [has read allowed roles] [the specified business rules]; otherwise, <c>false</c>.
    /// </returns>
    public static bool HasReadAllowedRoles(this BusinessRules businessRules, string propertyName)
    {
      return true;
    }

    /// <summary>
    /// Indicates whether the current user as defined by
    /// <see cref="Csla.ApplicationContext.User" />
    /// is explicitly allowed to read the property.
    /// </summary>
    /// <param name="businessRules">Business rules</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns>
    ///   <c>true</c> if [is read allowed] [the specified business rules]; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsReadAllowed(this BusinessRules businessRules, string propertyName)
    {
      var propertyInfo = PropertyHelper.GetRegisteredProperty(businessRules, propertyName);
      return businessRules.HasPermission(AuthorizationActions.ReadProperty, propertyInfo);
    }

    /// <summary>
    /// Indicates whether the property has a list
    /// of roles denied read access.
    /// </summary>
    /// <param name="businessRules">Business rules</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns>
    ///   <c>true</c> if [has read denied roles] [the specified business rules]; otherwise, <c>false</c>.
    /// </returns>
    public static bool HasReadDeniedRoles(this BusinessRules businessRules, string propertyName)
    {
      return true;
    }

    /// <summary>
    /// Indicates whether the current user as defined by
    /// <see cref="Csla.ApplicationContext.User" />
    /// is explicitly denied read access to the property.
    /// </summary>
    /// <param name="businessRules">Business rules list.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns>
    ///   <c>true</c> if [is read denied] [the specified business rules]; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsReadDenied(this BusinessRules businessRules, string propertyName)
    {
      var propertyInfo = PropertyHelper.GetRegisteredProperty(businessRules, propertyName);
      return !businessRules.HasPermission(AuthorizationActions.ReadProperty, propertyInfo);
    }

    /// <summary>
    /// Indicates whether the property has a list
    /// of roles granted write access.
    /// </summary>
    /// <param name="businessRules">Business rules</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns>
    ///   <c>true</c> if [has write allowed roles] [the specified business rules]; otherwise, <c>false</c>.
    /// </returns>
    public static bool HasWriteAllowedRoles(this BusinessRules businessRules, string propertyName)
    {
      return true;
    }

    /// <summary>
    /// Indicates whether the current user as defined by
    /// <see cref="Csla.ApplicationContext.User" />
    /// is explicitly allowed to set the property.
    /// </summary>
    /// <param name="businessRules">Business rules</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns>
    ///   <c>true</c> if [is write allowed] [the specified business rules]; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsWriteAllowed(this BusinessRules businessRules, string propertyName)
    {
      var propertyInfo = PropertyHelper.GetRegisteredProperty(businessRules, propertyName);
      return businessRules.HasPermission(AuthorizationActions.WriteProperty, propertyInfo);
    }

    /// <summary>
    /// Indicates whether the property has a list
    /// of roles denied write access.
    /// </summary>
    /// <param name="businessRules">Business rules</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns>
    ///   <c>true</c> if [has write denied roles] [the specified business rules]; otherwise, <c>false</c>.
    /// </returns>
    public static bool HasWriteDeniedRoles(this BusinessRules businessRules, string propertyName)
    {
      return true;
    }

    /// <summary>
    /// Indicates whether the current user as defined by
    /// <see cref="Csla.ApplicationContext.User" />
    /// is explicitly denied write access to the property.
    /// </summary>
    /// <param name="businessRules">Business rules</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns>
    ///   <c>true</c> if [is write denied] [the specified business rules]; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsWriteDenied(this BusinessRules businessRules, string propertyName)
    {
      var propertyInfo = PropertyHelper.GetRegisteredProperty(businessRules, propertyName);
      return !businessRules.HasPermission(AuthorizationActions.ReadProperty, propertyInfo);
    }

    /// <summary>
    /// Indicates whether the property has a list
    /// of roles granted execute access.
    /// </summary>
    /// <param name="businessRules">Business rules</param>
    /// <param name="methodName">Name of the method.</param>
    /// <returns>
    ///   <c>true</c> if [has execute allowed roles] [the specified business rules]; otherwise, <c>false</c>.
    /// </returns>
    public static bool HasExecuteAllowedRoles(this BusinessRules businessRules, string methodName)
    {
      return true;
    }

    /// <summary>
    /// Indicates whether the current user as defined by
    /// <see cref="Csla.ApplicationContext.User" />
    /// is explicitly allowed to execute the method.
    /// </summary>
    /// <param name="businessRules">Business rules</param>
    /// <param name="methodName">Name of the method.</param>
    /// <returns>
    ///   <c>true</c> if [is execute allowed] [the specified business rules]; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsExecuteAllowed(this BusinessRules businessRules, string methodName)
    {
      var methodInfo = new MethodInfo(methodName);
      return businessRules.HasPermission(AuthorizationActions.ExecuteMethod, methodInfo);
    }

    /// <summary>
    /// Indicates whether the property has a list
    /// of roles denied execute access.
    /// </summary>
    /// <param name="businessRules">Business rules</param>
    /// <param name="methodName">Name of the method.</param>
    /// <returns>
    ///   <c>true</c> if [has execute denied roles] [the specified business rules]; otherwise, <c>false</c>.
    /// </returns>
    public static bool HasExecuteDeniedRoles(this BusinessRules businessRules, string methodName)
    {
      return true;
    }

    /// <summary>
    /// Indicates whether the current user as defined by
    /// <see cref="Csla.ApplicationContext.User" />
    /// is explicitly denied execute access to the method.
    /// </summary>
    /// <param name="businessRules">Business rules</param>
    /// <param name="methodName">Name of the method.</param>
    /// <returns>
    ///   <c>true</c> if [is execute denied] [the specified business rules]; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsExecuteDenied(this BusinessRules businessRules, string methodName)
    {
      var methodInfo = new MethodInfo(methodName);
      return businessRules.HasPermission(AuthorizationActions.ReadProperty, methodInfo);
    }

    #endregion
  }
}
