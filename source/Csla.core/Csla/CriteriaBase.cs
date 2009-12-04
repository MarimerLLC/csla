using System;
using Csla.Serialization.Mobile;
using Csla.Core;
using System.ComponentModel;
#if SILVERLIGHT
using Csla.Serialization;
#endif

namespace Csla
{
  /// <summary>
  /// Base type from which Criteria classes can be
  /// derived in a business class. 
  /// </summary>
  [Serializable]
  public class CriteriaBase : ManagedObjectBase,
    ICriteria
  {
    /// <summary>
    /// Defines the TypeName property.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)] 
    public static readonly PropertyInfo<string> TypeNameProperty =
      RegisterProperty<CriteriaBase, string>(c => c.TypeName);

    [NonSerialized]
    [NotUndoable]
    private Type _objectType;

    /// <summary>
    /// Type of the business object to be instantiated by
    /// the server-side DataPortal. 
    /// </summary>
    public Type ObjectType
    {
      get
      {
        if (_objectType == null && FieldManager.FieldExists(TypeNameProperty))
        {
          string typeName = ReadProperty(TypeNameProperty);
          if (!string.IsNullOrEmpty(typeName))
          {
            _objectType = Csla.Reflection.MethodCaller.GetType(typeName, false);
          }
        }
        return _objectType;
      }
    }

    /// <summary>
    /// Assembly qualified type name of the business 
    /// object to be instantiated by
    /// the server-side DataPortal. 
    /// </summary>
    public string TypeName
    {
      get { return ReadProperty(TypeNameProperty); }
      protected set { LoadProperty(TypeNameProperty, value); }
    }

#if SILVERLIGHT
    private static bool _forceInit = false;

    /// <summary>
    /// Creates an instance of the object. For use by
    /// MobileFormatter only - you must provide a 
    /// Type parameter in your code.
    /// </summary>
    public CriteriaBase()
    {
      _forceInit = _forceInit && false;
    }

    /// <summary>
    /// Method called by MobileFormatter when an object
    /// should be deserialized. The data should be
    /// deserialized from the SerializationInfo parameter.
    /// </summary>
    /// <param name="info">
    /// Object containing the serialized data.
    /// </param>
    /// <param name="mode">Serialization mode.</param>
    protected override void OnSetState(SerializationInfo info, StateMode mode)
    {
      _forceInit = _forceInit && false;
      base.OnSetState(info, mode);
    }
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
      TypeName = type.AssemblyQualifiedName;
    }
  }
}
