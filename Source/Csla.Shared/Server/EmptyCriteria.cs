//-----------------------------------------------------------------------
// <copyright file="EmptyCriteria.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Empty criteria used by the data portal as a</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Serialization;

namespace Csla.Server
{
  /// <summary>
  /// Empty criteria used by the data portal as a
  /// placeholder for a create/fetch request that
  /// has no criteria.
  /// </summary>
  [Serializable]
  public class EmptyCriteria : Csla.Core.MobileObject
  { }
}