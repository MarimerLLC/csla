using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Csla.DiffGram
{
  /// <summary>
  /// Contains the state of a business
  /// object, with references to child
  /// DataItem objects.
  /// </summary>
  [Serializable]
  public class DataItem
  {
    /// <summary>
    /// Gets the arbitrary key
    /// value linking this DataItem to
    /// the associated business object.
    /// </summary>
    public int Key { get; internal set; }
    /// <summary>
    /// Gets the IsNew property
    /// value from the business object.
    /// </summary>
    public bool IsNew { get; internal set; }
    /// <summary>
    /// Gets the IsDeleted property
    /// value from the business object.
    /// </summary>
    public bool IsDeleted { get; internal set; }

    private List<DataField> _fields;
    /// <summary>
    /// Gets the list of DataField
    /// items containing the state for the
    /// associated business object.
    /// </summary>
    public List<DataField> DataFields
    {
      get
      {
        if (_fields == null)
          _fields = new List<DataField>();
        return _fields;
      }
    }

    private List<DataItem> _children;
    /// <summary>
    /// Gets the list of DataItem
    /// objects representing the state
    /// of the child objects associated
    /// with the current business object.
    /// </summary>
    public List<DataItem> Children
    {
      get
      {
        if (_children == null)
          _children = new List<DataItem>();
        return _children;
      }
    }
  }
}
