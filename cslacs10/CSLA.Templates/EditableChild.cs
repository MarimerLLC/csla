using System;
using System.Data;

namespace CSLA.Templates
{
  [Serializable()]
  public class EditableChild : BusinessBase
  {
    // Declare variables here to contain object state

    // Declare variables for any child collections here
 
    #region Business Properties and Methods

    // Implement properties and methods here so the UI or
    // other client code can interact with the object

    #endregion

    #region System.Object Overrides

    public override string ToString()
    {
      // Return text describing our object
    }

    public bool Equals(EditableRoot obj)
    {
      // Implement comparison between two of our type of object
    }

    public override int GetHashCode()
    {
      // Return a hash value for our object
    }

    #endregion

    #region Static Methods

    internal static EditableChild NewEditableChild() 
    {
      return (EditableChild)DataPortal.Create(new Criteria());
    }

    internal static EditableChild GetEditableChild(IDataReader dr) 
    {
      EditableChild obj = new EditableChild();
      obj.Fetch(dr);
      return obj;
    }

    #endregion

    #region Constructors

    private EditableChild()
    {
      // Prevent direct creation

      // Tell CSLA .NET that we are a child object
      MarkAsChild();
    }

    #endregion

    #region Criteria

    [Serializable()]
      private class Criteria
    {
      // Add criteria here
    }

    #endregion

    #region Data Access

    protected override void DataPortal_Create(object criteria)
    {
      Criteria crit = (Criteria)criteria;

      // Load default values from database
    }

    private void Fetch(IDataReader dr)
    {
      // Load object data from database
    }

    internal void Update(EditableRoot Parent)
    {
      // Insert or update object data into database
    }

    #endregion

  }
}