﻿//-----------------------------------------------------------------------
// <copyright file="NestingPOCO2Tests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests of serialization behaviour on the AutoSerializable class NestingPOCO2</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Csla.Serialization.Mobile;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Csla.Generator.AutoSerialization.CSharp.TestObjects;
using Microsoft.Extensions.DependencyInjection;
using Csla.Configuration;
using Csla.TestHelpers;

namespace Csla.Generator.AutoSerialization.CSharp.Tests
{

  /// <summary>
  /// Tests of serialization of the NestingPOCO2 class
  /// </summary>
  [TestClass]
  public class NestingPOCO2Tests
  {
    private static TestDIContext _testDIContext;

    [ClassInitialize]
    public static void ClassInitialize(TestContext testContext)
    {
      _testDIContext = TestDIContextFactory.CreateDefaultContext();
    }

    #region Serialize then Deserialize

    [TestMethod]
    public void SerializeThenDeserialize_NestingPOCO2WithValueOfTesty2_HasValueOfTesty2()
    {

      // Arrange
      string actual;
      string expected = "Testy2";
      NestingPOCO2 poco = new NestingPOCO2();
      poco.SetValue("Testy2");
      NestingPOCO2 deserializedPOCO;

      // Act
      deserializedPOCO = SerializeThenDeserialiseNestingPOCO2(poco);
      actual = deserializedPOCO.GetValue();

      // Assert
      Assert.AreEqual(expected, actual);

    }

    [TestMethod]
    public void SerializeThenDeserialize_NestingPOCO2WithNullValue_HasNullValue()
    {

      // Arrange
      string actual;
      NestingPOCO2 poco = new NestingPOCO2();
      poco.SetValue(null);
      NestingPOCO2 deserializedPOCO;

      // Act
      deserializedPOCO = SerializeThenDeserialiseNestingPOCO2(poco);
      actual = deserializedPOCO.GetValue();

      // Assert
      Assert.IsNull(actual);

    }

    #endregion

    #region Private Helper Methods

    private NestingPOCO2 SerializeThenDeserialiseNestingPOCO2(NestingPOCO2 valueToSerialize)
    {
      var applicationContext = _testDIContext.CreateTestApplicationContext();

      System.IO.MemoryStream serializationStream;
      NestingPOCO2 deserializedValue;
      MobileFormatter formatter = new MobileFormatter(applicationContext);

      // Act
      using (serializationStream = new System.IO.MemoryStream())
      {
        formatter.Serialize(serializationStream, valueToSerialize);
        serializationStream.Seek(0, System.IO.SeekOrigin.Begin);
        deserializedValue = formatter.Deserialize(serializationStream) as NestingPOCO2;
      }

      return deserializedValue;
    }

    #endregion

  }
}
