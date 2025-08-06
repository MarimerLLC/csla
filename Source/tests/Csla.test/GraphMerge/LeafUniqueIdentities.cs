//-----------------------------------------------------------------------
// <copyright file="GraphMergeTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

namespace Csla.Test.GraphMerge
{
  internal class LeafUniqueIdentities : BusinessBase<LeafUniqueIdentities>
  {
    public static readonly PropertyInfo<int> LeafIdProperty = RegisterProperty<int>(nameof(LeafId));
    public int LeafId
    {
      get => GetProperty(LeafIdProperty);
      set => SetProperty(LeafIdProperty, value);
    }

    [Create]
    [CreateChild]
    private async Task Create(int leafId)
    {
      using (BypassPropertyChecks)
      {
        LeafId = leafId;
      }

      await BusinessRules.CheckRulesAsync();
    }

    [InsertChild]
    private void Insert() { }

    [FetchChild]
    private void Fetch(int id)
    {
      using (BypassPropertyChecks)
      {
        LeafId = id;
      }
    }
  }
}
