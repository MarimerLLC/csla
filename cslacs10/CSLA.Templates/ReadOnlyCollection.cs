using System;
using System.Data;

namespace CSLA.Templates
{
  [Serializable()]
  public class ReadOnlyCollection : CSLA.ReadOnlyCollectionBase
  {
    #region Business Properties and Methods

    public ReadOnlyObject this [int index]
    {
      get
      {
        return (ReadOnlyObject)List[index];
      }
    }

    #endregion

    #region Contains

    public bool Contains(ReadOnlyObject item) 
    {
      foreach(ReadOnlyObject child in List)
        if(child.Equals(item))
          return true;

      return false;

    }

    #endregion

    #region Static Methods

    public static ReadOnlyCollection GetCollection() 
    {
      return (ReadOnlyCollection)DataPortal.Fetch(new Criteria());
    }

    #endregion

    #region Constructors

    private ReadOnlyCollection()
    {
      // Prevent direct creation
    }

    #endregion

    #region Criteria

    [Serializable()]
      public class Criteria
    {}

    #endregion

    #region Data Access

    protected override void DataPortal_Fetch(object criteria)
    {
      locked = false;

      Criteria crit = (Criteria)criteria;

      // Retrieve all child data into a data reader
      // then loop through and create each child object
      IDataReader dr;
      while(dr.Read())
        List.Add(ReadOnlyObject.GetReadOnlyObject(dr));

      locked = true;
    }

    #endregion

  }
}