//-----------------------------------------------------------------------
// <copyright file="PropertyInfoList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

namespace Csla.Core.FieldManager
{
  /// <summary>
  /// List of IPropertyInfo objects for a business object type.
  /// </summary>
  public class PropertyInfoList : List<IPropertyInfo>
  {
    internal bool IsLocked { get; set; }

    /// <summary>
    /// Creates an instance of the type.
    /// </summary>
    public PropertyInfoList()
    { }

    /// <summary>
    /// Creates an instance of the object that
    /// contains the items in 'list'.
    /// </summary>
    /// <param name="list">Source list.</param>
    /// <exception cref="ArgumentNullException"><paramref name="list"/> is <see langword="null"/>.</exception>
    public PropertyInfoList(IList<IPropertyInfo> list)
      : base(list)
    { }
  }
}