#if !NETSTANDARD && !NETSTANDARD2_0
//-----------------------------------------------------------------------
// <copyright file="HtmlExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Html extension methods providing support for HTML rendering based on security permissions in an application.</summary>
//-----------------------------------------------------------------------
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