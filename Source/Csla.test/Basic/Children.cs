//-----------------------------------------------------------------------
// <copyright file="Children.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

using System.Data;
using Csla.Core;

namespace Csla.Test.Basic
{
  [Serializable]
  public class Children : BusinessBindingListBase<Children, Child>
  {
    public void Add(IDataPortal<Child> dataPortal, string data)
    {
      Add(Child.NewChild(dataPortal, data));
    }

    internal static Children NewChildren(IDataPortal<Children> dataPortal)
    {
      return dataPortal.Create();
    }

    internal static Children GetChildren(IDataReader dr)
    {
      return null;
    }

    public Children()
    {
      MarkAsChild();
    }

    public int DeletedCount
    {
      get { return DeletedList.Count; }
    }
    
    public MobileList<Child> DeletedItems => DeletedList;

    [Create]
    private void Create()
    {
    }

  }
}