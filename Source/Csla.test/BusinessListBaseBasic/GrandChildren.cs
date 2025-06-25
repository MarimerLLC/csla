//-----------------------------------------------------------------------
// <copyright file="GrandChildren.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using System.Data;

namespace Csla.Test.BusinessListBaseBasic
{
  [Serializable]
  public class GrandChildren : BusinessListBase<GrandChildren, GrandChild>
  {
    public void Add(string data)
    {
      Add(GrandChild.NewGrandChild(data));
    }

    internal static GrandChildren NewGrandChildren()
    {
      return new GrandChildren();
    }

    internal static GrandChildren GetGrandChildren(IDataReader dr)
    {
      return null;
    }

    internal void Update(IDbTransaction tr)
    {
      foreach (GrandChild child in this)
      {
        child.Update(tr);
      }
    }

    public GrandChildren()
    {
      MarkAsChild();
    }
  }
}