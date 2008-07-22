using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;

namespace DataBinding.Business
{
  [Serializable]
  public partial class Customer : BusinessBase<Customer>
  {
    private Customer() { }

    #region Properties

    private static PropertyInfo<int> IdProperty = RegisterProperty(new PropertyInfo<int>("Id"));
    private static PropertyInfo<string> NameProperty = RegisterProperty(new PropertyInfo<string>("Name"));
    private static PropertyInfo<DateTime> BirthDateProperty = RegisterProperty(new PropertyInfo<DateTime>("BirthDate"));

    public int Id
    {
      get { return GetProperty(IdProperty); }
    }

    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    public DateTime BirthDate
    {
      get { return GetProperty(BirthDateProperty); }
      set { SetProperty(BirthDateProperty, value); }
    } 

    #endregion

    #region Object overrides

    public override string ToString()
    {
      return Name;
    } 

    #endregion
  }
}
