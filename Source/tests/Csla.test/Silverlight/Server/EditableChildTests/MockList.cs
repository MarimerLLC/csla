//-----------------------------------------------------------------------
// <copyright file="MockList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Linq;

namespace Csla.Testing.Business.EditableChildTests
{
  [Serializable]
  public partial class MockList : BusinessListBase<MockList, MockEditableChild>
  {
    public static Guid MockEditableChildId1 = new Guid("{4AF07634-1D10-4d09-BBCB-42396324C103}");
    public static Guid MockEditableChildId2 = new Guid("{402DB94B-0D93-4be2-AFD0-7D1587CDDBC2}");
    public static Guid MockEditableChildId3 = new Guid("{B2ADE99D-7AB4-4e70-AEA1-D70443BCC3B0}");

    private void Fetch(string nameFilter)
    {
      RaiseListChangedEvents = false;
      var children = 
        new[] 
          { 
            MockEditableChild.Load(MockEditableChildId1, "c1"), 
            MockEditableChild.Load(MockEditableChildId2, "c2"), 
            MockEditableChild.Load(MockEditableChildId3, "c3") 
          };

      var found = from c in children
                  where string.IsNullOrEmpty(nameFilter) || c.Name == nameFilter
                  select c;

      AddRange(found.ToArray());

      RaiseListChangedEvents = true;
    }
  }
}