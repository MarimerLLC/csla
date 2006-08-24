using System;

namespace Csla
{

  /// <summary>
  /// Base type from which Criteria classes can be
  /// derived in a business class. 
  /// </summary>
  [Serializable()]
  public abstract class CriteriaBase
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
    /// Initializes CriteriaBase with the type of
    /// business object to be created by the DataPortal.
    /// </summary>
    /// <param name="type">The type of the
    /// business object the data portal should create.</param>
    protected CriteriaBase(Type type)
    {
      _objectType = type;
    }
  }
}
