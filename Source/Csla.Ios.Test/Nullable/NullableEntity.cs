using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Ios.Test
{
  [Serializable()]
  public class NullableEntity : BusinessBase<NullableEntity>
  {
    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);

    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    public static readonly PropertyInfo<int?> NullableIntegerProperty = RegisterProperty<int?>(c => c.NullableInteger);

    public int? NullableInteger
    {
      get { return GetProperty(NullableIntegerProperty); }
      set { SetProperty(NullableIntegerProperty, value); }
    }

    #region Rules

    protected override void AddBusinessRules()
    {
      base.AddBusinessRules();

      //Name      
      BusinessRules.AddRule(new ClearRule(NameProperty, NullableIntegerProperty));
    }

    private class ClearRule : Csla.Rules.PropertyRule
    {

      private Csla.Core.IPropertyInfo _clearProperty;

      public ClearRule(Csla.Core.IPropertyInfo primaryProperty, Csla.Core.IPropertyInfo clearProperty)
        : base(primaryProperty)
      {
        _clearProperty = clearProperty;
        InputProperties = new List<Csla.Core.IPropertyInfo> { primaryProperty };
        AffectedProperties.Add(_clearProperty);
      }

      protected override void Execute(Csla.Rules.IRuleContext context)
      {
        var input = (string)context.InputPropertyValues[PrimaryProperty];
        if (string.IsNullOrEmpty(input))
        {
          context.AddOutValue(_clearProperty, null);
        }
      }
    }

    #endregion

    public static NullableEntity NewNullableEntity()
    {
      return Csla.DataPortal.Create<NullableEntity>();
    }

    protected override void DataPortal_Create()
    {
      Csla.ApplicationContext.GlobalContext.Add("NullableObject", "Created");
    }
  }
}