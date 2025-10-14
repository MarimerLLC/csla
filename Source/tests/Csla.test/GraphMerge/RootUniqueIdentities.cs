//-----------------------------------------------------------------------
// <copyright file="GraphMergeTests.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

namespace Csla.Test.GraphMerge
{
  internal class RootUniqueIdentities : BusinessBase<RootUniqueIdentities>
  {
    public static readonly PropertyInfo<Guid> IdProperty = RegisterProperty<Guid>(nameof(Id));
    public Guid Id
    {
      get => GetProperty(IdProperty);
      private set => SetProperty(IdProperty, value);
    }

    public static readonly PropertyInfo<BranchUniqueIdentities> BranchProperty = RegisterProperty<BranchUniqueIdentities>(nameof(Branch));
    public BranchUniqueIdentities Branch
    {
      get => GetProperty(BranchProperty);
      private set => SetProperty(BranchProperty, value);
    }

    [Fetch]
    private async void Create([Inject] IChildDataPortal<BranchUniqueIdentities> portalBranch)
    {
      using (BypassPropertyChecks)
      {
        Id = Guid.NewGuid();

        Branch = await portalBranch.FetchChildAsync();
      }
    }

    [Insert]
    private async Task Insert()
    {
      await FieldManager.UpdateChildrenAsync();
    }

    [Update]
    private async Task Update()
    {
      await FieldManager.UpdateChildrenAsync();
    }
  }
}
