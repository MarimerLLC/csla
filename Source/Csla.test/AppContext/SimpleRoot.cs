//-----------------------------------------------------------------------
// <copyright file="SimpleRoot.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>The unique ID of this object</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Test.AppContext
{
    [Serializable()]
    class SimpleRoot : BusinessBase<SimpleRoot>
    {
        private string _Data = string.Empty;
        /// <summary>
        /// The unique ID of this object
        /// </summary>
        /// <returns></returns>
        protected override object GetIdValue()
        {
            return this._Data;
        }
        /// <summary>
        /// The data value for this object
        /// </summary>
        public string Data
        {
            get { return this._Data; }
            set
            {
                if (!this._Data.Equals(value))
                {
                    this._Data = value;
                    this.MarkDirty();
                }
            }
        }
        /// <summary>
        /// Criteria for DataPortal overrides
        /// </summary>
        [Serializable()]
        internal class Criteria
        {
            public const string DefaultData = "<new>";

            private string _Data = string.Empty;
            public string Data
            {
                get { return this._Data; }
                set { this._Data = value; }
            }
            public Criteria()
            {
                this._Data = Criteria.DefaultData;
            }
            public Criteria(string Data)
            {
                this._Data = Data;
            }
        }

        /// <summary>
        /// Handles new DataPortal Create calls
        /// </summary>
        /// <param name="criteria"></param>
        private void DataPortal_Create(object criteria)
        {
            Criteria crit = criteria as Criteria;
            this._Data = crit.Data;

            TestResults.Add("Root", "Created");
        }
        /// <summary>
        /// Handles DataPortal fetch calls
        /// </summary>
        /// <param name="criteria"></param>
        protected void DataPortal_Fetch(object criteria)
        {
            Criteria crit = criteria as Criteria;
            this._Data = crit.Data;

            this.MarkOld();
            TestResults.Add("Root", "Fetched");
        }
        /// <summary>
        /// 
        /// </summary>
        [Update]
		protected void DataPortal_Update()
        {
            if (this.IsDeleted)
            {
                TestResults.Add("Root", "Deleted");
                this.MarkNew();
            }
            else
            {
                if (this.IsNew)
                {
                  TestResults.Add("Root", "Inserted");
                }
                else TestResults.Add("Root", "Updated");

        this.MarkOld();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="criteria"></param>
        [Delete]
		protected void DataPortal_Delete(object criteria)
        {
            TestResults.Add("Root", "Deleted");
        }
    }
}