using System;

namespace GraphMergerTest.Dal
{
    public interface IWidgetDal
    {
        WidgetDto Fetch (Guid      id);
        void      Insert(WidgetDto dto);
        void      Update(WidgetDto dto);
    }
}
