using System;
using System.Collections.Generic;
using System.Text;
using Csla;

namespace BLBTest
{
  [Serializable]
  public class DataEdit : BusinessBase<DataEdit>
  {
    
    public static readonly PropertyInfo<int> DataProperty = RegisterProperty<int>(c => c.Data);
    public int Data
    {
      get { return GetProperty(DataProperty); }
      set { SetProperty(DataProperty, value); }
    }

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    protected override void AddBusinessRules()
    {
        base.AddBusinessRules();
        
        BusinessRules.AddRule(new Csla.Rules.CommonRules.MinValue<int>(DataProperty, 10));
        BusinessRules.AddRule(new Csla.Rules.CommonRules.Required(NameProperty));
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
