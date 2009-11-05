using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

[DataContract]
public class ProjectData
{
  [DataMember]
  public Guid Id { get; set; }
  [DataMember]
  public string Name { get; set; }
  [DataMember]
  public string Started { get; set; }
  [DataMember]
  public string Ended { get; set; }
  [DataMember]
  public string Description { get; set; }
  [DataMember]
  public List<ProjectResourceData> ProjectResources { get; set; }

  public ProjectData()
  {
    this.ProjectResources = new List<ProjectResourceData>();
  }
}
