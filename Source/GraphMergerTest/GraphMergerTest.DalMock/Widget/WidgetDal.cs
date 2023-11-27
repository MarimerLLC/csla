using System;
using System.Linq;
using GraphMergerTest.Dal;

namespace GraphMergerTest.DalMock
{
    public class WidgetDal
        : IWidgetDal
    {
        public WidgetDto Fetch(Guid id)
        {
            var dto = (from   w in MockDb.Widgets
                       where  w.Id == id
                       select new WidgetDto
                       {
                           Id = w.Id,
                       }).Single();

            return dto;
        }

        public void Insert(WidgetDto dto)
        {
            var data = new WidgetDto();

            SetData(data, dto);

            MockDb.Widgets.Add(data);
        }

        public void Update(WidgetDto dto)
        {
            var data = (from   w in MockDb.Widgets
                        where  w.Id == dto.Id
                        select w).First();

            SetData(data, dto);
        }

        private void SetData(WidgetDto data, WidgetDto dto)
        {
            data.Id = dto.Id;
        }
    }
}
