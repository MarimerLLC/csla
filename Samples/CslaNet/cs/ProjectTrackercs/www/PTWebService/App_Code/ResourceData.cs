using System;
using System.Collections.Generic;

public class ResourceData
{
  private int _id;
  private string _name;
  private List<ResourceAssignmentData> _projects = new List<ResourceAssignmentData>();

  public int Id
  {
    get { return _id; }
    set { _id = value; }
  }

  public string Name
  {
    get { return _name; }
    set { _name = value; }
  }

  public void AddProject(ResourceAssignmentData project)
  {
    _projects.Add(project);
  }

  public ResourceAssignmentData[] ResourceAssignments
  {
    get
    {
      if (_projects.Count > 0)
        return _projects.ToArray();
      return null;
    }
    set
    {
      _projects = new List<ResourceAssignmentData>(value);
    }
  }
}
