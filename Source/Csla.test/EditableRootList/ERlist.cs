//-----------------------------------------------------------------------
// <copyright file="ERlist.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

namespace Csla.Test.EditableRootList
{
  public class ERlist : DynamicBindingListBase<ERitem>
  {
    public ERlist()
    {
      AllowEdit = true;
      AllowNew = true;
      AllowRemove = true;
    }

    [Create]
    private void Create()
    {
    }
  }
}