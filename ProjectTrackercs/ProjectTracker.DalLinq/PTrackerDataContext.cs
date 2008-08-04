using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Data;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel;
using System;

namespace ProjectTracker.DalLinq
{
  public partial class PTrackerDataContext
  {
    [Function(Name = "dbo.getProject")]
    [ResultType(typeof(Project))]
    [ResultType(typeof(Assignment))]
    public IMultipleResults getProject([Parameter(DbType = "UniqueIdentifier")] System.Nullable<System.Guid> id)
    {
      IExecuteResult result = this.ExecuteMethodCall(this, ((MethodInfo)(MethodInfo.GetCurrentMethod())), id);
      return ((IMultipleResults)(result.ReturnValue));
    }

  }
}
