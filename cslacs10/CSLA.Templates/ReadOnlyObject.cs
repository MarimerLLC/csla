using System;

namespace CSLA.Templates
{
  [Serializable()]
  public class ReadOnlyObject : ReadOnlyBase
  {
    // Declare variables here to contain object state

    // Declare variables for any child collections here

  #region Business Properties and Methods

    // Implement read-only properties and methods here so the UI or
    // other client code can interact with the object

    //public string Data
    //{
    //  get
    //  {
    //  }
    //}

  #endregion

  #region System.Object Overrides

    public override string ToString() 
    {
      // Return text describing our object
    }

    public bool Equals(ReadOnlyObject obj)
    {
      // Implement comparison between two of our type of object
    }

    public override int GetHashCode() 
    {
      // Return a hash value for our object
    }

  #endregion

  #region Static Methods

    public static ReadOnlyObject GetReadOnlyObject() 
    {
      return (ReadOnlyObject)DataPortal.Fetch(new Criteria());
    }

  #endregion

  #region Constructors

    private ReadOnlyObject()
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

    protected override void DataPortal_Fetch(object criteria)
    {
      Criteria crit = (Criteria)criteria;
      // Load object data from database
    }

  #endregion

  }
}
