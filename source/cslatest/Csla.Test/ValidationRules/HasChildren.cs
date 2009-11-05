using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla.Serialization;
using Csla.Serialization.Mobile;
using Csla.Core;

namespace Csla.Test.ValidationRules
{
  [Serializable]
  public class HasChildren : BusinessBase<HasChildren>
  {
    private static PropertyInfo<int> IdProperty = RegisterProperty(new PropertyInfo<int>("Id", "Id"));
    public int Id
    {
      get { return GetProperty(IdProperty); }
      set { SetProperty(IdProperty, value); }
    }

    private static PropertyInfo<ChildList> ChildListProperty = RegisterProperty(new PropertyInfo<ChildList>("ChildList", "Child list"));
    public ChildList ChildList
    {
      get 
      {
        if (!FieldManager.FieldExists(ChildListProperty))
          LoadProperty(ChildListProperty, ChildList.NewList());
        return GetProperty(ChildListProperty); 
      }
    }

    protected override void AddBusinessRules()
    {
      ValidationRules.AddRule<HasChildren>(OneItem, ChildListProperty);
    }

    private static bool OneItem<T>(T target, Csla.Validation.RuleArgs e)
      where T : HasChildren
    {
      if (target.ChildList.Count < 1)
      {
        e.Description = "At least one item required";
        return false;
      }
      else
        return true;
    }

    protected override void Initialize()
    {
      base.Initialize();
#if (!SILVERLIGHT)
      ChildList.ListChanged += new System.ComponentModel.ListChangedEventHandler(ChildList_ListChanged);
#endif
      this.ChildChanged += new EventHandler<ChildChangedEventArgs>(HasChildren_ChildChanged);
    }

#if (SILVERLIGHT)
    protected override void OnDeserialized()
    {
      base.OnDeserialized();
#else
    protected override void OnDeserialized(System.Runtime.Serialization.StreamingContext context)
    {
      base.OnDeserialized(context);
      ChildList.ListChanged += new System.ComponentModel.ListChangedEventHandler(ChildList_ListChanged);
#endif
      this.ChildChanged += new EventHandler<ChildChangedEventArgs>(HasChildren_ChildChanged);
    }

#if (!SILVERLIGHT)
    void ChildList_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
    {
      //ValidationRules.CheckRules(ChildListProperty);
    }
#endif

    void HasChildren_ChildChanged(object sender, ChildChangedEventArgs e)
    {
      ValidationRules.CheckRules(ChildListProperty);
    }

    public static void NewObject(EventHandler<DataPortalResult<HasChildren>> completed)
    {
      Csla.DataPortal.BeginCreate<HasChildren>(completed);
    }
  }
}
