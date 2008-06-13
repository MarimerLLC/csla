using System;
using Csla.Serialization;
using Csla.Core.FieldManager;

namespace Csla.Silverlight
{
  /// <summary>
  /// Base type from which Criteria classes can be
  /// derived in a business class. 
  /// </summary>
  [Serializable]
  public class CriteriaBase : Csla.ReadOnlyBase<CriteriaBase>
  {
    private Type _objectType;

    /// <summary>
    /// Type of the business object to be instantiated by
    /// the server-side DataPortal. 
    /// </summary>
    public Type ObjectType
    {
      get { return _objectType; }
    }

    /// <summary>
    /// Assembly qualified type name of the business 
    /// object to be instantiated by
    /// the server-side DataPortal. 
    /// </summary>
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

    /// <summary>
    /// Creates an instance of the object. For use by
    /// MobileFormatter only - you must provide a 
    /// Type parameter in your code.
    /// </summary>
    public CriteriaBase() { }

    /// <summary>
    /// Initializes CriteriaBase with the type of
    /// business object to be created by the DataPortal.
    /// </summary>
    /// <param name="type">The type of the
    /// business object the data portal should create.</param>
    public CriteriaBase(Type objectType)
    {
      _objectType = objectType;
    }

    #region MobileFormatter

    /// <summary>
    /// Override this method to insert your field values
    /// into the MobileFormatter serialzation stream.
    /// </summary>
    /// <param name="info">
    /// Object containing the data to serialize.
    /// </param>
    protected override void OnGetState(Csla.Serialization.Mobile.SerializationInfo info)
    {
      base.OnGetState(info);
      info.AddValue("Csla.Silverlight.CriteriaBase.typeName", TypeName);
    }

    /// <summary>
    /// Override this method to retrieve your field values
    /// from the MobileFormatter serialzation stream.
    /// </summary>
    /// <param name="info">
    /// Object containing the data to serialize.
    /// </param>
    protected override void OnSetState(Csla.Serialization.Mobile.SerializationInfo info)
    {
      base.OnSetState(info);
      string typeName = info.GetValue<string>("Csla.Silverlight.CriteriaBase.typeName"); //info.Values["Csla.Silverlight.CriteriaBase.typeName"].Value.ToString();
      _objectType = Type.GetType(typeName);
    }

    #endregion
  }
}
