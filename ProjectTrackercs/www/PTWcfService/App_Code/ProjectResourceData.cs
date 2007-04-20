using System;
using System.Runtime.Serialization;

[DataContract]
public class ProjectResourceData
{
  private int _resourceId;
  private string _firstName;
  private string _lastName;
  private string _assigned;
  private int _role;

  [DataMember]
  public int ResourceId
  {
    get { return _resourceId; }
    set { _resourceId = value; }
  }

  [DataMember]
  public string FirstName
  {
    get { return _firstName; }
    set { _firstName = value; }
  }

  [DataMember]
  public string LastName
  {
    get { return _lastName; }
    set { _lastName = value; }
  }

  [DataMember]
  public string Assigned
  {
    get { return _assigned; }
    set { _assigned = value; }
  }

  [DataMember]
  public int Role
  {
    get { return _role; }
    set { _role = value; }
  }	
}
