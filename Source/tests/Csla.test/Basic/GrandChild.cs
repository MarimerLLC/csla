//-----------------------------------------------------------------------
// <copyright file="GrandChild.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using System.Data;

namespace Csla.Test.Basic
{
  [Serializable]
  public class GrandChild : BusinessBase<GrandChild>
  {
    private string _data = "";

    protected override object GetIdValue()
    {
      return _data;
    }

    public string Data
    {
      get { return _data; }
      set
      {
        if (_data != value)
        {
          _data = value;
          MarkDirty();
        }
      }
    }

    internal static GrandChild NewGrandChild(string data)
    {
      return new GrandChild
      {
        _data = data
      };
    }

    internal static GrandChild GetGrandChild(IDataReader dr)
    {
      var obj = new GrandChild();
      obj.Fetch(dr);
      return obj;
    }

    public GrandChild()
    {
      MarkAsChild();
    }

    private void Fetch(IDataReader dr)
    {
      MarkOld();
    }

    internal void Update(IDbTransaction tr)
    {
      if (IsDeleted)
      {
        //we would delete here
        MarkNew();
      }
      else
      {
        if (IsNew)
        {
          //we would insert here
        }
        else
        {
          //we would update here
        }

        MarkOld();
      }
    }
  }
}