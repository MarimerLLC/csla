using Csla;

namespace PropertyPerf.Client.Model;

public class ChildList : BusinessListBase<ChildList, ChildType1>
{
    [FetchChild]
    private async Task Fetch([Inject] IChildDataPortal<ChildType1> portal)
    {
        using (LoadListMode)
        {
            for (int i = 0; i < 10000; i++)
            {
                Add(await portal.FetchChildAsync(i));
            }
        }
    }
}
