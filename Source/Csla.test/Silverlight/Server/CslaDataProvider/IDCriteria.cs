//-----------------------------------------------------------------------
// <copyright file="IDCriteria.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Security;
using Csla.Core;
using Csla.Serialization;

namespace Csla.Testing.Business.CslaDataProvider
{
  [Serializable]
  public class IDCriteria<T> : SingleCriteria<T, int>
  {
    public IDCriteria(int id) : base(id) { }

#if SILVERLIGHT
    public IDCriteria() : base() { }
#else
    private IDCriteria() : base() { }
#endif

  }

}