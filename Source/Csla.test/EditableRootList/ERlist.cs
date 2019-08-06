//-----------------------------------------------------------------------
// <copyright file="ERlist.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Test.EditableRootList
{
  public class ERlist : Csla.DynamicBindingListBase<ERitem>
  {
    public ERlist()
    {
      AllowEdit = true;
      AllowNew = true;
      AllowRemove = true;
    }

    protected override object AddNewCore()
    {
      ERitem item = ERitem.NewItem();
      this.Add(item);
      return item;
    }
  }
}