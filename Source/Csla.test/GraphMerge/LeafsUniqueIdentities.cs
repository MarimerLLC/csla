//-----------------------------------------------------------------------
// <copyright file="GraphMergeTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

namespace Csla.Test.GraphMerge
{
  internal class LeafsUniqueIdentities : BusinessListBase<LeafsUniqueIdentities, LeafUniqueIdentities>
  {
    [FetchChild]
    private async void Fetch([Inject] IChildDataPortal<LeafUniqueIdentities> childDataPortal)
    {
      using (LoadListMode)
      {
        foreach (var id in Enumerable.Range(1, 5))
        {
          Add(await childDataPortal.FetchChildAsync(id));
        }
      }
    }
  }
}
