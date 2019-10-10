//-----------------------------------------------------------------------
// <copyright file="CslaWarningsTagHelper.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Razor tag helper</summary>
//-----------------------------------------------------------------------
using Csla.Rules;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Csla.AspNetCore.Mvc.TagHelpers
{
  /// <summary>
  /// Razor tag helper that displays CSLA .NET
  /// validation warning text in a span.
  /// </summary>
  [HtmlTargetElement("span", Attributes = "csla-warnings-for")]
  public class CslaWarningsTagHelper : TagHelper
  {
    /// <summary>
    /// Model expression
    /// </summary>
    public ModelExpression CslaWarningsFor { get; set; }

    /// <summary>
    /// Process method
    /// </summary>
    /// <param name="context"></param>
    /// <param name="output"></param>
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
      if (CslaWarningsFor == null)
        return;
      var parts = CslaWarningsFor.Name.Split('.');
      if (parts.Length == 0)
        return;
      var model = CslaWarningsFor.ModelExplorer.Container.Model;
      var propertyName = parts[parts.Length - 1];
      object source = model;
      for (int i = 0; i < parts.Length - 1; i++)
        source = Csla.Reflection.MethodCaller.CallPropertyGetter(source, parts[i]);
      var result = string.Empty;
      if (source is Csla.Core.BusinessBase obj)
        result = obj.BrokenRulesCollection.ToString(",", RuleSeverity.Warning, propertyName);
      output.Content.SetContent(result);
    }
  }
}
