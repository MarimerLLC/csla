using System;
using System.Linq;
using Csla;
using Csla.Serialization;

namespace ProjectTracker.Library
{
  [Serializable()]
  public partial class ProjectList : ReadOnlyListBase<ProjectList, ProjectInfo>
  {
  }
}