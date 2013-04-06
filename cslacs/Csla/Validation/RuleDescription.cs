using System;
using System.Collections.Generic;

namespace Csla.Validation
{
  /// <summary>
  /// Parses a rule:// URI to provide
  /// easy access to the parts of the URI.
  /// </summary>
  public class RuleDescription
  {
    private string _scheme;
    private string _typeName;
    private string _methodName;
    private string _propertyName;
    private Dictionary<string, string> _arguments;

    /// <summary>
    /// Creates an instance of the object
    /// by parsing the provided rule:// URI.
    /// </summary>
    /// <param name="ruleString">The rule:// URI.</param>
    public RuleDescription(string ruleString)
    {
      Uri uri = new Uri(ruleString);

      _scheme = uri.Scheme + Uri.SchemeDelimiter;
      _typeName = Uri.UnescapeDataString(uri.Host);

      var parts = uri.LocalPath.Split('/');
      _methodName = parts[1];
      _propertyName = parts[2];

      string args = uri.Query;
      if (!(string.IsNullOrEmpty(args)))
      {
        if (args.StartsWith("?"))
        {
          args = args.Remove(0, 1);
        }
        _arguments = new Dictionary<string, string>();
        string[] argArray = args.Split('&');
        foreach (string arg in argArray)
        {
          string[] argParams = arg.Split('=');
          _arguments.Add(
            System.Uri.UnescapeDataString(argParams[0]), 
            System.Uri.UnescapeDataString(argParams[1]));
        }
      }
    }

    /// <summary>
    /// Parses a rule:// URI.
    /// </summary>
    /// <param name="ruleString">
    /// Text representation of a rule:// URI.</param>
    /// <returns>A populated RuleDescription object.</returns>
    public static RuleDescription Parse(string ruleString)
    {
      return new RuleDescription(ruleString);
    }

    /// <summary>
    /// Gets the scheme of the URI 
    /// (should always be rule://).
    /// </summary>
    public string Scheme
    {
      get { return _scheme; }
    }

    /// <summary>
    /// Gets the name of the type containing
    /// the rule method.
    /// </summary>
    public string MethodTypeName
    {
      get { return _typeName; }
    }

    /// <summary>
    /// Gets the name of the rule method.
    /// </summary>
    public string MethodName
    {
      get { return _methodName; }
    }
 
    /// <summary>
    /// Gets the name of the property with which
    /// the rule is associated.
    /// </summary>
    public string PropertyName
    {
      get { return _propertyName; }
    }
 
    /// <summary>
    /// Gets a Dictionary containing the
    /// name/value arguments provided to
    /// the rule method.
    /// </summary>
    public Dictionary<string, string> Arguments
    {
      get { return _arguments; }
    }
  }
}