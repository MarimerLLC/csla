using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using Csla;

namespace MultipleBindingSources
{
  [Serializable]
  public class Child : BusinessBase<Child>
  {
    #region Business Methods

    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(p => p.Id);
    public int Id
    {
      get { return GetProperty<int>(IdProperty); }
      set { SetProperty<int>(IdProperty, value); }
    }

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(p => p.Name);
    public string Name
    {
      get { return GetProperty<string>(NameProperty); }
      set { SetProperty<string>(NameProperty, value); }
    }

    private static int _lastId;

    public void DumpEditLevels(StringBuilder sb)
    {
      sb.AppendFormat("  {0} {1}: {2} {3}\r", this.GetType().Name, "n/a", this.EditLevel, this.BindingEdit);
    }

    protected override object GetIdValue()
    {
      return ReadProperty(IdProperty);
    }

    #endregion

    #region Business Rules

    protected override void AddBusinessRules()
    {
      // TODO: add validation rules
      //ValidationRules.AddRule(RuleMethod, "");
    }

    private static void AddObjectAuthorizationRules()
    {
      // TODO: add authorization rules
      //AuthorizationRules.AllowEdit(typeof(Child), "Role");
    }

    #endregion

    #region Factory Methods

    internal static Child NewEditableChild()
    {
      return DataPortal.CreateChild<Child>();
    }

    internal static Child GetEditableChild(object childData)
    {
      return DataPortal.FetchChild<Child>(childData);
    }

    public Child()
    { /* Require use of factory methods */ }

    #endregion

    #region Data Access

    protected override void Child_Create()
    {
      // TODO: load default values
      // omit this override if you have no defaults to set
      LoadProperty(IdProperty, System.Threading.Interlocked.Increment(ref _lastId));
      LoadProperty(NameProperty, string.Format("Child {0}", ReadProperty(IdProperty)));
      MarkAsChild();
    }

    private void Child_Fetch(object childData)
    {
      // TODO: load values
    }

    private void Child_Insert(object parent)
    {
      // TODO: insert values
    }

    private void Child_Update(object parent)
    {
      // TODO: update values
    }

    private void Child_DeleteSelf(object parent)
    {
      // TODO: delete values
    }

    #endregion
  }
}
