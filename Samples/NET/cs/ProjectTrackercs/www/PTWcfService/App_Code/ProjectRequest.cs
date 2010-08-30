using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

[DataContract]
public class ProjectRequest
{
  [DataMember]
  public Guid Id { get; set; }
}
