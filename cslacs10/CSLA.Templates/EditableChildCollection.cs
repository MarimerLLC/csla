using System;
using System.Data;

namespace CSLA.Templates
{
  [Serializable()]
  public class EditableChildCollection : BusinessCollectionBase
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
      List.Add(EditableChild.NewEditableChild(data));
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

    #region static Methods

    internal static EditableChildCollection NewEditableChildCollection() 
    {
      return new EditableChildCollection();
    }

    internal static EditableChildCollection GetEditableChildCollection(     
      IDataReader dr) 
    {
      EditableChildCollection col = new EditableChildCollection();
      col.Fetch(dr);
      return col;
    }

    #endregion

    #region Constructors

    private EditableChildCollection()
    {
      // Prevent direct creation
      MarkAsChild();
    }

    #endregion

    #region Data Access

    private void Fetch(IDataReader dr)
    {
      // Create a child object for each row in the data source
      while(dr.Read())
        List.Add(EditableChild.GetEditableChild(dr));
    }

    internal void Update(EditableRoot parent)
    {
      // Loop through each deleted child object and call its Update() method
      foreach(EditableChild child in deletedList)
        child.Update(parent);

      // Then clear the list of deleted objects because they are truly gone now
      deletedList.Clear();

      // Loop through each non-deleted child object and call its Update() method
      foreach(EditableChild child in List)
        child.Update(parent);
    }

    #endregion

  }
}
