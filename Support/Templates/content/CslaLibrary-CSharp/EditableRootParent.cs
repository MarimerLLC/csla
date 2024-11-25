using System;
using Csla;

namespace Company.CslaLibrary1
{
  [Serializable]
  public class EditableRootParent : BusinessBase<EditableRootParent>
  {
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(nameof(Id));
    public int Id
    {
      get { return GetProperty(IdProperty); }
      set { SetProperty(IdProperty, value); }
    }

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    // example with Editable Child list 
    public static readonly PropertyInfo<EditableChildList> ChildListProperty = RegisterProperty<EditableChildList>(nameof(ChildList), "Child list");
    public EditableChildList ChildList
    {
      get { return GetProperty<EditableChildList>(ChildListProperty); }
    }

    // Example with Editable Child 
    public static readonly PropertyInfo<EditableChild> ChildProperty = RegisterProperty<EditableChild>(nameof(Child), "Child");
    public EditableChild Child
    {
      get { return GetProperty<EditableChild>(ChildProperty); }
    }

    protected override void AddBusinessRules()
    {
      // TODO: add validation rules
      //BusinessRules.AddRule(new Rule(), IdProperty);
    }

    private static void AddObjectAuthorizationRules()
    {
      // TODO: add authorization rules
      //BusinessRules.AddRule(...);
    }

    [RunLocal]
    [Create]
    private void Create()
    {
      // TODO: load default values
      // omit this override if you have no defaults to set
      var listDataPortal = ApplicationContext.GetRequiredService<IChildDataPortal<EditableChildList>>();
      LoadProperty(ChildListProperty, listDataPortal.CreateChild());
      var itemDataPortal = ApplicationContext.GetRequiredService<IChildDataPortal<EditableChild>>();
      LoadProperty(ChildProperty, itemDataPortal.CreateChild());
      BusinessRules.CheckRules();
    }

    [Fetch]
    private void Fetch(int id)
    {
      // TODO: load values
      var listDataPortal = ApplicationContext.GetRequiredService<IChildDataPortal<EditableChildList>>();
      LoadProperty(ChildListProperty, listDataPortal.FetchChild(null));
      var itemDataPortal = ApplicationContext.GetRequiredService<IChildDataPortal<EditableChild>>();
      LoadProperty(ChildProperty, itemDataPortal.FetchChild(null));
    }

    [Insert]
    [Transactional(TransactionalTypes.TransactionScope)]
    private void Insert()
    {
      // TODO: insert values
      FieldManager.UpdateChildren(this);
    }

    [Update]
    [Transactional(TransactionalTypes.TransactionScope)]
    private void Update()
    {
      // TODO: update values
      FieldManager.UpdateChildren(this);
    }

    [DeleteSelf]
    [Transactional(TransactionalTypes.TransactionScope)]
    private void DeleteSelf()
    {
      Delete(this.Id);
    }

    [Delete]
    [Transactional(TransactionalTypes.TransactionScope)]
    private void Delete(int id)
    {
      // TODO: delete values
      FieldManager.UpdateChildren(this);
    }
  }
}
