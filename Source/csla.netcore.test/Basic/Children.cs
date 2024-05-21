//-----------------------------------------------------------------------
// <copyright file="Children.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using System.Data;

namespace Csla.Test.Basic
{
  [Serializable]
  public class Children : BusinessListBase<Children, Child>
  {
    public void Add(string data)
    {
      var child = AddNew();
      child.Data = data;
    }

    internal void Update(IDbTransaction tr)
    {
      foreach (Child child in this)
      {
        child.Update(tr);
      }
    }

    public int DeletedCount
    {
      get { return DeletedList.Count; }
    }

    public List<Child> GetDeletedList()
    {
      return DeletedList;
    }
  }
}