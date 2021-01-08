//-----------------------------------------------------------------------
// <copyright file="GenRootBase.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Test.Basic
{
    [Serializable()]
    public abstract class GenRootBase : BusinessBase<GenRoot>
    {
        private string _data = "";

        protected override object GetIdValue()
        {
            return _data;
        }

        public string Data
        {
            get { return _data; }
            set {
                if (_data != value) 
                {
                    _data = value;
                    MarkDirty();
                }
            }
        }

        [Serializable()]
        private class Criteria : CriteriaBase<Criteria>
        {
            public string _data;

            public Criteria() 
            {
                _data = "<new>";
            }

            public Criteria(string data)
            {
                this._data = data;
            }
        }

        public static GenRoot NewRoot()
        {
            return (GenRoot)(Csla.DataPortal.Create<GenRoot>(new Criteria()));
        }

        public static GenRoot GetRoot(string data)
        {
          return (GenRoot)(Csla.DataPortal.Fetch<GenRoot>(new Criteria(data)));
        }

        public static void DeleteRoot(string data)
        {
          Csla.DataPortal.Delete<GenRoot>(new Criteria(data));
        }

        [Create]
        private void DataPortal_Create(object criteria)
        {
            Criteria crit = (Criteria)(criteria);
            _data = crit._data;
#pragma warning disable CS0618 // Type or member is obsolete
            Csla.ApplicationContext.GlobalContext.Add("GenRoot", "Created");
#pragma warning restore CS0618 // Type or member is obsolete
    }

    [Fetch]
        protected void DataPortal_Fetch(object criteria)
        {
            Criteria crit = (Criteria)(criteria);
            _data = crit._data;
            MarkOld();
#pragma warning disable CS0618 // Type or member is obsolete
            Csla.ApplicationContext.GlobalContext.Add("GenRoot", "Fetched");
#pragma warning restore CS0618 // Type or member is obsolete
    }

    [Update]
        protected override void DataPortal_Update()
        {
            if (IsDeleted)
            {
        //we would delete here
#pragma warning disable CS0618 // Type or member is obsolete
              Csla.ApplicationContext.GlobalContext.Add("GenRoot", "Deleted");
#pragma warning restore CS0618 // Type or member is obsolete
              MarkNew();
            }
            else
            {
                if (IsNew)
                {
                  //we would insert here
#pragma warning disable CS0618 // Type or member is obsolete
                  Csla.ApplicationContext.GlobalContext.Add("GenRoot", "Inserted");
#pragma warning restore CS0618 // Type or member is obsolete
                }
                else 
                {
                  //we would update here
#pragma warning disable CS0618 // Type or member is obsolete
                  Csla.ApplicationContext.GlobalContext.Add("GenRoot", "Updated");
#pragma warning restore CS0618 // Type or member is obsolete
                }
                MarkOld();
            }
        }

        [Delete]
        protected void DataPortal_Delete(object Criteria)
        {
          //we would delete here
#pragma warning disable CS0618 // Type or member is obsolete
          Csla.ApplicationContext.GlobalContext.Add("GenRoot", "Deleted");
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
  }