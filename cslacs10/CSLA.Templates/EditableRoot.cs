using System;

namespace CSLA.Templates
{
  [Serializable()]
  public class EditableRoot : BusinessBase, IDisposable
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

    public new static bool Equals(object objA, object objB) 
    {
      if(objA is EditableRoot && objB is EditableRoot) 
        return ((EditableRoot)objA).Equals((EditableRoot)objB);
      else 
        return false;
    }
  
    public override bool Equals(object obj)
    {
      if(obj is EditableRoot)
        return this.Equals((EditableRoot)obj);
      else 
        return false;
    }
  

    public override int GetHashCode()
    {
      // Return a hash value for our object
    }

    #endregion

    #region Static Methods

    public static EditableRoot NewEditableRoot() 
    {
      return (EditableRoot)DataPortal.Create(new Criteria());
    }

    public static EditableRoot GetEditableRoot() 
    {
      return (EditableRoot)DataPortal.Fetch(new Criteria());
    }

    public static void DeleteEditableRoot()
    {
      DataPortal.Delete(new Criteria());
    }

    #endregion

    #region Constructors

    private EditableRoot()
    {
      // Prevent direct creation
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

    protected override void DataPortal_Fetch(object criteria)
    {
      Criteria crit = (Criteria)criteria;

      // Load object data from database
    }

    protected override void DataPortal_Update()
    {
      // Insert or update object data into database
    }

    protected override void DataPortal_Delete(object criteria)
    {
      Criteria crit = (Criteria)criteria;

      // Delete object data from database
    }

    #endregion


    public void Dispose()
    {
    }
  }
}