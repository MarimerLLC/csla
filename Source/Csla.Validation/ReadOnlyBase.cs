﻿﻿//-----------------------------------------------------------------------
// <copyright file="ReadOnlyBase.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>Intermediate base class for ReadOnlyBase that provides properties for the CSLA 3.x migration</summary>
//----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csla.Rules;
#if SILVERLIGHT || NETFX_CORE
using Csla.Serialization;
#endif

namespace Csla.Validation
{
  /// <summary>
  /// Intermediate base class for ReadOnlyBase
  /// </summary>
  /// <typeparam name="T"></typeparam>
  [Serializable]
  public class ReadOnlyBase<T> : Csla.ReadOnlyBase<T> where T:ReadOnlyBase<T>
  {
    /// <summary>
    /// ValidationRules for CSLA 3.x compatibility
    /// </summary>
    protected BusinessRules ValidationRules
    {
      get { return base.BusinessRules; }
    }

    protected BusinessRules AuthorizationRules
    {
      get { return base.BusinessRules; }
    }
  }
}
