//-----------------------------------------------------------------------
// <copyright file="GrandChildList.partial.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Testing.Business.EditableChildTests
{
  public partial class GrandChildList
  {
    #region Server Factories

    public static GrandChildList Load(Guid parentId)
    {
      return Csla.DataPortal.FetchChild<GrandChildList>(parentId);
    }

    #endregion

  }
}