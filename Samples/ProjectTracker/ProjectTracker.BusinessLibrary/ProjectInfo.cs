using System;
using System.ComponentModel.DataAnnotations;
using Csla;

namespace ProjectTracker.Library
{
  [Serializable]
  public class ProjectInfo : ReadOnlyBase<ProjectInfo>
  {
    public static readonly PropertyInfo<int> IdProperty = 
      RegisterProperty<int>(c => c.Id);
    [Display(Name = "Project id")]
    public int Id
    {
      get { return GetProperty(IdProperty); }
      private set { LoadProperty(IdProperty, value); }
    }

    public static readonly PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    [Display(Name = "Project name")]
#pragma warning disable CSLA0007 // Properties that use managed backing fields should only use Get/Set/Read/Load methods and nothing else
    public string Name
    {
      get { return GetProperty(NameProperty) ?? string.Empty; }
      private set { LoadProperty(NameProperty, value); }
    }
#pragma warning restore CSLA0007

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
      Name = item.Name;
    }
  }
}