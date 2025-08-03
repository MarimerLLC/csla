//-----------------------------------------------------------------------
// <copyright file="SingleRoot.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

namespace Csla.Test.ChildChanged
{
  [Serializable]
  public class SingleRoot : BusinessBase<SingleRoot>
  {
    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
    public string Name
    {
      get => GetProperty(NameProperty);
      set => SetProperty(NameProperty, value);
    }

    [Fetch]
    [FetchChild]
    private void Fetch(bool child)
    {
      if (child)
        MarkAsChild();
    }
  }
}