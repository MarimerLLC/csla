//-----------------------------------------------------------------------
// <copyright file="NullableObject.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

namespace Csla.Test.Nullable
{
  [Serializable]
  public class NullableObject : BusinessBase<NullableObject>
  {
    private string _name = string.Empty;
    private Nullable<int> _nullableInteger;
    public Nullable<int> _nullableIntMember;

    protected override object GetIdValue()
    {
      return _name;
    }

    public string Name
    {
      get { return _name; }
      set
      {
        _name = value;
      }
    }

    public Nullable<int> NullableInteger
    {
      get { return _nullableInteger; }
      set
      {
        if (_nullableInteger != value)
        {
          _nullableInteger = value;
          MarkDirty();
        }
      }
    }

    [Serializable]
    private class Criteria
    {
      public string _name;

      public Criteria()
      {
        _name = "<new>";
      }

      public Criteria(string name)
      {
        _name = name;
      }
    }

    public static NullableObject NewNullableObject(IDataPortal<NullableObject> dataPortal)
    {
      return dataPortal.Create(new Criteria());
    }

    public static NullableObject GetNullableObject(string name, IDataPortal<NullableObject> dataPortal)
    {
      return dataPortal.Fetch(new Criteria(name));
    }

    public static void DeleteNullableObject(string name, IDataPortal<NullableObject> dataPortal)
    {
      dataPortal.Delete(new Criteria(name));
    }

    public NullableObject()
    {
      AddBusinessRules();
    }

    private void DataPortal_Create(object criteria)
    {
      Criteria crit = (Criteria)(criteria);
      _name = crit._name;
      //Name = crit._name;
      TestResults.Add("NullableObject", "Created");
    }

    protected void DataPortal_Fetch(object criteria)
    {
      Criteria crit = (Criteria)(criteria);
      _name = crit._name;
      MarkOld();
      TestResults.Add("NullableObject", "Fetched");
    }

    [Update]
    protected void DataPortal_Update()
    {
      if (IsDeleted)
      {
        //we would delete here
        TestResults.Add("NullableObject", "Deleted");
        MarkNew();
      }
      else
      {
        if (IsNew)
        {
          //we would insert here
          TestResults.Add("NullableObject", "Inserted");
        }
        else
        {
          //we would update here
          TestResults.Add("NullableObject", "Updated");
        }

        MarkOld();
      }
    }

    [Delete]
    protected void DataPortal_Delete(object criteria)
    {
      //we would delete here
      TestResults.Add("NullableObject", "Deleted");
    }
  }
}