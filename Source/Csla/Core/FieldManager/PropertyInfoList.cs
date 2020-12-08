//-----------------------------------------------------------------------
// <copyright file="PropertyInfoList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Core.FieldManager
{
  /// <summary>
  /// List of IPropertyInfo objects for a business object type.
  /// </summary>
  public class PropertyInfoList : List<IPropertyInfo>
  {
    private bool _isLocked;
    internal bool IsLocked
    {
      get { return _isLocked; }
      set { _isLocked = value; }
    }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    public PropertyInfoList()
    { }

    /// <summary>
    /// Creates an instance of the object that
    /// contains the items in 'list'.
    /// </summary>
    /// <param name="list">Source list.</param>
    public PropertyInfoList(IList<IPropertyInfo> list)
      : base(list)
    { }
  }
}