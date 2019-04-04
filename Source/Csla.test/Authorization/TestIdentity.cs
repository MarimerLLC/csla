//-----------------------------------------------------------------------
// <copyright file="TestIdentity.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Test.Security
{
  [Serializable]
  public class TestIdentity : Csla.Security.CslaIdentity
  {
    public TestIdentity() { }

    public TestIdentity(string username, string password)
    {
      this.IsAuthenticated = true;
      this.Name = username;
    }

    public bool IsInRole(string role)
    {
      if (role == "Admin")
      {
        return true;
      }
      else
      {
        return false;
      }
    }
  }
}