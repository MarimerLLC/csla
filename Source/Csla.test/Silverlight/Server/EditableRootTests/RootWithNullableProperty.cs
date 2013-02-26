using System;
using Csla.Serialization;

namespace Csla.Testing.Business.EditableRootTests
{
  [Serializable]
  public partial class RootWithNullableProperty : BusinessBase<RootWithNullableProperty>
  {
    public static readonly PropertyInfo<SmartDate?> TodayProperty = RegisterProperty<SmartDate?>(c => c.Today);
    public SmartDate? Today
    {
      get { return GetProperty(TodayProperty); }
      set { SetProperty(TodayProperty, value); }
    }

    public static readonly PropertyInfo<DateTime?> OtherDateProperty = RegisterProperty<DateTime?>(c => c.OtherDate);
    public DateTime? OtherDate
    {
      get { return GetProperty(OtherDateProperty); }
      set { SetProperty(OtherDateProperty, value); }
    }
  }
}
