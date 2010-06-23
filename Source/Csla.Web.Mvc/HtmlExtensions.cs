//-----------------------------------------------------------------------
// <copyright file="HtmlExtensions.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
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
  }
}
