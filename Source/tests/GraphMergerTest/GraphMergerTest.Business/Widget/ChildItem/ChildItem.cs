using System.ComponentModel;
using Csla;
using GraphMergerTest.Dal;

namespace GraphMergerTest.Business
{
  [Serializable]
  public class ChildItem
    : BusinessBase<ChildItem>
  {
    #region Business

    public static readonly PropertyInfo<Guid> WidgetIdProperty
      = RegisterProperty<Guid>(nameof(WidgetId));

    [DataObjectField(true, false)]
    public Guid WidgetId
    {
      get => GetProperty(WidgetIdProperty);
      set => SetProperty(WidgetIdProperty, value);
    }

    public static readonly PropertyInfo<Guid> ChildItemIdProperty
      = RegisterProperty<Guid>(nameof(ChildItemId));

    [DataObjectField(true, false)]
    public Guid ChildItemId
    {
      get => GetProperty(ChildItemIdProperty);
      set => SetProperty(ChildItemIdProperty, value);
    }

    public override string ToString()
    {
      return ChildItemId.ToString();
    }

    #endregion

    #region Factory

    internal static ChildItem New(IDataPortal<Factory> dataPortal, Guid childItemId)
    {
      var factory = dataPortal.Create(childItemId);

      return factory.Result;
    }

    internal static ChildItem Get(IChildDataPortal<ChildItem> childDataPortal, ChildItemDto dto)
    {
      return childDataPortal.FetchChild(dto);
    }

    [Serializable]
    internal class Factory
      : ChildFactoryBase<Factory, ChildItem>
    {
      [Create]
      [RunLocal]
      private void Create([Inject] IChildDataPortal<ChildItem> childDataPortal, Guid childItemId)
      {
        Result = childDataPortal.CreateChild(childItemId);
      }
    }

    #endregion

    #region Data access

    [Create]
    [CreateChild]
    private void Create(Guid childItemId)
    {
      using (BypassPropertyChecks)
      {
        ChildItemId = childItemId;
      }

      BusinessRules.CheckRules();
    }

    [FetchChild]
    private void Fetch(ChildItemDto dto)
    {
      using (BypassPropertyChecks)
      {
        FromDto(dto);
      }
    }

    [InsertChild]
    private void Insert(Widget parent, [Inject] IChildItemDal dal)
    {
      using (BypassPropertyChecks)
      {
        WidgetId = parent.Id;

        var dto = ToDto();

        dal.Insert(dto);
      }

      FieldManager.UpdateChildren(this);
    }

    [DeleteSelfChild]
    private void DeleteSelf([Inject] IChildItemDal dal)
    {
      using (BypassPropertyChecks)
        Delete(WidgetId, ChildItemId, dal);
    }

    [Delete]
    private void Delete(Guid parentId, Guid childItemId, [Inject] IChildItemDal dal)
    {
      dal.Delete(parentId, childItemId);
    }

    #endregion

    #region DTO

    protected void FromDto(ChildItemDto dto)
    {
      WidgetId = dto.WidgetId;
      ChildItemId = dto.ChildItemId;
    }

    protected ChildItemDto ToDto()
    {
      return new ChildItemDto
      {
        WidgetId = WidgetId,
        ChildItemId = ChildItemId
      };
    }

    #endregion
  }
}