//-----------------------------------------------------------------------
// <copyright file="SimpleRoot.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>The unique ID of this object</summary>
//-----------------------------------------------------------------------

namespace Csla.Test.AppContext
{
  [Serializable]
  class SimpleRoot : BusinessBase<SimpleRoot>
  {
    private string _Data = string.Empty;

    /// <summary>
    /// The unique ID of this object
    /// </summary>
    protected override object GetIdValue()
    {
      return _Data;
    }

    /// <summary>
    /// The data value for this object
    /// </summary>
    public string Data
    {
      get { return _Data; }
      set
      {
        if (!_Data.Equals(value))
        {
          _Data = value;
          MarkDirty();
        }
      }
    }

    /// <summary>
    /// Criteria for DataPortal overrides
    /// </summary>
    [Serializable]
    internal class Criteria
    {
      public const string DefaultData = "<new>";

      private string _Data = string.Empty;

      public string Data
      {
        get { return _Data; }
        set { _Data = value; }
      }

      public Criteria()
      {
        _Data = DefaultData;
      }

      public Criteria(string Data)
      {
        _Data = Data;
      }
    }

    /// <summary>
    /// Handles new DataPortal Create calls
    /// </summary>
    /// <param name="criteria"></param>
    private void DataPortal_Create(object criteria)
    {
      Criteria crit = criteria as Criteria;
      _Data = crit.Data;

      TestResults.Add("Root", "Created");
    }

    /// <summary>
    /// Handles DataPortal fetch calls
    /// </summary>
    /// <param name="criteria"></param>
    protected void DataPortal_Fetch(object criteria)
    {
      Criteria crit = criteria as Criteria;
      _Data = crit.Data;

      MarkOld();
      TestResults.Add("Root", "Fetched");
    }

    /// <summary>
    /// 
    /// </summary>
    [Update]
    protected void DataPortal_Update()
    {
      if (IsDeleted)
      {
        TestResults.Add("Root", "Deleted");
        MarkNew();
      }
      else
      {
        if (IsNew)
        {
          TestResults.Add("Root", "Inserted");
        }
        else TestResults.Add("Root", "Updated");

        MarkOld();
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="criteria"></param>
    [Delete]
    protected void DataPortal_Delete(object criteria)
    {
      TestResults.Add("Root", "Deleted");
    }
  }
}