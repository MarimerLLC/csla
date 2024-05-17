//-----------------------------------------------------------------------
// <copyright file="GrandChild.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

namespace Csla.Test.ChildChanged
{
  [Serializable]
  public class Grandchild : BusinessBase<Grandchild>
  {
    private static PropertyInfo<string> NameProperty = RegisterProperty(new PropertyInfo<string>("Name"));
    public string Name
    {
      get => GetProperty(NameProperty);
      set => SetProperty(NameProperty, value);
    }

    private static PropertyInfo<SingleChild> ChildProperty = RegisterProperty(new PropertyInfo<SingleChild>("Child"));
    public SingleChild Child => GetProperty(ChildProperty);

    [Fetch]
    private void Fetch([Inject] IChildDataPortal<SingleChild> childDataPortal)
    {
      LoadProperty(ChildProperty, childDataPortal.FetchChild(true));
    }
  }
}