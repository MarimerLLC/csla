//-----------------------------------------------------------------------
// <copyright file="GrandChild.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>Business object type for use in tests</summary>
//-----------------------------------------------------------------------

using System.Data;
using System.ComponentModel.DataAnnotations;

namespace Csla.Test.Server.Interceptors
{
  [Serializable()]
  public class GrandChild : BusinessBase<GrandChild>
  {
    public static readonly PropertyInfo<string> DataProperty = RegisterProperty<string>(c => c.Data);
    [Required]
    public string Data
    {
      get { return GetProperty(DataProperty); }
      set { SetProperty(DataProperty, value); }
    }

    internal static GrandChild NewGrandChild(string data)
    {
      GrandChild obj = new GrandChild();
      obj.Data = data;
      return obj;
    }

    internal static GrandChild GetGrandChild(IDataReader dr)
    {
      GrandChild obj = new GrandChild();
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