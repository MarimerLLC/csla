//-----------------------------------------------------------------------
// <copyright file="AChild.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

namespace Csla.Test.LazyLoad
{
  [Serializable]
  public class AChild : Csla.BusinessBase<AChild>
  {
    public static PropertyInfo<Guid> IdProperty = RegisterProperty<Guid>(c => c.Id);
    public Guid Id
    {
      get => GetProperty(IdProperty);
      set => SetProperty(IdProperty, value);
    }

    public new int EditLevel => base.EditLevel;

    public AChild()
    {
    }

    [Create]
    [CreateChild]
    private void Create()
    {
      MarkAsChild();
      using (BypassPropertyChecks)
        Id = Guid.NewGuid();
    }
  }
}