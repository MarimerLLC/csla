using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Csla;
using Csla.Serialization;
using System.ComponentModel.DataAnnotations;

namespace MVVMexperiment
{
  [Serializable]
  public class Data : BusinessBase<Data>
  {
    private static PropertyInfo<int> IdProperty = RegisterProperty<int>(c => c.Id);
    [Range(0, 10, ErrorMessage = "Id must be between 0 and 10")]
    public int Id
    {
      get { return GetProperty(IdProperty); }
      set { SetProperty(IdProperty, value); }
    }

    private static PropertyInfo<string> NameProperty = RegisterProperty<string>(c => c.Name);
    [Required(ErrorMessage = "Name is required")]
    public string Name
    {
      get { return GetProperty(NameProperty); }
      set { SetProperty(NameProperty, value); }
    }

    //protected override void AddBusinessRules()
    //{
    //  ValidationRules.AddDataAnnotations();
    //  //ValidationRules.AddRule(
    //  //  Csla.Validation.CommonRules.MaxValue<int>,
    //  //  new Csla.Validation.CommonRules.MaxValueRuleArgs<int>(IdProperty, 10));
    //}

    private static int _lastId;

    public void Child_Fetch()
    {
      using (BypassPropertyChecks)
      {
        _lastId++;
        Id = _lastId;
        Name = "Name " + _lastId;
      }
    }
  }
}
