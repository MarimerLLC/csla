//-----------------------------------------------------------------------
// <copyright file="PropertyInfoList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Core.FieldManager
{
  internal class PropertyInfoList : List<IPropertyInfo>
  {
    public bool IsLocked { get; set; }

    public PropertyInfoList()
    { }

    public PropertyInfoList(IList<IPropertyInfo> list)
      : base(list)
    { }
  }
}