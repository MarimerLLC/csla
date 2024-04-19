//-----------------------------------------------------------------------
// <copyright file="AChildList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

namespace Csla.Test.LazyLoad
{
  [Serializable]
  public class AChildList : Csla.BusinessBindingListBase<AChildList, AChild>
  {

    [Fetch]
    private void Fetch([Inject] IChildDataPortal<AChild> childDataPortal)
    {
      MarkAsChild();
      this.Add(childDataPortal.CreateChild());
    }

    public new int EditLevel
    {
      get { return base.EditLevel; }
    }
  }
}