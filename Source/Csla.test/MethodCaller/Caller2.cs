//-----------------------------------------------------------------------
// <copyright file="Caller2.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Test.MethodCaller
{
  public class Caller2 : Caller1
  {
    public override int Method1()
    {
      return 2;
    }

    public override int Method2(int something)
    {
      return 2;
    }
  }
}