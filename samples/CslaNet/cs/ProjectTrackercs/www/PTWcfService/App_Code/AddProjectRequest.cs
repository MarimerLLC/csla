using System;
using System.Runtime.Serialization;

[DataContract]
public class AddProjectRequest
{
  [DataMember]
  public ProjectData ProjectData { get; set; }
}
