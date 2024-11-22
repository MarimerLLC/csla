//-----------------------------------------------------------------------
// <copyright file="GraphMergeTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

namespace Csla.Test.GraphMerge
{
  internal class BranchUniqueIdentities : BusinessBase<BranchUniqueIdentities>
  {
    public static readonly PropertyInfo<Guid> IdProperty = RegisterProperty<Guid>(nameof(Id));
    public Guid Id
    {
      get => GetProperty(IdProperty);
      private set => SetProperty(IdProperty, value);
    }

    public static readonly PropertyInfo<LeafsUniqueIdentities> LeafsProperty = RegisterProperty<LeafsUniqueIdentities>(nameof(Leafs));
    public LeafsUniqueIdentities Leafs
    {
      get => GetProperty(LeafsProperty);
      private set => SetProperty(LeafsProperty, value);
    }

    [FetchChild]
    private async void Create([Inject] IChildDataPortal<LeafsUniqueIdentities> leafsPortal)
    {
      using (BypassPropertyChecks)
      {
        Id = Guid.NewGuid();

        Leafs = await leafsPortal.FetchChildAsync();
      }
    }

    [InsertChild]
    private async Task Insert()
    {
      await FieldManager.UpdateChildrenAsync();
    }

    [UpdateChild]
    private void Update()
    {
      FieldManager.UpdateChildren();
    }
  }
}