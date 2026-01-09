using System.ComponentModel.DataAnnotations;
using Csla;

namespace ProjectTracker.Library
{
  [CslaImplementProperties]
  public partial class ResourceInfo : ReadOnlyBase<ResourceInfo>
  {
    [Display(Name = "Resource id")]
    public partial int Id { get; private set; }

    [Display(Name = "Resource name")]
    public partial string Name { get; private set; }

    public void SetName(ResourceEdit item)
    {
      Name = string.Format("{1}, {0}", item.FirstName, item.LastName);
      OnPropertyChanged(NameProperty);
    }

    public override string ToString()
    {
      return Name;
    }

    [FetchChild]
    private void Fetch(ProjectTracker.Dal.ResourceDto item)
    {
      Id = item.Id;
      Name = string.Format("{1}, {0}", item.FirstName, item.LastName);
    }
  }
}