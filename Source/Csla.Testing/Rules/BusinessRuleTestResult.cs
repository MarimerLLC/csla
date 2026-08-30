//-----------------------------------------------------------------------
// <copyright file="BusinessRuleTestResult.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Outcome of executing a business rule with BusinessRuleTester</summary>
//-----------------------------------------------------------------------

using Csla.Core;
using Csla.Rules;

namespace Csla.Testing.Rules
{
  /// <summary>
  /// The outcome of executing a business rule through
  /// <see cref="BusinessRuleTester"/>.
  /// </summary>
  /// <remarks>
  /// The result owns the services created for the rule, so
  /// <see cref="Context"/> stays usable after the rule has run. Disposing the
  /// result releases those services; the recorded results remain readable.
  /// </remarks>
  public sealed class BusinessRuleTestResult : IDisposable
  {
    private readonly IRuleContext _context;
    private readonly IDisposable? _scope;

    /// <summary>
    /// Creates a new instance of the type.
    /// </summary>
    /// <param name="context">The rule context used to execute the rule.</param>
    /// <param name="scope">The services created for the rule, if any.</param>
    /// <exception cref="ArgumentNullException"><paramref name="context"/> is <see langword="null"/>.</exception>
    internal BusinessRuleTestResult(IRuleContext context, IDisposable? scope)
    {
      _context = context ?? throw new ArgumentNullException(nameof(context));
      _scope = scope;
    }

    /// <summary>
    /// Releases the services created to run the rule.
    /// </summary>
    public void Dispose() => _scope?.Dispose();

    /// <summary>
    /// Gets the rule context that was passed to the rule.
    /// </summary>
    public IRuleContext Context => _context;

    /// <summary>
    /// Gets the results added by the rule. A rule that adds no result of its
    /// own is given a single success result when the context is completed.
    /// </summary>
    public IReadOnlyList<RuleResult> Results => _context.Results;

    /// <summary>
    /// Gets the out values set by the rule.
    /// </summary>
    public IReadOnlyDictionary<IPropertyInfo, object?> OutputPropertyValues => _context.OutputPropertyValues;

    /// <summary>
    /// Gets the properties the rule marked as dirty.
    /// </summary>
    public IReadOnlyList<IPropertyInfo> DirtyProperties => _context.DirtyProperties;

    /// <summary>
    /// Gets a value indicating whether the rule added any broken result with a
    /// severity of <see cref="RuleSeverity.Error"/>.
    /// </summary>
    public bool HasErrors => HasSeverity(RuleSeverity.Error);

    /// <summary>
    /// Gets a value indicating whether the rule added any broken result with a
    /// severity of <see cref="RuleSeverity.Warning"/>.
    /// </summary>
    public bool HasWarnings => HasSeverity(RuleSeverity.Warning);

    /// <summary>
    /// Gets a value indicating whether the rule added any broken result with a
    /// severity of <see cref="RuleSeverity.Information"/>.
    /// </summary>
    public bool HasInformation => HasSeverity(RuleSeverity.Information);

    /// <summary>
    /// Gets a value indicating whether the rule completed without adding any
    /// broken result of any severity.
    /// </summary>
    public bool IsSuccess => _context.Results.All(r => r.Success);

    /// <summary>
    /// Gets the descriptions of all broken results with a severity of
    /// <see cref="RuleSeverity.Error"/>.
    /// </summary>
    public IReadOnlyList<string> ErrorMessages => GetMessages(RuleSeverity.Error);

    /// <summary>
    /// Gets the descriptions of all broken results with a severity of
    /// <see cref="RuleSeverity.Warning"/>.
    /// </summary>
    public IReadOnlyList<string> WarningMessages => GetMessages(RuleSeverity.Warning);

    /// <summary>
    /// Gets the descriptions of all broken results with a severity of
    /// <see cref="RuleSeverity.Information"/>.
    /// </summary>
    public IReadOnlyList<string> InformationMessages => GetMessages(RuleSeverity.Information);

    /// <summary>
    /// Gets the out value set by the rule for the specified property.
    /// </summary>
    /// <typeparam name="T">Type of the property value.</typeparam>
    /// <param name="property">The property whose out value is required.</param>
    /// <exception cref="ArgumentNullException"><paramref name="property"/> is <see langword="null"/>.</exception>
    /// <exception cref="KeyNotFoundException">The rule did not set an out value for <paramref name="property"/>.</exception>
    public T? GetOutValue<T>(IPropertyInfo property)
    {
      if (property is null)
        throw new ArgumentNullException(nameof(property));
      if (!_context.OutputPropertyValues.TryGetValue(property, out var value))
        throw new KeyNotFoundException($"No out value was set for property '{property.Name}'.");

      return (T?)value;
    }

    /// <summary>
    /// Gets the out value set by the rule for the specified property, if any.
    /// </summary>
    /// <typeparam name="T">Type of the property value.</typeparam>
    /// <param name="property">The property whose out value is required.</param>
    /// <param name="value">The out value, or the default value of <typeparamref name="T"/> when the rule set no out value.</param>
    /// <returns><see langword="true"/> if the rule set an out value for <paramref name="property"/>; otherwise <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="property"/> is <see langword="null"/>.</exception>
    public bool TryGetOutValue<T>(IPropertyInfo property, out T? value)
    {
      if (property is null)
        throw new ArgumentNullException(nameof(property));

      if (_context.OutputPropertyValues.TryGetValue(property, out var result))
      {
        value = (T?)result;
        return true;
      }

      value = default;
      return false;
    }

    private bool HasSeverity(RuleSeverity severity) => _context.Results.Any(r => !r.Success && r.Severity == severity);

    private List<string> GetMessages(RuleSeverity severity) =>
      _context.Results.Where(r => !r.Success && r.Severity == severity).Select(r => r.Description).ToList();
  }
}
