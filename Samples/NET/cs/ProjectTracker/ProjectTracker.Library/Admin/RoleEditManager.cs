#if !SILVERLIGHT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;

namespace ProjectTracker.Library.Admin
{
  [Serializable]
  public class RoleEditManager : CommandBase<RoleEditManager>
  {
    public static RoleEdit GetRoleEdit(int id)
    {
      var cmd = new RoleEditManager { Id = id };
      cmd = DataPortal.Execute<RoleEditManager>(cmd);
      return cmd.RoleEdit;
    }

    public static RoleEdit SaveRoleEdit(RoleEdit role, bool forceUpdate)
    {
      if (forceUpdate)
        role.MarkOld();
      var cmd = new RoleEditManager { RoleEdit = role };
      cmd = DataPortal.Execute<RoleEditManager>(cmd);
      return cmd.RoleEdit;
    }

    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id);
    private int Id
    {
      get { return ReadProperty(IdProperty); }
      set { LoadProperty(IdProperty, value); }
    }

    public static readonly PropertyInfo<RoleEdit> RoleEditProperty = RegisterProperty<RoleEdit>(c => c.RoleEdit);
    private RoleEdit RoleEdit
    {
      get { return ReadProperty(RoleEditProperty); }
      set { LoadProperty(RoleEditProperty, value); }
    }

    protected override void DataPortal_Execute()
    {
      if (Id > 0)
        GetRoleEdit();
      else
        SaveRoleEdit();
    }

    private void GetRoleEdit()
    {
      using (var ctx = ProjectTracker.Dal.DalFactory.GetManager())
      {
        var dal = ctx.GetProvider<ProjectTracker.Dal.IRoleDal>();
        var item = dal.Fetch(Id);
        RoleEdit = DataPortal.FetchChild<RoleEdit>(item);
      }
    }

    private void SaveRoleEdit()
    {
      var list = RoleEditList.GetRoles();
      if (RoleEdit.IsNew)
        list.Add(RoleEdit);
      else
        list.GetRoleById(RoleEdit.Id).Name = RoleEdit.Name;
      list.Save();
    }
  }
}
#endif
