using System;
using System.Runtime.Serialization;

[DataContract]
public class ProjectListRequest
{
  [DataMember]
  public string Name { get; set; }
}
