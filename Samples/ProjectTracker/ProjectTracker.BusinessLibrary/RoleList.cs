using Csla;
using ProjectTracker.Dal;

namespace ProjectTracker.Library
{
  public class RoleList : NameValueListBase<int, string>
  {
    [Fetch]
    private void Fetch([Inject] IRoleDal dal)
    {
      using (LoadListMode)
      {
        foreach (var item in dal.Fetch())
          Add(new NameValuePair(item.Id, item.Name));
      }
    }
  }
}