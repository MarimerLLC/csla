//-----------------------------------------------------------------------
// <copyright file="SplitOverload.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Csla.Serialization;

namespace Csla.Test.DataPortalTest
{
  [Serializable]
  class SplitOverload : SplitOverloadBase<SplitOverload>
  {
  }
}