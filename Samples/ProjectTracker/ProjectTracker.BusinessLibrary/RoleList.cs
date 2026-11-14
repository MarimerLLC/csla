using Csla;
using ProjectTracker.Dal;

namespace ProjectTracker.Library
{
  public partial class RoleList : NameValueListBase<int, string>
  {
    /// <summary>
    /// Gets the id of the role assigned by default: the first role
    /// in the list, or 0 if the list is empty.
    /// </summary>
    public int DefaultRole => Count > 0 ? this[0].Key : 0;

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