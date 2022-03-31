//-----------------------------------------------------------------------
// <copyright file="ChildEntityList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Csla.Test.DataBinding
{
    [Serializable()]
    public class ChildEntityList : BusinessBindingListBase<ChildEntityList, ChildEntity>
    {
        public ChildEntityList()
        {
        }

        #region "Criteria"

        [Serializable()]
        private class Criteria
        {
            //no criteria for this list
        }

        #endregion

        [Fetch]
        private void DataPortal_Fetch(object criteria, IChildDataPortal<ChildEntity> childDataPortal)
        {
            for (int i = 0; i < 10; i++)
                Add(childDataPortal.CreateChild(i, "first" + i, "last" + i));
        }
    }
}