//-----------------------------------------------------------------------
// <copyright file="ChildEntity.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

namespace Csla.Test.DataBinding
{
  [Serializable]
  public class ChildEntity : BusinessBase<ChildEntity>
  {
    private int _ID;
    private string _firstName = "";
    private string _lastName = "";

    public int ID => _ID;

    public string FirstName
    {
      get => _firstName;
      set
      {
        if (_firstName != value)
          _firstName = value;
      }
    }

    public string LastName
    {
      get => _lastName;
      set
      {
        if (_lastName != value)
          _lastName = value;
      }
    }

    #region "Object ID value"

    protected override object GetIdValue()
    {
      return _ID;
    }

    #endregion

    internal ChildEntity(int id, string firstName, string lastName)
    {
      _ID = id;
      _firstName = firstName;
      _lastName = lastName;
    }
  }
}