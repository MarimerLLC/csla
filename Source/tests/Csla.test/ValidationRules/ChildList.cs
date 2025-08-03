//-----------------------------------------------------------------------
// <copyright file="ChildList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

namespace Csla.Test.ValidationRules
{
  [Serializable]
  public class ChildList : BusinessBindingListBase<ChildList, Child>
  {
    [Create]
    private void Create()
    {

    }
  }
}