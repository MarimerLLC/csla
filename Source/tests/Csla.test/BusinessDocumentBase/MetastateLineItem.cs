//-----------------------------------------------------------------------
// <copyright file="MetastateLineItem.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Minimal child object for MetastateDocument tests.</summary>
//-----------------------------------------------------------------------

namespace Csla.Test.BusinessDocumentBase
{
  [Serializable]
  public class MetastateLineItem : BusinessBase<MetastateLineItem>
  {
    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
    public string Name
    {
      get => GetProperty(NameProperty);
      set => SetProperty(NameProperty, value);
    }

    [CreateChild]
    private void CreateChild() { }

    [FetchChild]
    private void Child_Fetch() { MarkOld(); }

    [InsertChild]
    private void Child_Insert() { }

    [UpdateChild]
    private void Child_Update() { }

    [DeleteSelfChild]
    private void Child_DeleteSelf() { }
  }
}
