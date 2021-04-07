namespace Csla.Analyzers
{
  internal static class HelpUrlBuilder
  {
    internal static string Build(string identifier, string analyzerName) => 
      $"https://github.com/MarimerLLC/csla/tree/main/docs/analyzers/{identifier}-{analyzerName}.md";
  }
}