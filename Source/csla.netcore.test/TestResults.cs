using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Csla.Test
{

  /// <summary>
  /// Static dictionary-like class that offers similar functionality to GlobalContext
  /// This is used in tests to record the completion of operations, for testing that the operation occurred
  /// </summary>
  internal class TestResults
  {
    private static AsyncLocal<Dictionary<string, string>> _testResults = new AsyncLocal<Dictionary<string, string>>();

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
    /// Get a result of an operation from the underlying results dictionary
    /// </summary>
    /// <param name="key">The key by which the operation is known</param>
    /// <returns>The value recorded against the operation, or an empty string if no result is found</returns>
    public static string GetResult(string key)
    {
      if (!_testResults.Value.ContainsKey(key)) return string.Empty;

      return _testResults.Value[key];
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
