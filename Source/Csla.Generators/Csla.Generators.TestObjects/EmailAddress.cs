//-----------------------------------------------------------------------
// <copyright file="EmailAddress.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Class that can be used for testing serialization behaviour for classes implementing IMobileObject</summary>
//-----------------------------------------------------------------------
using Csla.Serialization.Mobile;
using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Generators.TestObjects
{

  /// <summary>
  /// Object already implementing IMobileObject that can be used for testing serialization behaviour
  /// </summary>
  [Serializable]
  public class EmailAddress : IMobileObject
  {

    public string Email { get; set; }

    public void GetChildren(SerializationInfo info, MobileFormatter formatter)
    {
    }

    public void GetState(SerializationInfo info)
    {
      info.AddValue("Email", Email);
    }

    public void SetChildren(SerializationInfo info, MobileFormatter formatter)
    {
    }

    public void SetState(SerializationInfo info)
    {
      Email = info.GetValue<string>("Email");
    }
  }
}
