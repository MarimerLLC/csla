using System;
using System.Runtime.Serialization;

[DataContract]
public class UpdateProjectRequest
{
  [DataMember]
  public ProjectData ProjectData { get; set; }
}
