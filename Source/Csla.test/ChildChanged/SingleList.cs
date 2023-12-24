//-----------------------------------------------------------------------
// <copyright file="SingleList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Test.ChildChanged
{
  [Serializable]
  public class SingleList : BusinessBindingListBase<SingleList, SingleRoot>
  {
    [Fetch]
    [FetchChild]
    private void Fetch(bool child)
    {
      if (child)
        MarkAsChild();
    }
  }
}