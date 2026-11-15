using Microsoft.CodeAnalysis;

namespace Csla.Analyzers
{
  /// <summary>
  /// Decides whether an invoked method adds to a rule context: an <c>Add</c> method of the
  /// context type, or a method with <c>[AddsToRuleContext]</c>. Used by CSLA0017 and CSLA0031.
  /// </summary>
  internal static class RuleContextAddCallDetector
  {
    internal const string AttributeMetadataName = "Csla.Rules.AddsToRuleContextAttribute";
    internal const string RuleContextMetadataName = "Csla.Rules.IRuleContext";

    /// <summary>
    /// Gets whether <paramref name="invoked"/> is an <c>Add</c> method declared by
    /// <paramref name="contextType"/>.
    /// </summary>
    internal static bool IsAddMethodOn(IMethodSymbol invoked, ITypeSymbol contextType)
      => invoked.Name.StartsWith("Add") && SymbolEqualityComparer.Default.Equals(invoked.ContainingType, contextType);

    /// <summary>
    /// Gets whether <paramref name="invoked"/> has <c>[AddsToRuleContext]</c>. A reduced
    /// extension method or a constructed generic method is checked through its definition.
    /// </summary>
    internal static bool HasAttribute(IMethodSymbol invoked, INamedTypeSymbol? attributeType)
    {
      if (attributeType is null)
        return false;
      var definition = (invoked.ReducedFrom ?? invoked).OriginalDefinition;
      return definition.GetAttributes()
        .Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, attributeType));
    }
  }
}
