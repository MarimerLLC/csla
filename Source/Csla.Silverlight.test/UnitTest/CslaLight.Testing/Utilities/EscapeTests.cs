//-----------------------------------------------------------------------
// <copyright file="EscapeTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Net;
using System.Windows;
#if !__ANDROID__
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
#endif
using UnitDriven;

namespace cslalighttest.Utilities
{
  [TestClass]
  public class EscapeTests
  {
    [TestMethod]
    public void TestEscape1()
    {
      string raw = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz`~!@#$%^&*()_+-=[]\\{}|;':\",./<>?";

      string expected = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz%60~!%40%23%24%25%5E%26*()_%2B-%3D%5B%5D%5C%7B%7D%7C%3B'%3A%22%2C.%2F%3C%3E%3F";
      string actual = Csla.Utilities.EscapeDataString(raw);

      Assert.AreEqual(expected, actual);
    }
  }
}