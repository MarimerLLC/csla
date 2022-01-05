using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Csla;
using Csla.Serialization;

namespace Csla.Test.BasicModern
{
  [Serializable]
  public class Root : BusinessBase<Root>
  {
    public static readonly PropertyInfo<int> IdProperty = RegisterProperty<int>(nameof(Id));
    public int Id
    {
      get { return GetProperty(IdProperty); }
      set { SetProperty(IdProperty, value); }
    }

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(nameof(Name));
    [Required]
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    public static readonly PropertyInfo<ChildList> ChildrenProperty = RegisterProperty<ChildList>(nameof(Children));
    public ChildList Children
    {
      get { return GetProperty(ChildrenProperty); }
      private set { LoadProperty(ChildrenProperty, value); }
    }

    public void MakeOld()
    {
      MarkOld();
    }

    [Create]
	protected void DataPortal_Create([Inject] IChildDataPortal<ChildList> childDataPortal)
    {
      Children = childDataPortal.CreateChild();
      BusinessRules.CheckRules();
    }
    
    private void DataPortal_Fetch(int id, [Inject] IChildDataPortal<ChildList> childDataPortal)
    {
      Children = childDataPortal.CreateChild();
    }

    [Insert]
    protected void DataPortal_Insert()
    {
      FieldManager.UpdateChildren();
    }

    [Update]
		protected void DataPortal_Update()
    {
      FieldManager.UpdateChildren();
    }

    [DeleteSelf]
    protected void DataPortal_DeleteSelf()
    {
      DataPortal_Delete(ReadProperty(IdProperty));
    }

    [Delete]
		private void DataPortal_Delete(int id)
    {
      FieldManager.UpdateChildren();
    }
  }
}
