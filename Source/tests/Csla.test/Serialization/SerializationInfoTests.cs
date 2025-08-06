//-----------------------------------------------------------------------
// <copyright file="SerializationTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Csla.Serialization.Mobile;

[TestClass]
public class SerializationInfoTests
{
  [TestMethod]
  public void IsNativeType_ShouldReturnTrue_ForPrimitiveTypes()
  {
    Assert.IsTrue(SerializationInfo.IsNativeType(typeof(int)), "int should be a native type");
    Assert.IsTrue(SerializationInfo.IsNativeType(typeof(double)), "double should be a native type");
    Assert.IsTrue(SerializationInfo.IsNativeType(typeof(bool)), "bool should be a native type");
  }

  [TestMethod]
  public void IsNativeType_ShouldReturnTrue_ForEnums()
  {
    Assert.IsTrue(SerializationInfo.IsNativeType(typeof(DayOfWeek)), "Enum should be a native type");
  }

  [TestMethod]
  public void IsNativeType_ShouldReturnFalse_ForIMobileObject()
  {
    Assert.IsFalse(SerializationInfo.IsNativeType(typeof(SerializationInfo)), "IMobileObject should not be a native type");
  }

  [TestMethod]
  public void IsNativeType_ShouldReturnTrue_ForKnownTypes()
  {
    Assert.IsTrue(SerializationInfo.IsNativeType(typeof(byte[])), "byte[] should be a native type");
  }

  [TestMethod]
  public void IsNativeType_ShouldReturnFalse_ForCustomTypes()
  {
    Assert.IsFalse(SerializationInfo.IsNativeType(typeof(CustomType)), "CustomType should not be a native type");
  }

  private class CustomType;
}