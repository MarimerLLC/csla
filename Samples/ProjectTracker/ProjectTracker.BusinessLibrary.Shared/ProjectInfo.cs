using System;
//using System.ComponentModel.DataAnnotations;
using Csla;
using Csla.Serialization;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class ProjectInfo : ReadOnlyBase<ProjectInfo>
  {
    public static PropertyInfo<int> IdProperty = 
      RegisterProperty<int>(c => c.Id);
    //[Display(Name = "Project id")]
    public int Id
    {
      get { return GetProperty(IdProperty); }
      private set { LoadProperty(IdProperty, value); }
    }

    public static PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    //[Display(Name = "Project name")]
    public string Name
    {
      get { return GetProperty(NameProperty); }
      private set { LoadProperty(NameProperty, value); }
    }

    public void SetName(ProjectEdit item)
    {
      Name = item.Name;
      OnPropertyChanged(NameProperty);
    }

    public override string ToString()
    {
      return Name;
    }

#if FULL_DOTNET
    private void Child_Fetch(ProjectTracker.Dal.ProjectDto item)
    {
      Id = item.Id;
      Name = item.Name;
    }
#endif
  }
}