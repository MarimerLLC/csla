using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Csla.Analyzers.Extensions
{
  internal static class SuppressionAnalysisContextExtensions
  {
    /// <summary>
    /// Finds the innermost syntax node at the location of <paramref name="diagnostic"/>,
    /// and the semantic model for its tree.
    /// </summary>
    internal static bool TryFindNode(this SuppressionAnalysisContext context, Diagnostic diagnostic,
      [NotNullWhen(true)] out SyntaxNode? node, [NotNullWhen(true)] out SemanticModel? model)
    {
      var tree = diagnostic.Location.SourceTree;

      if (tree is null)
      {
        (node, model) = (null, null);
        return false;
      }

      node = tree.GetRoot(context.CancellationToken)
        .FindNode(diagnostic.Location.SourceSpan, getInnermostNodeForTie: true);
      model = context.GetSemanticModel(tree);
      return true;
    }
  }
}
