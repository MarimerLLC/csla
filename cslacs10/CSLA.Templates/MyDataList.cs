using System;

namespace CSLA.Templates
{
  [Serializable()]
  public class MyDataList : NameValueList
  {
  #region static Methods

    public static MyDataList GetMyDataList() 
    {
      return (MyDataList)DataPortal.Fetch(new Criteria());
    }

  #endregion

  #region Constructors

    private MyDataList()
    {
      // Prevent direct creation
    }

    // This constructor overload is required because the base class
    //  (NameObjectCollectionBase) implements ISerializable
    private MyDataList(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) :
      base(info, context)
    {}

  #endregion

  #region Criteria

    [Serializable()]
      private class Criteria
    {
      // Add criteria here
    }

  #endregion

  #region Data Access

    // Called by DataPortal to load data from the database
    protected override void DataPortal_Fetch(object criteria)
    {
      SimpleFetch("myDatabase", "myTable", "myNameColumn", " myValueColumn");
    }

  #endregion

  }
}
