//-----------------------------------------------------------------------
// <copyright file="ITrackStatus.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Defines the common properties required objects</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Core
{
  /// <summary>
  /// Defines the common properties required objects
  /// that track their own status.
  /// </summary>
  public interface ITrackStatus : INotifyBusy
  {
    /// <summary>
    /// Returns true if the object 
    /// and its child objects are currently valid, 
    /// false if the
    /// object or any of its child objects have broken 
    /// rules or are otherwise invalid.
    /// </summary>
    /// <remarks>
    /// <para>
    /// By default this property relies on the underling ValidationRules
    /// object to track whether any business rules are currently broken for this object.
    /// </para><para>
    /// You can override this property to provide more sophisticated
    /// implementations of the behavior. For instance, you should always override
    /// this method if your object has child objects, since the validity of this object
    /// is affected by the validity of all child objects.
    /// </para>
    /// </remarks>
    /// <returns>A value indicating if the object is currently valid.</returns>
    bool IsValid { get; }
    /// <summary>
    /// Returns true if the object is currently 
    /// valid, false if the
    /// object has broken rules or is otherwise invalid.
    /// </summary>
    /// <remarks>
    /// <para>
    /// By default this property relies on the underling ValidationRules
    /// object to track whether any business rules are currently broken for this object.
    /// </para><para>
    /// You can override this property to provide more sophisticated
    /// implementations of the behavior.
    /// </para>
    /// </remarks>
    /// <returns>A value indicating if the object is currently valid.</returns>
    bool IsSelfValid { get; }
    /// <summary>
    /// Returns true if this object's 
    /// data, or any of its fields or child objects data, 
    /// has been changed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When an object's data is changed, CSLA .NET makes note of that change
    /// and considers the object to be 'dirty' or changed. This value is used to
    /// optimize data updates, since an unchanged object does not need to be
    /// updated into the database. All new objects are considered dirty. All objects
    /// marked for deletion are considered dirty.
    /// </para><para>
    /// Once an object's data has been saved to the database (inserted or updated)
    /// the dirty flag is cleared and the object is considered unchanged. Objects
    /// newly loaded from the database are also considered unchanged.
    /// </para>
    /// </remarks>
    /// <returns>A value indicating if this object's data has been changed.</returns>
    bool IsDirty { get; }
    /// <summary>
    /// Returns true if this object's data has been changed.
    /// </summary>
    /// <remarks>
    /// <para>
    /// When an object's data is changed, CSLA .NET makes note of that change
    /// and considers the object to be 'dirty' or changed. This value is used to
    /// optimize data updates, since an unchanged object does not need to be
    /// updated into the database. All new objects are considered dirty. All objects
    /// marked for deletion are considered dirty.
    /// </para><para>
    /// Once an object's data has been saved to the database (inserted or updated)
    /// the dirty flag is cleared and the object is considered unchanged. Objects
    /// newly loaded from the database are also considered unchanged.
    /// </para>
    /// </remarks>
    /// <returns>A value indicating if this object's data has been changed.</returns>
    bool IsSelfDirty { get; }
    /// <summary>
    /// Returns true if this object is marked for deletion.
    /// </summary>
    /// <remarks>
    /// CSLA .NET supports both immediate and deferred deletion of objects. This
    /// property is part of the support for deferred deletion, where an object
    /// can be marked for deletion, but isn't actually deleted until the object
    /// is saved to the database. This property indicates whether or not the
    /// current object has been marked for deletion. If it is true
    /// , the object will
    /// be deleted when it is saved to the database, otherwise it will be inserted
    /// or updated by the save operation.
    /// </remarks>
    /// <returns>A value indicating if this object is marked for deletion.</returns>
    bool IsDeleted { get; }
    /// <summary>
    /// Returns true if this is a new object, 
    /// false if it is a pre-existing object.
    /// </summary>
    /// <remarks>
    /// An object is considered to be new if its primary identifying (key) value 
    /// doesn't correspond to data in the database. In other words, 
    /// if the data values in this particular
    /// object have not yet been saved to the database the object is considered to
    /// be new. Likewise, if the object's data has been deleted from the database
    /// then the object is considered to be new.
    /// </remarks>
    /// <returns>A value indicating if this object is new.</returns>
    bool IsNew { get; }
    /// <summary>
    /// Returns true if this object is both dirty and valid.
    /// </summary>
    /// <remarks>
    /// An object is considered dirty (changed) if 
    /// <see cref="P:Csla.BusinessBase.IsDirty" /> returns true. It is
    /// considered valid if IsValid
    /// returns true. The IsSavable property is
    /// a combination of these two properties. 
    /// </remarks>
    /// <returns>A value indicating if this object is both dirty and valid.</returns>
    bool IsSavable { get; }
    /// <summary>
    /// Returns true if this is a child object, 
    /// false if it is a root object.
    /// </summary>
    /// <remarks>
    /// An object should be marked as a child object unless it is the
    /// top most object (root object) in a business object graph.
    /// </remarks>
    /// <returns>A value indicating if this a child object.</returns>
    bool IsChild { get; }
  }
}