using System;
using System.Data;

namespace CSLA.Templates
{
  [Serializable()]
  public class EditableRootCollection : BusinessCollectionBase
  {
  #region Business Properties and Methods

    public EditableChild this [int index]
    {
      get
      {
        return (EditableChild)List[index];
      }
    }

    public void Add(string data)
    {
      List.Add(EditableChild.NewEditableChild());
    }

    public void Remove(EditableChild child)
    {
      List.Remove(child);
    }

    #endregion

    #region Contains

    public bool Contains(EditableChild item) 
    {
      foreach(EditableChild child in List)
        if(child.Equals(item))
          return true;

      return false;
    }

    public bool ContainsDeleted(EditableChild item) 
    {
      foreach(EditableChild child in deletedList)
        if(child.Equals(item))
          return true;

      return false;
    }

    #endregion

    #region Static Methods

    public static EditableRootCollection NewCollection() 
    {
      return new EditableRootCollection();
    }

    public static EditableRootCollection GetCollection() 
    {
      return (EditableRootCollection)DataPortal.Fetch(new Criteria());
    }

    public static void DeleteCollection()
    {
      DataPortal.Delete(new Criteria());
    }

    #endregion

    #region Constructors

    private EditableRootCollection()
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

      // Retrieve all child data into a data reader
      // then loop through and create each child object
      IDataReader dr;
      while(dr.Read())
        List.Add(EditableChild.GetEditableChild(dr));
    }

    protected override void DataPortal_Update()
    {
      // Loop through each deleted child object and call its Update() method
      foreach(EditableChild child in deletedList)
        child.Update(this);

      // Then clear the list of deleted objects because they are truly gone now
      deletedList.Clear();

      // Loop through each non-deleted child object and call its Update() method
      foreach(EditableChild child in List)
        child.Update(this);
    }

    protected override void DataPortal_Delete(object criteria)
    {
      Criteria crit = (Criteria)criteria;

      // Delete all child object data that matches the criteria
    }

    #endregion

  }
}