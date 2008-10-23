using System;
using System.Runtime.Serialization;

[DataContract]
public class RoleData
{
  [DataMember]
  public int Key { get; set; }
  [DataMember]
  public string Value { get; set; }
}
