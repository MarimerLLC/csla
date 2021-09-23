//-----------------------------------------------------------------------
// <copyright file="NestingPOCOTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Tests of serialization behaviour on the AutoSerializable class NestingPOCO</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Csla.Serialization.Mobile;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Csla.Generators.TestObjects;

namespace Csla.Generators.Tests
{

  /// <summary>
  /// Tests of serialization of the NestingPOCO class
  /// </summary>
  [TestClass]
  public class NestingPOCOTests
  {

    #region Serialize then Deserialize

    [TestMethod]
    public void SerializeThenDeserialize_NestingPOCOWithValueOfTesty_HasValueOfTesty()
    {

      // Arrange
      string actual;
      string expected = "Testy";
      NestingPOCO poco = new NestingPOCO();
      poco.SetValue("Testy");
      NestingPOCO deserializedPOCO;

      // Act
      deserializedPOCO = SerializeThenDeserialiseNestingPOCO(poco);
      actual = deserializedPOCO.GetValue();

      // Assert
      Assert.AreEqual(expected, actual);

    }

    [TestMethod]
    public void SerializeThenDeserialize_NestingPOCOWithNullValue_HasNullValue()
    {

      // Arrange
      string actual;
      NestingPOCO poco = new NestingPOCO();
      poco.SetValue(null);
      NestingPOCO deserializedPOCO;

      // Act
      deserializedPOCO = SerializeThenDeserialiseNestingPOCO(poco);
      actual = deserializedPOCO.GetValue();

      // Assert
      Assert.IsNull(actual);

    }

    #endregion

    #region Private Helper Methods

    private NestingPOCO SerializeThenDeserialiseNestingPOCO(NestingPOCO valueToSerialize)
    {
      System.IO.MemoryStream serializationStream;
      NestingPOCO deserializedValue;
      MobileFormatter formatter = new MobileFormatter();

      // Act
      using (serializationStream = new System.IO.MemoryStream())
      {
        formatter.Serialize(serializationStream, valueToSerialize);
        serializationStream.Seek(0, System.IO.SeekOrigin.Begin);
        deserializedValue = formatter.Deserialize(serializationStream) as NestingPOCO;
      }

      return deserializedValue;
    }

    #endregion

  }
}
