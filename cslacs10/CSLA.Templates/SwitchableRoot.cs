using System;

namespace CSLA.Templates
{
  [Serializable()]
  public class SwitchableRoot : BusinessBase
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

    public bool Equals(SwitchableRoot obj)
    {
      // Implement comparison between two of our type of object
    }

    public new static bool Equals(object objA, object objB) 
    {
      if(objA is SwitchableRoot && objB is SwitchableRoot) 
        return ((SwitchableRoot)objA).Equals((SwitchableRoot)objB);
      else 
        return false;
    }
  
    public override bool Equals(object obj)
    {
      if(obj is SwitchableRoot)
        return this.Equals((SwitchableRoot)obj);
      else 
        return false;
    }
  

    public override int GetHashCode()
    {
      // Return a hash value for our object
    }

    #endregion

    #region Static Methods

    public static SwitchableRoot NewSwitchableRoot() 
    {
      return (SwitchableRoot)DataPortal.Create(new Criteria(false));
    }

    internal static SwitchableRoot NewSwitchableChild() 
    {
      return (SwitchableRoot)DataPortal.Create(new Criteria(true));
    }

    public static SwitchableRoot GetSwitchableRoot() 
    {
      return (SwitchableRoot)DataPortal.Fetch(new Criteria(false));
    }

    internal static SwitchableRoot GetSwitchableChild() 
    {
      return (SwitchableRoot)DataPortal.Fetch(new Criteria(true));
    }

    public static void DeleteSwitchableRoot()
    {
      DataPortal.Delete(new Criteria(false));
    }

    #endregion

    #region Constructors

    private SwitchableRoot()
    {
      // Prevent direct creation
    }

    #endregion

    #region Criteria

    [Serializable()]
      private class Criteria
    {
      public bool IsChild;

      public Criteria(bool IsChild)
      {
        this.IsChild = IsChild;
      }
    }

    #endregion

    #region Data Access

    protected override void DataPortal_Create(object criteria)
    {
      Criteria crit = (Criteria)criteria;

      if(crit.IsChild)
        MarkAsChild();

      // Load default values from database
    }

    protected override void DataPortal_Fetch(object criteria)
    {
      Criteria crit = (Criteria)criteria;

      // Load object data from database
      MarkAsChild();
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

  }
}
