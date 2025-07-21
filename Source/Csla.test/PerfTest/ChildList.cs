namespace Csla.Test.PerfTest;

public class ChildList : BusinessListBase<ChildList, ChildType1>
{
  [FetchChild]
  private async Task Fetch([Inject] IChildDataPortal<ChildType1> portal)
  {
    using (LoadListMode)
      for (var i = 0; i < 10000; i++)
        Add(await portal.FetchChildAsync(i));
  }
}
