using System.ComponentModel.DataAnnotations;
using Csla;

namespace ProjectTracker.Library
{
  [CslaImplementProperties]
  public partial class ProjectInfo : ReadOnlyBase<ProjectInfo>
  {
    [Display(Name = "Project id")]
    public partial int Id { get; private set; }

    [Display(Name = "Project name")]
    public partial string Name { get; private set; }

    public void SetName(ProjectEdit item)
    {
      Name = item.Name;
      OnPropertyChanged(NameProperty);
    }

    public override string ToString()
    {
      return Name;
    }

    [FetchChild]
    private void Fetch(ProjectTracker.Dal.ProjectDto item)
    {
      Id = item.Id;
      Name = item.Name ?? string.Empty;
    }
  }
}