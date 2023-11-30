using System.Collections.Generic;
using GraphMergerTest.Dal;

namespace GraphMergerTest.DalMock
{
    public static class MockDb
    {
        public static List<WidgetDto>    Widgets    { get; private set; } = new List<WidgetDto>();
        public static List<ChildItemDto> ChildItems { get; private set; } = new List<ChildItemDto>();
    }
}
