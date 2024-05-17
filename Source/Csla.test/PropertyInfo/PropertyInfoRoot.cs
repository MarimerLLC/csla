//-----------------------------------------------------------------------
// <copyright file="PropertyInfoRoot.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Note: We exposed the PropertyInfo's so we can test it...</summary>
//-----------------------------------------------------------------------

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Csla.Test.PropertyInfo
{
  [Serializable]
  public class PropertyInfoRoot : BusinessBase<PropertyInfoRoot>
  {
    #region Factory Methods

    public static PropertyInfoRoot NewPropertyInfoRoot(IDataPortal<PropertyInfoRoot> dataPortal)
    {
      return dataPortal.Create();
    }

    #endregion

    #region DataPortal Methods

    [Create]
    protected void DataPortal_Create()
    {
    }

    #endregion

    /// <summary>
    /// Note: We exposed the PropertyInfo's so we can test it...
    /// </summary>
    #region Properties

    public static readonly PropertyInfo<System.String> _nameProperty = RegisterProperty<System.String>(p => p.Name);
    public System.String Name
    {
      get => GetProperty(_nameProperty);
      set => SetProperty(_nameProperty, value);
    }

    public static readonly PropertyInfo<System.String> _nameDataAnnotationsProperty = RegisterProperty<System.String>(p => p.NameDataAnnotations);
    [Display(Name = "Name: DataAnnotations")]
    public System.String NameDataAnnotations
    {
      get => GetProperty(_nameDataAnnotationsProperty);
      set => SetProperty(_nameDataAnnotationsProperty, value);
    }

    public static readonly PropertyInfo<System.String> _nameComponentModelProperty = RegisterProperty<System.String>(p => p.NameComponentModel);
    [DisplayName("Name: ComponentModel")]
    public System.String NameComponentModel
    {
      get => GetProperty(_nameComponentModelProperty);
      set => SetProperty(_nameComponentModelProperty, value);
    }

    public static readonly PropertyInfo<System.String> _nameFriendlyNameProperty = RegisterProperty<System.String>(p => p.NameFriendlyName, "Name: Friendly Name");
    public System.String NameFriendlyName
    {
      get => GetProperty(_nameFriendlyNameProperty);
      set => SetProperty(_nameFriendlyNameProperty, value);
    }

    public static readonly PropertyInfo<string> NameDefaultValueProperty = RegisterProperty<string>(c => c.NameDefaultValue, string.Empty, "x");
    public string NameDefaultValue
    {
      get => GetProperty(NameDefaultValueProperty);
      set => SetProperty(NameDefaultValueProperty, value);
    }

    public static readonly PropertyInfo<string> StringNullDefaultValueProperty = RegisterProperty<string>(c => c.StringNullDefaultValue, string.Empty, null);
    public string StringNullDefaultValue
    {
      get => GetProperty(StringNullDefaultValueProperty);
      set => SetProperty(StringNullDefaultValueProperty, value);
    }

    public static readonly PropertyInfo<string> ContainingTypeProperty = RegisterProperty<string>(new PropertyInfo<string>(nameof(ContainingType), null, typeof(PropertyInfoRoot), RelationshipTypes.None));
    public string ContainingType
    {
      get => GetProperty(ContainingTypeProperty);
      set => SetProperty(ContainingTypeProperty, value);
    }

    public static readonly PropertyInfo<string> ContainingTypeNullProperty = RegisterProperty<string>(new PropertyInfo<string>(nameof(ContainingTypeNull), null, null, RelationshipTypes.None));
    public string ContainingTypeNull
    {
      get => GetProperty(ContainingTypeNullProperty);
      set => SetProperty(ContainingTypeNullProperty, value);
    }

    #endregion
  }
}