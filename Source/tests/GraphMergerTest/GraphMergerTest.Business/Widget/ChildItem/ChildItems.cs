using Csla;
using GraphMergerTest.Dal;

namespace GraphMergerTest.Business
{
  [Serializable]
  public class ChildItems
    : BusinessListBase<ChildItems, ChildItem>
  {
    #region Business

    public Widget Widget => Parent as Widget;

    public List<ChildItem> AddNew(IEnumerable<Guid> childItemIds)
    {
      var dataPortal = ApplicationContext.GetRequiredService<IDataPortal<ChildItem.Factory>>();
      var list = new List<ChildItem>();

      foreach (var childItemId in childItemIds)
      {
        var item = GetChildItem(childItemId);

        if (item != null)
          continue;

        item = ChildItem.New(dataPortal, childItemId);

        list.Add(item);
        Add(item);
      }

      return list;
    }

    public ChildItem GetChildItem(Guid childItemId)
    {
      foreach (var item in this)
      {
        if (item.ChildItemId == childItemId)
          return item;
      }

      return null;
    }

    #endregion

    #region Factory

    internal static ChildItems New(IDataPortal<Factory> dataPortal)
    {
      var factory = dataPortal.Create();

      return factory.Result;
    }

    internal static async Task<ChildItems> GetAsync(Guid parentId,
      IChildDataPortal<ChildItems> childDataPortal)
    {
      return await childDataPortal.FetchChildAsync(parentId);
    }

    [Serializable]
    internal class Factory
      : ChildFactoryBase<Factory, ChildItems>
    {
      [Create]
      [RunLocal]
      private void Create([Inject] IChildDataPortal<ChildItems> childDataPortal)
      {
        Result = childDataPortal.CreateChild();
      }
    }

    #endregion

    #region Data access

    [FetchChild]
    private void Fetch(Guid parentId,
      [Inject] IChildItemDal dal,
      [Inject] IChildDataPortal<ChildItem> childDataPortal)
    {
      using (LoadListMode)
      {
        var list = dal.FetchList(parentId);

        var items = new List<ChildItem>();

        foreach (var dto in list)
        {
          var item = childDataPortal.FetchChild(dto);

          Add(item);
        }
      }
    }

    #endregion
  }
}