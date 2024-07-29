using GraphMergerTest.Dal;

namespace GraphMergerTest.DalMock
{
  public class ChildItemDal
    : IChildItemDal
  {
    public void Insert(ChildItemDto dto)
    {
      var data = new ChildItemDto();

      SetData(data, dto);

      MockDb.ChildItems.Add(data);
    }

    private void SetData(ChildItemDto data, ChildItemDto dto)
    {
      data.WidgetId = dto.WidgetId;
      data.ChildItemId = dto.ChildItemId;
    }

    public void Delete(Guid widgetId, Guid childItemId)
    {
      var data = (from x in MockDb.ChildItems
                  where x.WidgetId == widgetId
                        && x.ChildItemId == childItemId
                  select x).FirstOrDefault();

      if (data != null)
        MockDb.ChildItems.Remove(data);
    }

    public List<ChildItemDto> FetchList(Guid widgetId)
    {
      var list = (from x in MockDb.ChildItems
                  where x.WidgetId == widgetId
                  select new ChildItemDto
                  {
                    WidgetId = x.WidgetId,
                    ChildItemId = x.ChildItemId,
                  }).ToList();

      return list;
    }
  }
}