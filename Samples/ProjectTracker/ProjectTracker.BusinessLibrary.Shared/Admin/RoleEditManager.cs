#if !NETFX_CORE
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
    public static RoleEdit NewRoleEdit()
    {
      return DataPortal.CreateChild<RoleEdit>();
    }

    public static RoleEdit GetRoleEdit(int id)
    {
      var cmd = new RoleEditManager { Id = id };
      cmd = DataPortal.Execute<RoleEditManager>(cmd);
      return cmd.RoleEdit;
    }

    public static RoleEdit SaveRoleEdit(RoleEdit role)
    {
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
      var item = list.GetRoleById(RoleEdit.Id);
      if (item == null)
        list.Add(RoleEdit);
      else
        item.Name = RoleEdit.Name;
      list.Save();
    }
  }
}
#endif
