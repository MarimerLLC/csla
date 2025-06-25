//-----------------------------------------------------------------------
// <copyright file="ReadOnlyList.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------

namespace Csla.Test.BusinessListBaseBasic
{
#if csla20vb
    [Serializable()]
    public class ReadOnlyList : ReadOnlyListBase<ReadOnlyList, object>
    {
    #region "Criteria"

        private class Criteria
        {
        }

        #endregion

        private ReadOnlyList()
        {
            //require use of factory method
        }

    #region "Factory methods"

        public static ReadOnlyList GetReadOnlyList()
        {
            return Csla.DataPortal.Fetch<ReadOnlyList>(new Criteria());
        }

        #endregion

    #region "Data access"

        protected override void DataPortal_Fetch(object criteria)
        {
            TestResults.Reinitialise();
            TestResults.Add("ReadOnlyList", "Fetched");
        }

        #endregion

    }
#elif csla20cs
    public class ReadOnlyList
    {
    }
#endif
}