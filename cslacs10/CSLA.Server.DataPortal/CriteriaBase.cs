using System;

namespace CSLA
{
  /// <summary>
  /// Base type from which Criteria classes can be
  /// derived in a business class. 
  /// </summary>
  [Serializable()]
  public class CriteriaBase
  {
    /// <summary>
    /// Type of the business object to be instantiated by
    /// the server-side DataPortal. 
    /// </summary>
    public Type ObjectType;
                        
    /// <summary>
    /// Initializes CriteriaBase with the type of
    /// business object to be created by the DataPortal.
    /// </summary>
    public CriteriaBase(Type type)
    {
      ObjectType = type;
    }
  }
}
