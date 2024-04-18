//-----------------------------------------------------------------------
// <copyright file="ListContainerList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Test.ChildChanged
{
  [Serializable]
  public class ListContainerList : BusinessListBase<ListContainerList, ContainsList>
  {
    [Fetch]
    private void Create()
    {
    }
  }
}