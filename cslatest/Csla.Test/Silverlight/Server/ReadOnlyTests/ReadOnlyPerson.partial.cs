using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.Testing.Business.ReadOnlyTest
{
  public partial class ReadOnlyPerson
  {
    private ReadOnlyPerson() { }

    private void DataPortal_Fetch(SingleCriteria<ReadOnlyPerson, Guid> criteria)
    {
      LoadProperty<Guid>(IdProperty, criteria.Value);
      LoadProperty(NameProperty, criteria.Value.ToString());
      LoadProperty(BirthdateProperty, new DateTime(1980, 1, 1));
    }
  }
}
