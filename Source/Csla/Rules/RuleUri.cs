//-----------------------------------------------------------------------
// <copyright file="RuleUri.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Parses a rule:// URI to provide</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Rules
{
  /// <summary>
  /// Parses a rule:// URI to provide
  /// easy access to the parts of the URI.
  /// </summary>
  public class RuleUri
  {
    private Uri _uri;

    /// <summary>
    /// Creates an instance of the object
    /// by parsing the provided rule:// URI.
    /// </summary>
    /// <param name="ruleString">The rule:// URI.</param>
    public RuleUri(string ruleString)
      : this(new Uri(ruleString))
    { }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="uri">The rule:// URI.</param>
    public RuleUri(Uri uri)
    {
      _uri = uri;
      if (_uri.Scheme != "rule")
        throw new ArgumentException("RuleUri.Scheme");
    }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="rule">Rule object.</param>
    /// <param name="property">Property to which rule applies.</param>
    public RuleUri(IBusinessRuleBase rule, Csla.Core.IPropertyInfo property)
      : this(GetTypeName(rule), ((property == null) ? "(object)" : property.Name))
      //: this(rule.GetType().FullName, ((property == null) ? "null" : property.Name))
    { }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <param name="typeName">Name of the rule type.</param>
    /// <param name="propertyName">Name of the business object property or the string literal "null".</param>
    public RuleUri(string typeName, string propertyName)
    {
      var hostName = EncodeString(typeName).Replace(".-", ".");
      if (hostName.Length > 63)
      {
        var tmp = hostName;
        hostName = null;
        for (int i = 0; i < tmp.Length - 1; i = i + 63)
          hostName = hostName + tmp.Substring(i, ((i + 63 <= tmp.Length) ? 63 : tmp.Length - i)) + "/";
        hostName = hostName.Substring(0, hostName.Length - 1);
      }

      var uriString = "rule://" + hostName + "/" + EncodeString(propertyName);
      _uri = new Uri(uriString);
    }

    private static string EncodeString(string value)
    {
      var result = value;
      result = result.Replace("+", "-");
      result = result.Replace(" ", "");
      result = result.Replace("[", "");
      result = result.Replace("]", "");
      result = result.Replace("`", "-");
      result = result.Replace(",", "-");
      result = result.Replace("=", "-");
      result = System.Uri.EscapeUriString(result);
      result = result.Replace("%", "-");
      return result;
    }

    /// <summary>
    /// Parses a rule:// URI.
    /// </summary>
    /// <param name="ruleString">
    /// Text representation of a rule:// URI.</param>
    /// <returns>A populated RuleDescription object.</returns>
    public static RuleUri Parse(string ruleString)
    {
      return new RuleUri(ruleString);
    }

    /// <summary>
    /// Gets a string representation of the rule URI.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      return _uri.ToString();
    }

    /// <summary>
    /// Adds a query parameter to the URI.
    /// </summary>
    /// <param name="key">Key for the parameter.</param>
    /// <param name="value">Value of the parameter.</param>
    public void AddQueryParameter(string key, string value)
    {
      var uriText = ToString();
      if (uriText.Contains("?"))
        uriText = uriText + "&" + Uri.EscapeUriString(key) + "=" + Uri.EscapeUriString(value);
      else
        uriText = uriText + "?" + Uri.EscapeUriString(key) + "=" + Uri.EscapeUriString(value);
      _uri = new Uri(uriText);
    }

    /// <summary>
    /// Gets the name of the type containing
    /// the rule method.
    /// </summary>
    public string RuleTypeName
    {
      get 
      {
        string name = _uri.Host;
        if (_uri.Parts().Length > 1)
          for (int i = 0; i < _uri.Parts().Length - 1; i++)
            name = name + _uri.Parts()[i];
        return name.Replace("/", "");
      }
    }

    /// <summary>
    /// Gets the name of the property with which
    /// the rule is associated.
    /// </summary>
    public string PropertyName
    {
      get { return _uri.Parts()[_uri.Parts().Length - 1]; }
    }

    /// <summary>
    /// Gets a Dictionary containing the
    /// name/value arguments provided to
    /// the rule method.
    /// </summary>
    public Dictionary<string, string> Arguments
    {
      get
      {
        Dictionary<string, string> result = null;
        string args = _uri.Query;
        if (!(string.IsNullOrEmpty(args)))
        {
          if (args.StartsWith("?"))
            args = args.Remove(0, 1);

          result = new Dictionary<string, string>();
          string[] argArray = args.Split('&');
          foreach (string arg in argArray)
          {
            string[] argParams = arg.Split('=');
            result.Add(
              System.Uri.UnescapeDataString(argParams[0]),
              System.Uri.UnescapeDataString(argParams[1]));
          }
        }
        return result;
      }
    }

    private static string GetTypeName(IBusinessRuleBase rule)
    {
      var type = rule.GetType();
      return GetTypeName(type);
    }

    /// <summary>
    /// Gets the name of the type. 
    /// Recursive processing of generic constraints and parameters.
    /// </summary>
    /// <param name="type">The type.</param>
    /// <returns></returns>
    private static string GetTypeName(Type type)
    {
      if (!type.IsGenericType)
      {
        return type.FullName;
      }
      else // generic type
      {
        var sb = new StringBuilder();
        if (!string.IsNullOrEmpty(type.Namespace)) 
          sb.Append(type.Namespace + ".");
        var typeName = type.Name;
        sb.Append(typeName.Replace("`1", ""));
        foreach (var t in type.GetGenericArguments())
        {
          sb.Append("-");
          if (t.IsGenericType) 
            sb.Append(GetTypeName(t));
          else 
            sb.Append(t.FullName);
          sb.Append("-");
        }
        return sb.ToString();
      }
    }
  }

  /// <summary>
  /// Extension methods for System.Uri.
  /// </summary>
  public static class UriExtensions
  {
    /// <summary>
    /// Gets the segments (/ delimited parts) of the path.
    /// </summary>
    /// <param name="uri">URI to parse.</param>
    /// <returns>
    /// Returns the Segments property. 
    /// </returns>
    public static string[] Parts(this System.Uri uri)
    {
      return uri.Segments;
    }
  }

}