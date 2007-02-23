using System;

public class ResourceAssignmentData
{
  private Guid _projectId;
  private string _projectName;
  private string _assigned;
  private int _role;

  public Guid ProjectId
  {
    get { return _projectId; }
    set { _projectId = value; }
  }

  public string ProjectName
  {
    get { return _projectName; }
    set { _projectName = value; }
  }

  public string Assigned
  {
    get { return _assigned; }
    set { _assigned = value; }
  }

  public int Role
  {
    get { return _role; }
    set { _role = value; }
  }
}
