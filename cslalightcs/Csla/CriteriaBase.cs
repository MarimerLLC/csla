using System;
#if SILVERLIGHT
using Csla.Serialization;
using Csla.Serialization.Mobile;
using Csla.Core;
#endif

namespace Csla
{
  /// <summary>
  /// Base type from which Criteria classes can be
  /// derived in a business class. 
  /// </summary>
  [Serializable]
  public class CriteriaBase : Csla.Core.MobileObject, ICriteria
  {
    private Type _objectType;

    /// <summary>
    /// Type of the business object to be instantiated by
    /// the server-side DataPortal. 
    /// </summary>
    public Type ObjectType
    {
      get { return _objectType; }
      protected set { _objectType = value; }
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

#if SILVERLIGHT
    /// <summary>
    /// Creates an instance of the object. For use by
    /// MobileFormatter only - you must provide a 
    /// Type parameter in your code.
    /// </summary>
    public CriteriaBase() 
    { }
#else 
    /// <summary>
    /// Initializes empty CriteriaBase. The type of
    /// business object to be created by the DataPortal
    /// MUST be supplied by the subclass.
    /// </summary>
    protected CriteriaBase()
    { }
#endif

    /// <summary>
    /// Initializes CriteriaBase with the type of
    /// business object to be created by the DataPortal.
    /// </summary>
    /// <param name="type">The type of the
    /// business object the data portal should create.</param>
    protected CriteriaBase(Type type)
    {
      _objectType = type;
    }

    #region MobileFormatter

    /// <summary>
    /// Override this method to insert your field values
    /// into the MobileFormatter serialzation stream.
    /// </summary>
    /// <param name="info">
    /// Object containing the data to serialize.
    /// </param>
    protected override void OnGetState(SerializationInfo info, StateMode mode)
    {
      base.OnGetState(info, mode);
      info.AddValue("Csla.Silverlight.CriteriaBase.typeName", TypeName);
    }

    /// <summary>
    /// Override this method to retrieve your field values
    /// from the MobileFormatter serialzation stream.
    /// </summary>
    /// <param name="info">
    /// Object containing the data to serialize.
    /// </param>
    protected override void OnSetState(SerializationInfo info, StateMode mode)
    {
      base.OnSetState(info, mode);
      string typeName = info.GetValue<string>("Csla.Silverlight.CriteriaBase.typeName"); //info.Values["Csla.Silverlight.CriteriaBase.typeName"].Value.ToString();
      _objectType = Type.GetType(typeName);
    }

    #endregion
  }
}
