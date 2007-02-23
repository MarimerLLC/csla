using System;
using System.Collections.Generic;

public class ProjectData
{
  private Guid _id;
  private string _name;
  private string _started;
  private string _ended;
  private string _description;
  private List<ProjectResourceData> 
    _resources = new List<ProjectResourceData>();

  public Guid Id
  {
    get { return _id; }
    set { _id = value; }
  }

  public string Name
  {
    get { return _name; }
    set { _name = value; }
  }

  public string Started
  {
    get { return _started; }
    set { _started = value; }
  }

  public string Ended
  {
    get { return _ended; }
    set { _ended = value; }
  }

  public string Description
  {
    get { return _description; }
    set { _description = value; }
  }

  public void AddResource(ProjectResourceData resource)
  {
    _resources.Add(resource);
  }

  public ProjectResourceData[] ProjectResources
  {
    get
    {
      if (_resources.Count > 0)
        return _resources.ToArray();
      return null;
    }
    set
    {
      _resources = new List<ProjectResourceData>(value);
    }
  }
}
