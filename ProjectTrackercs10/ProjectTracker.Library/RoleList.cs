using System;
using CSLA;

namespace ProjectTracker.Library
{
  [Serializable()]
	public class RoleList : NameValueList
	{

    #region Shared Methods

    public static RoleList GetList()
    {
      return (RoleList)DataPortal.Fetch(new Criteria());
    }

    #endregion

    #region Constructors

		private RoleList()
		{
			// prevent direct creation
		}

    // this constructor overload is required because
    // the base class (NameObjectCollectionBase) implements
    // ISerializable
    private RoleList(System.Runtime.Serialization.SerializationInfo info, 
      System.Runtime.Serialization.StreamingContext context) : base(info, context)
    {
    }

    #endregion

    #region Criteria

    [Serializable()]
    private class Criteria
    {
      // add criteria here
    }

    #endregion


    #region Data Access

    // called by DataPortal to load data from the database
    protected override void DataPortal_Fetch(object Criteria)
    {
      SimpleFetch("PTracker", "Roles", "id", "name");
    }

    #endregion
	}
}
