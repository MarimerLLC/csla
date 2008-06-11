using System;
using Csla.Serialization;
using Csla.Core.FieldManager;

namespace Csla.Silverlight
{
  [Serializable]
  public class CriteriaBase : MobileObject
  {
    static CriteriaBase()
    {
      TypeNameProperty = PropertyInfoManager.RegisterProperty<string>(
        typeof(CriteriaBase),
        new PropertyInfo<string>("TypeName"));
    }

    public static PropertyInfo<string> TypeNameProperty;

    public string TypeName
    {
      get
      {
        return GetProperty<string>(TypeNameProperty);
      }
      protected set
      {
        SetProperty<string>(TypeNameProperty, value);
      }
    }

    public CriteriaBase() { }
    public CriteriaBase(Type objectType)
    {
      this.TypeName = objectType.FullName + "," + objectType.Assembly.FullName;
    }
  }
}
