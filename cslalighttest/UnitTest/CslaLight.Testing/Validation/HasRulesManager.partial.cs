using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Csla.DataPortalClient;
using Csla.Serialization.Mobile;
using Csla.Core;

namespace Csla.Test.ValidationRules
{
  public partial class HasRulesManager
  {
    public HasRulesManager()
      : base()
    { }

    public void DataPortal_Create(LocalProxy<HasRulesManager>.CompletedHandler completed, object criteria)
    {
      Criteria crit = (Criteria)(criteria);
      _name = crit._name;
      //Name = crit._name;
      Csla.ApplicationContext.GlobalContext.Add("HasRulesManager", "Created");
      this.ValidationRules.CheckRules();

      completed(this, null);
    }

    #if !SILVERLIGHT

    protected override void DataPortal_Fetch(object criteria)
    {
      Criteria crit = (Criteria)(criteria);
      _name = crit._name;
      MarkOld();
      Csla.ApplicationContext.GlobalContext.Add("HasRulesManager", "Fetched");
    }

    protected override void DataPortal_Update()
    {
      if (IsDeleted)
      {
        //we would delete here
        Csla.ApplicationContext.GlobalContext.Add("HasRulesManager", "Deleted");
        MarkNew();
      }
      else
      {
        if (this.IsNew)
        {
          //we would insert here
          Csla.ApplicationContext.GlobalContext.Add("HasRulesManager", "Inserted");
        }
        else
        {
          //we would update here
          Csla.ApplicationContext.GlobalContext.Add("HasRulesManager", "Updated");
        }
        MarkOld();
      }
    }

    protected override void DataPortal_Delete(object criteria)
    {
      //we would delete here
      Csla.ApplicationContext.GlobalContext.Add("HasRulesManager", "Deleted");
    }

#endif

    protected override void OnGetState(SerializationInfo info, StateMode mode)
    {
      info.AddValue("Name", Name);
      base.OnGetState(info, mode);
    }

    protected override void OnSetState(SerializationInfo info, StateMode mode)
    {
      Name = info.GetValue<string>("Name");
      base.OnSetState(info, mode);
    }
  }
}
