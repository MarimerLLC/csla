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
    public static readonly PropertyInfo<string> TypeNameProperty = RegisterProperty(
      typeof(CriteriaBase),
      new PropertyInfo<string>("TypeName"));

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

    [System.Runtime.Serialization.OnDeserialized()]
    private void OnDeserializedHandler(System.Runtime.Serialization.StreamingContext context)
    {
      OnDeserialized(context);
      OnDeserializedInternal();
    }

    /// <summary>
    /// Override this method to perform custom opertaions
    /// after the object has been deserialized on the server or client
    /// </summary>
    protected virtual void OnDeserializedInternal()
    {
      _forceInit = true;
      if (FieldManager != null)
        FieldManager.SetPropertyList(this.GetType());
    }

    /// <summary>
    /// This method is called on a newly deserialized object
    /// after deserialization is complete.
    /// </summary>
    /// <param name="context">Serialization context object.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnDeserialized(System.Runtime.Serialization.StreamingContext context)
    {
      // do nothing - this is here so a subclass
      // could override if needed
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
