//-----------------------------------------------------------------------
// <copyright file="PropertyInfoRoot.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Note: We exposed the PropertyInfo's so we can test it...</summary>
//-----------------------------------------------------------------------
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Csla.Test.PropertyInfo
{
  [Serializable()]
  public class PropertyInfoRoot : BusinessBase<PropertyInfoRoot>
  {
    #region Factory Methods

    public static PropertyInfoRoot NewPropertyInfoRoot()
    {
      return Csla.DataPortal.Create<PropertyInfoRoot>();
    }

    #endregion

    #region DataPortal Methods

    protected override void DataPortal_Create()
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
      get { return GetProperty(_nameProperty); }
      set { SetProperty(_nameProperty, value); }
    }

    public static readonly PropertyInfo<System.String> _nameDataAnnotationsProperty = RegisterProperty<System.String>(p => p.NameDataAnnotations);
    [Display(Name = "Name: DataAnnotations")]
    public System.String NameDataAnnotations
    {
      get { return GetProperty(_nameDataAnnotationsProperty); }
      set { SetProperty(_nameDataAnnotationsProperty, value); }
    }

    public static readonly PropertyInfo<System.String> _nameComponentModelProperty = RegisterProperty<System.String>(p => p.NameComponentModel);
    [DisplayName("Name: ComponentModel")]
    public System.String NameComponentModel
    {
      get { return GetProperty(_nameComponentModelProperty); }
      set { SetProperty(_nameComponentModelProperty, value); }
    }

    public static readonly PropertyInfo<System.String> _nameFriendlyNameProperty = RegisterProperty<System.String>(p => p.NameFriendlyName, "Name: Friendly Name");
    public System.String NameFriendlyName
    {
      get { return GetProperty(_nameFriendlyNameProperty); }
      set { SetProperty(_nameFriendlyNameProperty, value); }
    }

    public static readonly PropertyInfo<string> NameDefaultValueProperty = RegisterProperty<string>(c => c.NameDefaultValue, string.Empty, "x");
    public string NameDefaultValue
    {
      get { return GetProperty(NameDefaultValueProperty); }
      set { SetProperty(NameDefaultValueProperty, value); }
    }
    
    public static readonly PropertyInfo<string> StringNullDefaultValueProperty = RegisterProperty<string>(c => c.StringNullDefaultValue, string.Empty, null);
    public string StringNullDefaultValue
    {
      get { return GetProperty(StringNullDefaultValueProperty); }
      set { SetProperty(StringNullDefaultValueProperty, value); }
    }


    #endregion
  }
}