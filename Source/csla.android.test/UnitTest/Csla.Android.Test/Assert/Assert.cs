using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Csla.Android.Test
{
  public static class Assert
  {
    static string _errorMessage = "";
    static bool _isValid = true;

    public static bool IsValid
    {
      get { return _isValid;  }
    }

    public static string ErrorMessage
    {
      get { return _errorMessage; }
    }

    public static void Reset()
    {
      _errorMessage = "";
      _isValid = false;
    }

    public static void AreEqual(string value1, string value2)
    {
      _isValid = (value1.Equals(value2));
    }

    public static void AreEqual(bool value1, bool value2)
    {
      _isValid = (value1.Equals(value2));
    }

    public static void AreEqual(string value1, string value2, string message)
    {
      _isValid = (value1.Equals(value2));
      if (_errorMessage == "")
      {
        _errorMessage += message;
      }
      else
      {
        _errorMessage += "|" + message;
      }
    }

    public static void IsTrue(bool testValue)
    {
      _isValid = testValue;
    }

    public static void IsFalse(bool testValue)
    {
      _isValid = !testValue;
    }

    public static void IsInstanceOfType(Type checkType, object target)
    {
      _isValid = checkType == target.GetType();
    }
  }
}