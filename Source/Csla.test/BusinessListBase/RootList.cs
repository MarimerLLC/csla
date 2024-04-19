//-----------------------------------------------------------------------
// <copyright file="RootList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

namespace Csla.Test.BusinessListBase
{
  [Serializable]
  public class RootList : BusinessListBase<RootList, Child>
  {
    [Create]
    private void Create()
    {
    }

    [Update]
    protected void DataPortal_Update()
    {
      Child_Update();
    }
  }
}