using System;
using System.Collections.Generic;

namespace GraphMergerTest.Dal
{
    public interface IChildItemDal
    {
        void               Insert   (ChildItemDto dto);
        void               Delete   (Guid         parentId, Guid childItemId);
        List<ChildItemDto> FetchList(Guid         parentId);
    }
}
