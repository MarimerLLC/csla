//-----------------------------------------------------------------------
// <copyright file="InsertObjectArgs.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Argument object used in the InsertObject event.</summary>
//-----------------------------------------------------------------------
using System;

namespace Csla.Web
{
  /// <summary>
  /// Argument object used in the InsertObject event.
  /// </summary>
  public class InsertObjectArgs : EventArgs
  {

    private System.Collections.IDictionary _values;
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
    /// The list of data values entered by the user.
    /// </summary>
    /// <remarks>It is up to the event handler in the
    /// web page to take the list of values, put them
    /// into a business object and to save that object
    /// into the database.</remarks>
    public System.Collections.IDictionary Values
    {
      get { return _values; }
    }

    /// <summary>
    /// Create an instance of the object.
    /// </summary>
    public InsertObjectArgs(System.Collections.IDictionary values)
    {
      _values = values;
    }

  }
}