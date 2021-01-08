//-----------------------------------------------------------------------
// <copyright file="HtmlExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Html extension methods providing support for HTML rendering based on security permissions in an application.</summary>
//-----------------------------------------------------------------------
#if NETSTANDARD2_0 || NET5_0 || NETCORE3_1
using System;
using System.Linq;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Csla.Web.Mvc
{
  /// <summary>
  /// Html extension methods providing support for HTML rendering based on security permissions in an application.
  /// </summary>
  public static class HtmlExtensions
  {
    /// <summary>
    /// Gets the validation information messages
    /// for a property.
    /// </summary>
    /// <typeparam name="T">Model type</typeparam>
    /// <param name="htmlHelper">IHtmlHelper instance</param>
    /// <param name="expression">Model property</param>
    /// <returns></returns>
    public static HtmlString InformationFor<T>(
      this IHtmlHelper<T> htmlHelper,
      System.Linq.Expressions.Expression<Func<T, object>> expression)
    {
      var result = string.Empty;
      var model = htmlHelper.ViewData.Model;
      System.Reflection.PropertyInfo reflectedPropertyInfo = Csla.Reflection.Reflect<T>.GetProperty(expression);
      var propertyName = reflectedPropertyInfo.Name;
      if (model is Csla.Core.BusinessBase bb)
        result = bb.BrokenRulesCollection.ToString(",", Rules.RuleSeverity.Information, propertyName);
      return new HtmlString(result);
    }

    /// <summary>
    /// Gets the validation warning messages
    /// for a property.
    /// </summary>
    /// <typeparam name="T">Model type</typeparam>
    /// <param name="htmlHelper">IHtmlHelper instance</param>
    /// <param name="expression">Model property</param>
    /// <returns></returns>
    public static HtmlString WarningFor<T>(
      this IHtmlHelper<T> htmlHelper,
      System.Linq.Expressions.Expression<Func<T, object>> expression)
    {
      var result = string.Empty;
      var model = htmlHelper.ViewData.Model;
      System.Reflection.PropertyInfo reflectedPropertyInfo = Csla.Reflection.Reflect<T>.GetProperty(expression);
      var propertyName = reflectedPropertyInfo.Name;
      if (model is Csla.Core.BusinessBase bb)
        result = bb.BrokenRulesCollection.ToString(",", Rules.RuleSeverity.Warning, propertyName);
      return new HtmlString(result);
    }

    /// <summary>
    /// Gets the validation error messages
    /// for a property.
    /// </summary>
    /// <typeparam name="T">Model type</typeparam>
    /// <param name="htmlHelper">IHtmlHelper instance</param>
    /// <param name="expression">Model property</param>
    /// <returns></returns>
    public static HtmlString ErrorFor<T>(
      this IHtmlHelper<T> htmlHelper,
      System.Linq.Expressions.Expression<Func<T, object>> expression)
    {
      var result = string.Empty;
      var model = htmlHelper.ViewData.Model;
      System.Reflection.PropertyInfo reflectedPropertyInfo = Csla.Reflection.Reflect<T>.GetProperty(expression);
      var propertyName = reflectedPropertyInfo.Name;
      if (model is Csla.Core.BusinessBase bb)
        result = bb.BrokenRulesCollection.ToString(",", Rules.RuleSeverity.Error, propertyName);
      return new HtmlString(result);
    }

    /// <summary>
    /// Conditionally render HTML output according to the provided authorization action and underlyiong object type.
    /// </summary>
    /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
    /// <param name="action">AuthorizationActions for which the authorization is required.</param>
    /// <param name="objectType">CSLA object type for which the action is applied.</param>
    /// <param name="granted">The rendered HTML output for granted users.</param>
    /// <param name="denied">The rendered HTML output for denied users.</param>
    /// <returns>The appropriate HTML rendered output.</returns>
    public static HtmlString HasPermission(
                                    this IHtmlHelper htmlHelper,
                                    Csla.Rules.AuthorizationActions action,
                                    Type objectType,
                                    HtmlString granted,
                                    HtmlString denied)
    {
      if (Csla.Rules.BusinessRules.HasPermission(action, objectType))
        return granted;
      else
        return denied;
    }

    /// <summary>
    /// Conditionally render HTML output according to the provided authorization action and underlyiong object type.
    /// </summary>
    /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
    /// <param name="action">AuthorizationActions for which the authorization is required.</param>
    /// <param name="objectType">CSLA object type for which the action is applied.</param>
    /// <param name="granted">The rendered HTML output for granted users.</param>
    /// <param name="denied">The rendered HTML output for denied users.</param>
    /// <returns>The appropriate HTML rendered output.</returns>
    public static HtmlString HasPermission(
                                    this IHtmlHelper htmlHelper,
                                    Csla.Rules.AuthorizationActions action,
                                    Type objectType,
                                    HtmlString granted,
                                    string denied)
    {
      if (Csla.Rules.BusinessRules.HasPermission(action, objectType))
        return granted;
      else
        return new HtmlString(denied);
    }

    /// <summary>
    /// Conditionally render HTML output according to the provided authorization action and underlyiong object type.
    /// </summary>
    /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
    /// <param name="action">AuthorizationActions for which the authorization is required.</param>
    /// <param name="objectType">CSLA object type for which the action is applied.</param>
    /// <param name="granted">The rendered HTML output for granted users.</param>
    /// <param name="denied">The rendered HTML output for denied users.</param>
    /// <returns>The appropriate HTML rendered output.</returns>
    public static HtmlString HasPermission(
                                    this IHtmlHelper htmlHelper,
                                    Csla.Rules.AuthorizationActions action,
                                    Type objectType,
                                    string granted,
                                    string denied)
    {
      if (Csla.Rules.BusinessRules.HasPermission(action, objectType))
        return new HtmlString(granted);
      else
        return new HtmlString(denied);
    }

    /// <summary>
    /// Conditionally render HTML output according to the provided authorization member action and underlyiong object type.
    /// </summary>
    /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
    /// <param name="action">AuthorizationActions for which the authorization is required.</param>
    /// <param name="target">CSLA object instance.</param>
    /// <param name="member">CSLA object member.</param>
    /// <param name="granted">The rendered HTML output for granted users.</param>
    /// <param name="denied">The rendered HTML output for denied users.</param>
    /// <returns>The appropriate HTML rendered output.</returns>
    public static HtmlString HasPermission(
                                    this IHtmlHelper htmlHelper,
                                    Csla.Rules.AuthorizationActions action,
                                    object target,
                                    Csla.Core.IMemberInfo member,
                                    HtmlString granted,
                                    HtmlString denied)
    {
      var instance = target as Csla.Security.IAuthorizeReadWrite;
      if (instance == null) return denied;

      if ((action == Rules.AuthorizationActions.ReadProperty && instance.CanReadProperty(member.Name)) ||
          (action == Rules.AuthorizationActions.WriteProperty && instance.CanWriteProperty(member.Name)) ||
          (action == Rules.AuthorizationActions.ExecuteMethod && instance.CanExecuteMethod(member.Name)))
        return granted;
      else
        return denied;
    }

    /// <summary>
    /// Conditionally render HTML output according to the provided authorization member action and underlyiong object type.
    /// </summary>
    /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
    /// <param name="action">AuthorizationActions for which the authorization is required.</param>
    /// <param name="target">CSLA object instance.</param>
    /// <param name="member">CSLA object member.</param>
    /// <param name="granted">The rendered HTML output for granted users.</param>
    /// <param name="denied">The rendered HTML output for denied users.</param>
    /// <returns>The appropriate HTML rendered output.</returns>
    public static HtmlString HasPermission(
                                    this IHtmlHelper htmlHelper,
                                    Csla.Rules.AuthorizationActions action,
                                    object target,
                                    Csla.Core.IMemberInfo member,
                                    HtmlString granted,
                                    string denied)
    {
      var instance = target as Csla.Security.IAuthorizeReadWrite;
      if (instance == null) return new HtmlString(denied);

      if ((action == Rules.AuthorizationActions.ReadProperty && instance.CanReadProperty(member.Name)) ||
          (action == Rules.AuthorizationActions.WriteProperty && instance.CanWriteProperty(member.Name)) ||
          (action == Rules.AuthorizationActions.ExecuteMethod && instance.CanExecuteMethod(member.Name)))
        return granted;
      else
        return new HtmlString(denied);
    }

    /// <summary>
    /// Conditionally render HTML output according to the provided authorization member action and underlyiong object type.
    /// </summary>
    /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
    /// <param name="action">AuthorizationActions for which the authorization is required.</param>
    /// <param name="target">CSLA object instance.</param>
    /// <param name="member">CSLA object member.</param>
    /// <param name="granted">The rendered HTML output for granted users.</param>
    /// <param name="denied">The rendered HTML output for denied users.</param>
    /// <returns>The appropriate HTML rendered output.</returns>
    public static HtmlString HasPermission(
                                    this IHtmlHelper htmlHelper,
                                    Csla.Rules.AuthorizationActions action,
                                    object target,
                                    Csla.Core.IMemberInfo member,
                                    string granted,
                                    string denied)
    {
      var instance = target as Csla.Security.IAuthorizeReadWrite;
      if (instance == null) return new HtmlString(denied);

      if ((action == Rules.AuthorizationActions.ReadProperty && instance.CanReadProperty(member.Name)) ||
          (action == Rules.AuthorizationActions.WriteProperty && instance.CanWriteProperty(member.Name)) ||
          (action == Rules.AuthorizationActions.ExecuteMethod && instance.CanExecuteMethod(member.Name)))
        return new HtmlString(granted);
      else
        return new HtmlString(denied);
    }

    /// <summary>
    /// Conditionally render HTML output according to the provided authorization action and underlyiong object type.
    /// </summary>
    /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
    /// <param name="action">AuthorizationActions for which the authorization is required.</param>
    /// <param name="objectType">CSLA object type for which the action is applied.</param>
    /// <param name="grantedAction">The rendered HTML helper action for granted users.</param>
    /// <returns>The appropriate HTML rendered output.</returns>
    public static HtmlString HasPermission(
                                    this IHtmlHelper htmlHelper,
                                    Csla.Rules.AuthorizationActions action,
                                    Type objectType,
                                    Func<IHtmlHelper, HtmlString> grantedAction)
    {
      if (Csla.Rules.BusinessRules.HasPermission(action, objectType))
        return grantedAction.Invoke(htmlHelper);
      else
        return HtmlString.Empty;
    }

    /// <summary>
    /// Conditionally render HTML output according to the provided authorization member action and underlyiong object type.
    /// </summary>
    /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
    /// <param name="action">AuthorizationActions for which the authorization is required.</param>
    /// <param name="target">CSLA object instance.</param>
    /// <param name="member">CSLA object member.</param>
    /// <param name="grantedAction">The rendered HTML helper action for granted users.</param>
    /// <param name="denieddAction">The rendered HTML helper action for denied users.</param>
    /// <returns>The appropriate HTML rendered output.</returns>
    public static HtmlString HasPermission(
                                    this IHtmlHelper htmlHelper,
                                    Csla.Rules.AuthorizationActions action,
                                    object target,
                                    Csla.Core.IMemberInfo member,
                                    Func<IHtmlHelper, HtmlString> grantedAction,
                                    Func<IHtmlHelper, HtmlString> denieddAction)
    {
      var instance = target as Csla.Security.IAuthorizeReadWrite;
      if (instance == null) return denieddAction.Invoke(htmlHelper);

      if ((action == Rules.AuthorizationActions.ReadProperty && instance.CanReadProperty(member.Name)) ||
          (action == Rules.AuthorizationActions.WriteProperty && instance.CanWriteProperty(member.Name)) ||
          (action == Rules.AuthorizationActions.ExecuteMethod && instance.CanExecuteMethod(member.Name)))
        return grantedAction.Invoke(htmlHelper);
      else
        return denieddAction.Invoke(htmlHelper);
    }
  }
}
#else
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace Csla.Web.Mvc
{
  /// <summary>
  /// Html extension methods providing support for HTML rendering based on security permissions in an application.
  /// </summary>
  public static class HtmlExtensions
  {
    /// <summary>
    /// Conditionally render HTML output according to the provided authorization action and underlyiong object type.
    /// </summary>
    /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
    /// <param name="action">AuthorizationActions for which the authorization is required.</param>
    /// <param name="objectType">CSLA object type for which the action is applied.</param>
    /// <param name="granted">The rendered HTML output for granted users.</param>
    /// <param name="denied">The rendered HTML output for denied users.</param>
    /// <returns>The appropriate HTML rendered output.</returns>
    public static MvcHtmlString HasPermission(
                                    this HtmlHelper htmlHelper,
                                    Csla.Rules.AuthorizationActions action,
                                    Type objectType,
                                    MvcHtmlString granted,
                                    MvcHtmlString denied)
    {
      if (Csla.Rules.BusinessRules.HasPermission(action, objectType))
        return granted;
      else
        return denied;
    }

    /// <summary>
    /// Conditionally render HTML output according to the provided authorization action and underlyiong object type.
    /// </summary>
    /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
    /// <param name="action">AuthorizationActions for which the authorization is required.</param>
    /// <param name="objectType">CSLA object type for which the action is applied.</param>
    /// <param name="granted">The rendered HTML output for granted users.</param>
    /// <param name="denied">The rendered HTML output for denied users.</param>
    /// <returns>The appropriate HTML rendered output.</returns>
    public static MvcHtmlString HasPermission(
                                    this HtmlHelper htmlHelper,
                                    Csla.Rules.AuthorizationActions action,
                                    Type objectType,
                                    MvcHtmlString granted,
                                    string denied)
    {
      if (Csla.Rules.BusinessRules.HasPermission(action, objectType))
        return granted;
      else
        return MvcHtmlString.Create(denied);
    }

    /// <summary>
    /// Conditionally render HTML output according to the provided authorization action and underlyiong object type.
    /// </summary>
    /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
    /// <param name="action">AuthorizationActions for which the authorization is required.</param>
    /// <param name="objectType">CSLA object type for which the action is applied.</param>
    /// <param name="granted">The rendered HTML output for granted users.</param>
    /// <param name="denied">The rendered HTML output for denied users.</param>
    /// <returns>The appropriate HTML rendered output.</returns>
    public static MvcHtmlString HasPermission(
                                    this HtmlHelper htmlHelper,
                                    Csla.Rules.AuthorizationActions action,
                                    Type objectType,
                                    string granted,
                                    string denied)
    {
      if (Csla.Rules.BusinessRules.HasPermission(action, objectType))
        return MvcHtmlString.Create(granted);
      else
        return MvcHtmlString.Create(denied);
    }

    /// <summary>
    /// Conditionally render HTML output according to the provided authorization member action and underlyiong object type.
    /// </summary>
    /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
    /// <param name="action">AuthorizationActions for which the authorization is required.</param>
    /// <param name="target">CSLA object instance.</param>
    /// <param name="member">CSLA object member.</param>
    /// <param name="granted">The rendered HTML output for granted users.</param>
    /// <param name="denied">The rendered HTML output for denied users.</param>
    /// <returns>The appropriate HTML rendered output.</returns>
    public static MvcHtmlString HasPermission(
                                    this HtmlHelper htmlHelper,
                                    Csla.Rules.AuthorizationActions action,
                                    object target,
                                    Csla.Core.IMemberInfo member,
                                    MvcHtmlString granted,
                                    MvcHtmlString denied)
    {
      var instance = target as Csla.Security.IAuthorizeReadWrite;
      if (instance == null) return denied;

      if ((action == Rules.AuthorizationActions.ReadProperty && instance.CanReadProperty(member.Name)) ||
          (action == Rules.AuthorizationActions.WriteProperty && instance.CanWriteProperty(member.Name)) ||
          (action == Rules.AuthorizationActions.ExecuteMethod && instance.CanExecuteMethod(member.Name)))
        return granted;
      else
        return denied;
    }

    /// <summary>
    /// Conditionally render HTML output according to the provided authorization member action and underlyiong object type.
    /// </summary>
    /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
    /// <param name="action">AuthorizationActions for which the authorization is required.</param>
    /// <param name="target">CSLA object instance.</param>
    /// <param name="member">CSLA object member.</param>
    /// <param name="granted">The rendered HTML output for granted users.</param>
    /// <param name="denied">The rendered HTML output for denied users.</param>
    /// <returns>The appropriate HTML rendered output.</returns>
    public static MvcHtmlString HasPermission(
                                    this HtmlHelper htmlHelper,
                                    Csla.Rules.AuthorizationActions action,
                                    object target,
                                    Csla.Core.IMemberInfo member,
                                    MvcHtmlString granted,
                                    string denied)
    {
      var instance = target as Csla.Security.IAuthorizeReadWrite;
      if (instance == null) return MvcHtmlString.Create(denied);

      if ((action == Rules.AuthorizationActions.ReadProperty && instance.CanReadProperty(member.Name)) ||
          (action == Rules.AuthorizationActions.WriteProperty && instance.CanWriteProperty(member.Name)) ||
          (action == Rules.AuthorizationActions.ExecuteMethod && instance.CanExecuteMethod(member.Name)))
        return granted;
      else
        return MvcHtmlString.Create(denied);
    }

    /// <summary>
    /// Conditionally render HTML output according to the provided authorization member action and underlyiong object type.
    /// </summary>
    /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
    /// <param name="action">AuthorizationActions for which the authorization is required.</param>
    /// <param name="target">CSLA object instance.</param>
    /// <param name="member">CSLA object member.</param>
    /// <param name="granted">The rendered HTML output for granted users.</param>
    /// <param name="denied">The rendered HTML output for denied users.</param>
    /// <returns>The appropriate HTML rendered output.</returns>
    public static MvcHtmlString HasPermission(
                                    this HtmlHelper htmlHelper,
                                    Csla.Rules.AuthorizationActions action,
                                    object target,
                                    Csla.Core.IMemberInfo member,
                                    string granted,
                                    string denied)
    {
      var instance = target as Csla.Security.IAuthorizeReadWrite;
      if (instance == null) return MvcHtmlString.Create(denied);

      if ((action == Rules.AuthorizationActions.ReadProperty && instance.CanReadProperty(member.Name)) ||
          (action == Rules.AuthorizationActions.WriteProperty && instance.CanWriteProperty(member.Name)) ||
          (action == Rules.AuthorizationActions.ExecuteMethod && instance.CanExecuteMethod(member.Name)))
        return MvcHtmlString.Create(granted);
      else
        return MvcHtmlString.Create(denied);
    }

    /// <summary>
    /// Conditionally render HTML output according to the provided authorization action and underlyiong object type.
    /// </summary>
    /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
    /// <param name="action">AuthorizationActions for which the authorization is required.</param>
    /// <param name="objectType">CSLA object type for which the action is applied.</param>
    /// <param name="grantedAction">The rendered HTML helper action for granted users.</param>
    /// <returns>The appropriate HTML rendered output.</returns>
    public static MvcHtmlString HasPermission(
                                    this HtmlHelper htmlHelper,
                                    Csla.Rules.AuthorizationActions action,
                                    Type objectType,
                                    Func<HtmlHelper, MvcHtmlString> grantedAction)
    {
      if (Csla.Rules.BusinessRules.HasPermission(action, objectType))
        return grantedAction.Invoke(htmlHelper);
      else
        return MvcHtmlString.Empty;
    }

    /// <summary>
    /// Conditionally render HTML output according to the provided authorization member action and underlyiong object type.
    /// </summary>
    /// <param name="htmlHelper">The HTML helper instance that this method extends.</param>
    /// <param name="action">AuthorizationActions for which the authorization is required.</param>
    /// <param name="target">CSLA object instance.</param>
    /// <param name="member">CSLA object member.</param>
    /// <param name="grantedAction">The rendered HTML helper action for granted users.</param>
    /// <param name="denieddAction">The rendered HTML helper action for denied users.</param>
    /// <returns>The appropriate HTML rendered output.</returns>
    public static MvcHtmlString HasPermission(
                                    this HtmlHelper htmlHelper,
                                    Csla.Rules.AuthorizationActions action,
                                    object target,
                                    Csla.Core.IMemberInfo member,
                                    Func<HtmlHelper, MvcHtmlString> grantedAction,
                                    Func<HtmlHelper, MvcHtmlString> denieddAction)
    {
      var instance = target as Csla.Security.IAuthorizeReadWrite;
      if (instance == null) return denieddAction.Invoke(htmlHelper);

      if ((action == Rules.AuthorizationActions.ReadProperty && instance.CanReadProperty(member.Name)) ||
          (action == Rules.AuthorizationActions.WriteProperty && instance.CanWriteProperty(member.Name)) ||
          (action == Rules.AuthorizationActions.ExecuteMethod && instance.CanExecuteMethod(member.Name)))
        return grantedAction.Invoke(htmlHelper);
      else
        return denieddAction.Invoke(htmlHelper);
    }
  }
}
#endif