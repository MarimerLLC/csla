using System;
using Csla;
using Csla.Rules.CommonRules;
using System.ComponentModel.DataAnnotations;
using ShortCircuitingRules.Rules;
using StopIfIsNotNew = ShortCircuitingRules.Rules.StopIfIsNotNew;

namespace ShortCircuitingRules
{
  [Serializable]
  public class Root : BusinessBase<Root>
  {
    #region Business Methods

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);

    [Required]
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    [Range(0, 50)] 
    public static readonly PropertyInfo<int> Num1Property = RegisterProperty<int>(c => c.Num1);
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

    public static readonly PropertyInfo<string> NoteProperty = RegisterProperty<string>(c => c.Note);
    public string Note
    {
      get { return GetProperty(NoteProperty); }
      set { SetProperty(NoteProperty, value); }
    }
    #endregion

    #region Validation Rules

    protected override void AddBusinessRules()
    {
      // call base class implementation to add data annotation rules to BusinessRules 
      base.AddBusinessRules();

      // add authorization rules 
      // Required as DataAnnotationRuleon Name property
      // add maxlength with higher priority so it will not execute if Required is already broken.
      // NOTE: BusinessRules.ProcessThroughPriority is default 0 meaning that all rules at Priority lower or equal to 0 is always run, unless StopProcessing is called explicitly.
      BusinessRules.AddRule(new MaxLength(NameProperty, 50) { Priority = 1 });  
      BusinessRules.AddRule(new MinValue<int>(Num2Property, 5));
      BusinessRules.AddRule(new Required(NoteProperty));


      // ShortCircuit (ie do not run rules) for these properties when object has IsNew = false
      // DataAnnotation rules is always added with priority -1 so by giving ShortCircuiting rules a priority of -1 
      // you can also block DataAnootation rules from being executed. 
      // The same may also be done with StopINotCanWrite to prevent validation of fields that the user is not allowed to edit. 
      BusinessRules.AddRule(new StopIfIsNotNew(NameProperty) {Priority = -1});
      BusinessRules.AddRule(new StopIfIsNotNew(Num2Property) {Priority = -1});
      BusinessRules.AddRule(new StopIfIsNotNew(Num1Property) {Priority = -1});

      // only validates Note when user is allowed to edit field.
      BusinessRules.AddRule(new StopIfNotCanWrite(NoteProperty) {Priority = -1});

    }

    #endregion

    #region Factory Methods

    public static Root NewEditableRoot()
    {
      return DataPortal.Create<Root>();
    }

    public static Root GetEditableRoot()
    {
      return DataPortal.Fetch<Root>(null);
    }

    #endregion

    #region Data Access
    protected override void DataPortal_Create()
    {
      base.DataPortal_Create();
    }

    protected void DataPortal_Fetch(object criteria)
    {
      BusinessRules.CheckRules();
    }
    #endregion
  }
}
