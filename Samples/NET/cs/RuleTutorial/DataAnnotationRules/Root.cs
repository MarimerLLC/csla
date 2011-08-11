using System;
using Csla;
using Csla.Rules;
using Csla.Rules.CommonRules;
using System.ComponentModel.DataAnnotations;

namespace DataAnnotationRules
{
  [Serializable]
  public class Root : BusinessBase<Root>
  {
    #region Business Methods


    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    [Required]    // DataAnnotation must be placed here on the actual property - not the PropertyInfo
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }


    public static readonly PropertyInfo<int> Num1Property = RegisterProperty<int>(c => c.Num1);
    [Range(8, 100)]
    public int Num1
    {
      get { return GetProperty(Num1Property); }
      set { SetProperty(Num1Property, value); }
    }

    public static readonly PropertyInfo<int> Num2Property = RegisterProperty<int>(c => c.Num2);
    public int Num2
    {
      get { return GetProperty(Num2Property); }
      set { SetProperty(Num2Property, value); }
    }

    #endregion

    #region Validation Rules

    protected override void AddBusinessRules()
    {
      // call base class implementation to add data annotation rules to BusinessRules 
      base.AddBusinessRules();

      // NOTE: DataAnnotation rules is always added with Priority = 0 
    }

    #endregion

    #region Factory Methods

    public static Root NewEditableRoot()
    {
      return DataPortal.Create<Root>();
    }


    #endregion
  }
}
