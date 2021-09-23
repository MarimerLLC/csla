//-----------------------------------------------------------------------
// <copyright file="PersonSerializationInfoFactory.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Factory for the creation of test SerializationInfo instances</summary>
//-----------------------------------------------------------------------
using Csla.Serialization.Mobile;
using System;
using System.Collections.Generic;
using System.Text;

namespace CslaSerialization.UnitTests.Helpers
{
  internal static class PersonSerializationInfoFactory
  {

    internal static SerializationInfo GetDefaultSerializationInfo()
    {
      SerializationInfo serializationInfo;

      serializationInfo = new SerializationInfo();
      serializationInfo.AddValue("_middleName", "");
      serializationInfo.AddValue("PersonId", 5);
      serializationInfo.AddValue("FirstName", "");
      serializationInfo.AddValue("LastName", "");
      serializationInfo.AddValue("DateOfBirth", DateTime.MinValue);
      serializationInfo.AddValue("NonSerializedText", "");
      serializationInfo.AddValue("PrivateText", "");
      serializationInfo.AddValue("PrivateSerializedText", "");

      return serializationInfo;
    }

  }
}
