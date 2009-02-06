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
    private static bool _forceInit = false;

    /// <summary>
    /// Defines the TypeName property.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)] 
    public static readonly PropertyInfo<string> TypeNameProperty =
      RegisterProperty<CriteriaBase, string>(c => c.TypeName);
      //typeof(CriteriaBase),
      //new PropertyInfo<string>("TypeName"));

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
            _objectType = Type.GetType(typeName, false);
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
    /// <summary>
    /// Creates an instance of the object. For use by
    /// MobileFormatter only - you must provide a 
    /// Type parameter in your code.
    /// </summary>
    [Obsolete("For use by MobileFormatter only")]
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

    [System.Runtime.Serialization.OnDeserialized()]
    private void OnDeserializedHandler(System.Runtime.Serialization.StreamingContext context)
    {
      OnDeserialized(context);
    }

    /// <summary>
    /// This method is called on a newly deserialized object
    /// after deserialization is complete.
    /// </summary>
    /// <param name="context">Serialization context object.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnDeserialized(System.Runtime.Serialization.StreamingContext context)
    {
      _forceInit = _forceInit && false;
      if (FieldManager != null)
        FieldManager.SetPropertyList(this.GetType());
    }

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
