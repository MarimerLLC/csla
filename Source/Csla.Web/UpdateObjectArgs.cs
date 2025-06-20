//-----------------------------------------------------------------------
// <copyright file="UpdateObjectArgs.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Argument object used in the UpdateObject event.</summary>
//-----------------------------------------------------------------------

namespace Csla.Web
{
  /// <summary>
  /// Argument object used in the UpdateObject event.
  /// </summary>
  public class UpdateObjectArgs : EventArgs
  {
    /// <summary>
    /// Gets or sets the number of rows affected
    /// while handling this event.
    /// </summary>
    /// <value></value>
    /// <remarks>
    /// The code handling the event should set this
    /// value to indicate the number of rows affected
    /// by the operation.
    /// </remarks>
    public int RowsAffected { get; set; }

    /// <summary>
    /// The list of key values entered by the user.
    /// </summary>
    /// <remarks>It is up to the event handler in the
    /// web page to take the list of values, put them
    /// into a business object and to save that object
    /// into the database.</remarks>
    public System.Collections.IDictionary Keys { get; }

    /// <summary>
    /// The list of data values entered by the user.
    /// </summary>
    /// <remarks>It is up to the event handler in the
    /// web page to take the list of values, put them
    /// into a business object and to save that object
    /// into the database.</remarks>
    public System.Collections.IDictionary Values { get; }

    /// <summary>
    /// The list of old data values maintained by
    /// data binding.
    /// </summary>
    /// <remarks>It is up to the event handler in the
    /// web page to take the list of values, put them
    /// into a business object and to save that object
    /// into the database.</remarks>
    public System.Collections.IDictionary OldValues { get; }

    /// <summary>
    /// Creates an instance of the object.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="keys"/>, <paramref name="values"/> or <paramref name="oldValues"/> is <see langword="null"/>.</exception>
    public UpdateObjectArgs(System.Collections.IDictionary keys, System.Collections.IDictionary values, System.Collections.IDictionary oldValues)
    {
      Keys = keys ?? throw new ArgumentNullException(nameof(keys));
      Values = values ?? throw new ArgumentNullException(nameof(values));
      OldValues = oldValues ?? throw new ArgumentNullException(nameof(oldValues));
    }

  }
}