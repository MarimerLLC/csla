//-----------------------------------------------------------------------
// <copyright file="SingleList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;

namespace Csla.Test.ChildChanged
{
  [Serializable]
  public class SingleList : BusinessBindingListBase<SingleList, SingleRoot>
  {
    public SingleList()
    {
    }

    public SingleList(bool child)
      : this()
    {
      if (child)
        MarkAsChild();
    }
  }
}