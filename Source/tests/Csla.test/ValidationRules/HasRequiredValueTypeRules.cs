//-----------------------------------------------------------------------
// <copyright file="HasRequiredValueTypeRules.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Business object with Required rules on value type properties</summary>
//-----------------------------------------------------------------------

namespace Csla.Test.ValidationRules
{
  [Serializable]
  public class HasRequiredValueTypeRules : BusinessBase<HasRequiredValueTypeRules>
  {
    public static PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id);
    public int Id
    {
      get { return GetProperty(IdProperty); }
      set { SetProperty(IdProperty, value); }
    }

    public static PropertyInfo<int?> NullableIdProperty = RegisterProperty<int?>(c => c.NullableId);
    public int? NullableId
    {
      get { return GetProperty(NullableIdProperty); }
      set { SetProperty(NullableIdProperty, value); }
    }

    protected override void AddBusinessRules()
    {
      BusinessRules.AddRule(new Rules.CommonRules.Required(IdProperty));
      BusinessRules.AddRule(new Rules.CommonRules.Required(NullableIdProperty));
    }

    public void CheckRules()
    {
      BusinessRules.CheckRules();
    }

    [Create]
    private void Create()
    {
    }
  }
}
