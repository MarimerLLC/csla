//-----------------------------------------------------------------------
// <copyright file="CslaErrorsTagHelper.cs" company="Marimer LLC">
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
  /// validation error text in a span.
  /// </summary>
  [HtmlTargetElement("span", Attributes = "csla-errors-for")]
  public class CslaErrorsTagHelper : TagHelper
  {
    /// <summary>
    /// Model expression
    /// </summary>
    public ModelExpression CslaErrorsFor { get; set; }

    /// <summary>
    /// Process method
    /// </summary>
    /// <param name="context"></param>
    /// <param name="output"></param>
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
      if (CslaErrorsFor == null)
        return;
      var parts = CslaErrorsFor.Name.Split('.');
      if (parts.Length == 0)
        return;
      var model = CslaErrorsFor.ModelExplorer.Container.Model;
      var propertyName = parts[parts.Length - 1];
      object source = model;
      for (int i = 0; i < parts.Length - 1; i++)
        source = Csla.Reflection.MethodCaller.CallPropertyGetter(source, parts[i]);
      var result = string.Empty;
      if (source is Csla.Core.BusinessBase obj)
        result = obj.BrokenRulesCollection.ToString(",", RuleSeverity.Error, propertyName);
      output.Content.SetContent(result);
    }
  }
}
