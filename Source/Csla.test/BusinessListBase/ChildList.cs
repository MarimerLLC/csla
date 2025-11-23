//-----------------------------------------------------------------------
// <copyright file="ChildList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using Csla.Core;

namespace Csla.Test.BusinessListBase
{
  [Serializable]
  public class ChildList : BusinessListBase<ChildList, Child>
  {
    public MobileList<Child> DeletedItems => DeletedList;
  }
}