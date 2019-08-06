using System;
using System.Collections.Generic;
using System.Text;
using Csla;

namespace MultipleBindingSources
{
  [Serializable]
  public class Root : BusinessBase<Root>
  {
    #region Business Methods
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(p => p.Id);
    public int Id
    {
      get { return GetProperty<int>(IdProperty); }
      set { SetProperty<int>(IdProperty, value); }
    }

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>( p => p.Name);
    public string Name
    {
      get { return GetProperty<string>(NameProperty); }
      set { SetProperty<string>(NameProperty, value); }
    }

    public static readonly PropertyInfo<string> Address1Property = RegisterProperty<string>(p => p.Address1);
    public string Address1
    {
      get { return GetProperty<string>(Address1Property); }
      set { SetProperty<string>(Address1Property, value); }
    }

    public static readonly PropertyInfo<ChildList> ChildrenProperty = RegisterProperty(new PropertyInfo<ChildList>("Children", "Children"));
    public ChildList Children
    {
      get
      {
        if (!FieldManager.FieldExists(ChildrenProperty))
          LoadProperty<ChildList>(ChildrenProperty, ChildList.NewChildList());
        return GetProperty<ChildList>(ChildrenProperty); 
      }
      set { SetProperty<ChildList>(ChildrenProperty, value); }
    }

    protected override object GetIdValue()
    {
      return ReadProperty(IdProperty);
    }

    #endregion

    public void DumpEditLevels()
    {
      StringBuilder sb = new StringBuilder();
      sb.AppendFormat("{0} {1}: {2} {3}\r", this.GetType().Name, this.GetIdValue().ToString(), this.EditLevel, this.BindingEdit);
      var childList = ReadProperty<ChildList>(ChildrenProperty);
      if (childList != null)
        childList.DumpEditLevels(sb);
      else
        sb.AppendFormat("{0} null: - -\r", "ChildList", "-");
      System.Windows.Forms.MessageBox.Show(sb.ToString());
    }

    #region Business Rules

    protected override void AddBusinessRules()
    {
      BusinessRules.AddRule(new MakeUpper(NameProperty));
    }

    public class MakeUpper : Csla.Rules.BusinessRule
    {
      public MakeUpper(Csla.Core.IPropertyInfo primaryProperty)
        : base(primaryProperty)
      {
        InputProperties = new List<Csla.Core.IPropertyInfo> { primaryProperty };
      }

      protected override void Execute(Csla.Rules.IRuleContext context)
      {
        var sender = (Root)context.Target;
        using (sender.BypassPropertyChecks)
        {
          var value = (string)context.InputPropertyValues[PrimaryProperty];
          if (!string.IsNullOrEmpty(value))
            context.AddOutValue(PrimaryProperty, value.ToUpper());
        }
      }
    }

    private static void AddObjectAuthorizationRules()
    {
      // TODO: add authorization rules
      //AuthorizationRules.AllowEdit(typeof(Root), "Role");
    }

    #endregion

    #region Factory Methods

    public static Root NewEditableRoot()
    {
      return DataPortal.Create<Root>();
    }

    public static Root GetEditableRoot(int id)
    {
      return DataPortal.Fetch<Root>(id);
    }

    public static void DeleteEditableRoot(int id)
    {
      DataPortal.Delete<Root>(id);
    }

    public Root()
    { /* Require use of factory methods */ }

    #endregion

    #region Data Access

    [RunLocal]
    protected override void DataPortal_Create()
    {
      // TODO: load default values
      // omit this override if you have no defaults to set
      base.DataPortal_Create();
      LoadProperty(IdProperty, 1);
      LoadProperty(NameProperty, "Root 1");
      LoadProperty(ChildrenProperty, ChildList.NewChildList());
    }

    private void DataPortal_Fetch(int criteria)
    {
      // TODO: load values
    }

    //[Transactional(TransactionalTypes.TransactionScope)]
    protected override void DataPortal_Insert()
    {
      // TODO: insert values
    }

    //[Transactional(TransactionalTypes.TransactionScope)]
    protected override void DataPortal_Update()
    {
      // TODO: update values
    }

    //[Transactional(TransactionalTypes.TransactionScope)]
    protected override void DataPortal_DeleteSelf()
    {
      DataPortal_Delete(this.Id);
    }

    //[Transactional(TransactionalTypes.TransactionScope)]
    private void DataPortal_Delete(int criteria)
    {
      // TODO: delete values
    }

    #endregion
  }
}
