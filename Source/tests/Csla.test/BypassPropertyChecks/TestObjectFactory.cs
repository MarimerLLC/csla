﻿//-----------------------------------------------------------------------
// <copyright file="TestObjectFactory.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

namespace Csla.Test.BypassPropertyChecks
{
  public class TestObjectFactory : Csla.Server.ObjectFactory
  {
    public TestObjectFactory(ApplicationContext applicationContext) : base(applicationContext) {}

    public BypassBusinessBaseUsingFactory Fetch()
    {
      BypassBusinessBaseUsingFactory returnValue = new BypassBusinessBaseUsingFactory();
      using (BypassPropertyChecks(returnValue))
      {
        returnValue.Id2 = 7; // bypass user rights
      }
      return returnValue;
    }
  }
}