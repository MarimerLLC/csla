//-----------------------------------------------------------------------
// <copyright file="TestObjectFactory.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Test.BypassPropertyChecks
{
  public class TestObjectFactory : Csla.Server.ObjectFactory
  {
    public TestObjectFactory() { }

    public BypassBusinessBaseUsingFactory Fetch()
    {
      BypassBusinessBaseUsingFactory returnValue = new BypassBusinessBaseUsingFactory();
      using (this.BypassPropertyChecks(returnValue))
      {
        returnValue.Id2 = 7; // bypass user rights
      }
      return returnValue;
    }
  }
}