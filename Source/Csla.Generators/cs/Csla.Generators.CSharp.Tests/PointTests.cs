//-----------------------------------------------------------------------
// <copyright file="PointTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests of serialization behaviour on the AutoSerializable struct Point</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Csla.Serialization.Mobile;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Csla.Generators.CSharp.TestObjects;
using Microsoft.Extensions.DependencyInjection;
using Csla.Configuration;
using Csla.TestHelpers;

namespace Csla.Generators.CSharp.Tests
{

  /// <summary>
  /// Tests of serialization of the Point struct
  /// </summary>
  [TestClass]
  public class PointTests
  {
    private static TestDIContext _testDIContext;

    [ClassInitialize]
    public static void ClassInitialize(TestContext testContext)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
    }

    #region Serialize then Deserialize

    [TestMethod]
    public void SerializeThenDeserialize_PointWithXOf21_HasXOf21()
    {

      // Arrange
      int actual;
      int expected = 21;
      Point point = new Point() { X = 1, Y = 1 };
      point.X = 21;
      Point deserializedPoint;

      // Act
      deserializedPoint = SerializeThenDeserialisePoint(point);
      actual = deserializedPoint.X;

      // Assert
      Assert.AreEqual(expected, actual);

    }

    [TestMethod]
    public void SerializeThenDeserialize_PointWithYOf17_HasYOf17()
    {

      // Arrange
      int actual;
      int expected = 17;
      Point point = new Point() { X = 1, Y = 1 };
      point.Y = 17;
      Point deserializedPoint;

      // Act
      deserializedPoint = SerializeThenDeserialisePoint(point);
      actual = deserializedPoint.Y;

      // Assert
      Assert.AreEqual(expected, actual);

    }

    #endregion

    #region Private Helper Methods

    private Point SerializeThenDeserialisePoint(Point valueToSerialize)
    {
      var applicationContext = _testDIContext.CreateTestApplicationContext();

      System.IO.MemoryStream serializationStream;
      Nullable<Point> deserializedValue;
      MobileFormatter formatter = new MobileFormatter(applicationContext);

      // Act
      using (serializationStream = new System.IO.MemoryStream())
      {
        formatter.Serialize(serializationStream, valueToSerialize);
        serializationStream.Seek(0, System.IO.SeekOrigin.Begin);
        deserializedValue = formatter.Deserialize(serializationStream) as Nullable<Point>;
      }

      return deserializedValue.Value;
    }

    #endregion

  }
}
