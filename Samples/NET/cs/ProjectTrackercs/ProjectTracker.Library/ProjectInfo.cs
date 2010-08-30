using System;
using Csla;
using Csla.Serialization;

namespace ProjectTracker.Library
{
  [Serializable()]
  public partial class ProjectInfo : ReadOnlyBase<ProjectInfo>
  {
    public Guid Id { get; private set; }
    public string Name { get; private set; }

    public override string ToString()
    {
      return Name;
    }

    private ProjectInfo()
    { /* require use of factory methods */ }

    internal ProjectInfo(Guid id, string name)
    {
      Id = id;
      Name = name;
    }
  }
}