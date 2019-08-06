//-----------------------------------------------------------------------
// <copyright file="DeleteObjectArgs.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Argument object used in the DeleteObject event.</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Web
{
  /// <summary>
  /// Argument object used in the DeleteObject event.
  /// </summary>
  public class DeleteObjectArgs : EventArgs
  {
    private System.Collections.IDictionary _keys;
    private System.Collections.IDictionary _oldValues;
    private int _rowsAffected;

    /// <summary>
    /// Gets or sets the number of rows affected
    /// while handling this event.
    /// </summary>
    /// <value></value>
    /// <returns></returns>
    /// <remarks>
    /// The code handling the event should set this
    /// value to indicate the number of rows affected
    /// by the operation.
    /// </remarks>
    public int RowsAffected
    {
      get { return _rowsAffected; }
      set { _rowsAffected = value; }
    }

    /// <summary>
    /// The list of key values entered by the user.
    /// </summary>
    /// <remarks>It is up to the event handler in the
    /// web page to use the values to identify the 
    /// object to be deleted.</remarks>
    public System.Collections.IDictionary Keys
    {
      get { return _keys; }
    }

    /// <summary>
    /// The list of old data values maintained by
    /// data binding.
    /// </summary>
    /// <remarks>It is up to the event handler in the
    /// web page to use the values to identify the 
    /// object to be deleted.</remarks>
    public System.Collections.IDictionary OldValues
    {
      get { return _oldValues; }
    }

    /// <summary>
    /// Create an instance of the object.
    /// </summary>
    public DeleteObjectArgs(System.Collections.IDictionary keys, System.Collections.IDictionary oldValues)
    {
      _keys = keys;
      _oldValues = oldValues;
    }

  }
}