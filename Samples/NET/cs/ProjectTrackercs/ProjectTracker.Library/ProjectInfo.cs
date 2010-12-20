using System;
using System.ComponentModel.DataAnnotations;
using Csla;
using Csla.Serialization;

namespace ProjectTracker.Library
{
  [Serializable()]
  public class ProjectInfo : ReadOnlyBase<ProjectInfo>
  {
    public static PropertyInfo<Guid> IdProperty = RegisterProperty<Guid>(c => c.Id);
    [Display(Name = "Project id")]
    public Guid Id
    {
      get { return GetProperty(IdProperty); }
      private set { LoadProperty(IdProperty, value); }
    }

    public static PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    [Display(Name = "Project name")]
    public string Name
    {
      get { return GetProperty(NameProperty); }
      private set { LoadProperty(NameProperty, value); }
    }

    public override string ToString()
    {
      return Name;
    }
  }
}