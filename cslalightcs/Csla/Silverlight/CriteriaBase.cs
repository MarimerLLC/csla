using System;
using Csla.Serialization;
using Csla.Core.FieldManager;

namespace Csla.Silverlight
{
  [Serializable]
  public class CriteriaBase : Csla.ReadOnlyBase<CriteriaBase>
  {
    private Type _objectType;

    public string TypeName
    {
      get 
      {
        if (_objectType != null)
          return _objectType.FullName + "," + _objectType.Assembly.FullName;
        else
          return null;
      }
    }

    public CriteriaBase() { }

    public CriteriaBase(Type objectType)
    {
      _objectType = objectType;
    }

    protected override void OnGetState(Csla.Serialization.Mobile.SerializationInfo info)
    {
      base.OnGetState(info);
      info.AddValue("Csla.Silverlight.CriteriaBase.typeName", TypeName);
    }

    protected override void OnSetState(Csla.Serialization.Mobile.SerializationInfo info)
    {
      base.OnSetState(info);
      string typeName = info.Values["Csla.Silverlight.CriteriaBase.typeName"].ToString();
      _objectType = Type.GetType(typeName);
    }
  }
}
