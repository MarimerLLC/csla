//-----------------------------------------------------------------------
// <copyright file="MetastateDocument.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Test document for metastate property change event tests.</summary>
//-----------------------------------------------------------------------

using System.ComponentModel.DataAnnotations;

namespace Csla.Test.BusinessDocumentBase
{
  [Serializable]
  public class MetastateDocument : BusinessDocumentBase<MetastateDocument, MetastateLineItem>
  {
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(nameof(Id));
    public int Id
    {
      get => GetProperty(IdProperty);
      set => SetProperty(IdProperty, value);
    }

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
    [Required]
    public string Name
    {
      get => GetProperty(NameProperty);
      set => SetProperty(NameProperty, value);
    }

    public void MakeOld() => MarkOld();

    [Create]
    private void DataPortal_Create()
    {
      BusinessRules.CheckRules();
    }

    [Insert]
    private void DataPortal_Insert()
    {
      FieldManager.UpdateChildren();
      Child_Update();
    }

    [Update]
    private void DataPortal_Update()
    {
      FieldManager.UpdateChildren();
      Child_Update();
    }

    [DeleteSelf]
    private void DataPortal_DeleteSelf()
    {
      Child_Update();
    }
  }
}
