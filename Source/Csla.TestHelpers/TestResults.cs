﻿//-----------------------------------------------------------------------
// <copyright file="TestResults.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Static, dictionary-style container for test results</summary>
//-----------------------------------------------------------------------

namespace Csla.Test
{

  /// <summary>
  /// Static dictionary-like class that offers similar functionality to GlobalContext
  /// This is used in tests to record the completion of operations, for testing that the operation occurred
  /// </summary>
  public class TestResults
  {
    private static readonly AsyncLocal<Dictionary<string, string>> _testResults = new();

    /// <summary>
    /// Add an item to the test results, to indicate an outcome of a particular operation
    /// </summary>
    /// <param name="key">The key by which the operation is recognised</param>
    /// <param name="value">The outcome recorded against the operation</param>
    public static void Add(string key, string value)
    {
      _testResults.Value.Add(key, value);
    }

    /// <summary>
    /// Overwrite an item in the test results, to indicate an outcome of a particular operation
    /// </summary>
    /// <param name="key">The key by which the operation is recognised</param>
    /// <param name="value">The outcome recorded against the operation</param>
    public static void AddOrOverwrite(string key, string value)
    {
      _testResults.Value[key] = value;
    }

    /// <summary>
    /// Get a result of an operation from the underlying results dictionary
    /// </summary>
    /// <param name="key">The key by which the operation is known</param>
    /// <returns>The value recorded against the operation, or an empty string if no result is found</returns>
    public static string GetResult(string key)
    {
      if (!_testResults.Value.TryGetValue(key, out var result)) return string.Empty;

      return result;
    }

    /// <summary>
    /// Get the existence of a result of an operation from the underlying results dictionary
    /// </summary>
    /// <param name="key">The key by which the operation is known</param>
    /// <returns>Whether the key is present in the dictionary of results</returns>
    public static bool ContainsResult(string key)
    {
      return _testResults.Value.ContainsKey(key);
    }

    /// <summary>
    /// Reinitialise the dictionary, clearing any existing results, ready for the next test
    /// </summary>
    public static void Reinitialise()
    {
      if (_testResults.Value is null) _testResults.Value = new Dictionary<string, string>();
      _testResults.Value.Clear();
    }

  }
}
