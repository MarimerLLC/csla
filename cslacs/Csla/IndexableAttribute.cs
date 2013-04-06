using System;

namespace Csla
{
  /// <summary>
  /// Marks a property to indicate that, when contained in a CSLA collection 
  /// class, that an index should be built for the property that will
  /// be used in LINQ queries
  /// </summary>
  /// <remarks>
  /// Marking a variable with this attribute will cause any CSLA collection
  /// class to create an internal index on this value.  Use this carefully, as
  /// the more items are indexed, the slower add operations will be
  /// </remarks>
  [AttributeUsage(AttributeTargets.Property)]
  public sealed class IndexableAttribute : Attribute
  {
    private IndexModeEnum _indexMode = IndexModeEnum.IndexModeOnDemand;
    /// <summary>
    /// Allows user to determine how indexing will occur
    /// </summary>
    public IndexModeEnum IndexMode 
    { 
      get 
      { 
        return _indexMode; 
      } 
      set 
      { 
        _indexMode = value; 
      } 
    }
    /// <summary>
    /// Sets the property as indexable on demand
    /// </summary>
    public IndexableAttribute() { }
    /// <summary>
    /// Set the indexable property, along with it's index mode
    /// </summary>
    public IndexableAttribute(IndexModeEnum indexMode) { _indexMode = indexMode; }

  }
}
