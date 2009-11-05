using System;
using System.Collections.Generic;
using System.Text;
using Csla;

namespace BLBTest
{
  [Serializable]
  public class DataEdit : BusinessBase<DataEdit>
  {
    private static PropertyInfo<int> DataProperty = RegisterProperty<int>(c => c.Data);
    public int Data
    {
      get { return GetProperty(DataProperty); }
      set { SetProperty(DataProperty, value); }
    }

    private static PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    protected override void AddBusinessRules()
    {
      ValidationRules.AddRule(
        Csla.Validation.CommonRules.MinValue<int>, 
        new Csla.Validation.CommonRules.MinValueRuleArgs<int>(DataProperty, 10));

      ValidationRules.AddRule(
        Csla.Validation.CommonRules.StringRequired, NameProperty);
    }

    public DataEdit()
    {
      MarkAsChild();
    }

    public DataEdit(int id, string name)
      : this()
    {
      using (BypassPropertyChecks)
      {
        Data = id;
        Name = name;
      }
    }

    public int CurrentEditLevel
    {
      get
      {
        return EditLevel;
      }
    }

    public int CurrentEditLevelAdded
    {
      get
      {
        Csla.Core.IEditableBusinessObject ebo = (Csla.Core.IEditableBusinessObject)this;
        return ebo.EditLevelAdded;
      }
    }

    protected override void AcceptChangesComplete()
    {
      System.Diagnostics.Debug.WriteLine(string.Format("Acc: {0} ({1}, {2})", Data, CurrentEditLevel, CurrentEditLevelAdded));
      base.AcceptChangesComplete();
    }

    protected override void UndoChangesComplete()
    {
      System.Diagnostics.Debug.WriteLine(string.Format("Und: {0} ({1}, {2})", Data, CurrentEditLevel, CurrentEditLevelAdded));
      base.UndoChangesComplete();
    }

    protected override void CopyStateComplete()
    {
      System.Diagnostics.Debug.WriteLine(string.Format("Beg: {0} ({1}, {2})", Data, CurrentEditLevel, CurrentEditLevelAdded));
      base.CopyStateComplete();
    }
  }
}
