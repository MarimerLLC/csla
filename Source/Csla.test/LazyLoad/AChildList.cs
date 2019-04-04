//-----------------------------------------------------------------------
// <copyright file="AChildList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Test.LazyLoad
{
  [Serializable]
  public class AChildList : Csla.BusinessBindingListBase<AChildList, AChild>
  {
    public AChildList()
    {
      MarkAsChild();
      this.Add(new AChild());
    }

    public new int EditLevel
    {
      get { return base.EditLevel; }
    }
  }
}