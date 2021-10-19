//-----------------------------------------------------------------------
// <copyright file="CslaInformationTagHelper.cs" company="Marimer LLC">
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
  /// information text in a span.
  /// </summary>
  [HtmlTargetElement("span", Attributes = "csla-information-for")]
  public class CslaInformationTagHelper : TagHelper
  {
    /// <summary>
    /// Model expression
    /// </summary>
    public ModelExpression CslaInformationFor { get; set; }

    /// <summary>
    /// Process method
    /// </summary>
    /// <param name="context"></param>
    /// <param name="output"></param>
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
      if (CslaInformationFor == null)
        return;
      var parts = CslaInformationFor.Name.Split('.');
      if (parts.Length == 0)
        return;
      var model = CslaInformationFor.ModelExplorer.Container.Model;
      var propertyName = parts[parts.Length - 1];
      object source = model;
      for (int i = 0; i < parts.Length - 1; i++)
        source = Csla.Reflection.MethodCaller.CallPropertyGetter(source, parts[i]);
      var result = string.Empty;
      if (source is Csla.Core.BusinessBase obj)
        result = obj.BrokenRulesCollection.ToString(",", RuleSeverity.Information, propertyName);
      output.Content.SetContent(result);
    }
  }
}
