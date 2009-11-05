using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Csla.Test.DataBinding
{
    [Serializable()]
    public class ParentEntity : BusinessBase<ParentEntity>
    {
        #region "Business methods"

        private int _ID;
        private string _data;
        private ChildEntityList _children = ChildEntityList.NewChildEntityList();
        [NotUndoable()]
        private string _notUndoable;

        public string NotUndoable
        {
            get { return _notUndoable; }
            set { _notUndoable = value; }
        }

        public int ID
        {
            get { return _ID; }
        }

        public string Data
        {
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
            get
            {
                if (CanReadProperty())
                {
                    return _data;
                }
                else
                {
                    throw new System.Security.SecurityException("Property get not allowed");
                }
            }
            [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
            set
            {
                if (CanWriteProperty())
                {
                    if (_data != value)
                    {
                        _data = value;
                        PropertyHasChanged("Data"); //I prefer not to check the stacktrace
                    }
                }
                else
                {
                    throw new System.Security.SecurityException("Property set not allowed");
                }
            }
        }

        public ChildEntityList Children
        {
            get { return _children; }
        }

        public override bool IsDirty
        {
            get
            {
                return base.IsDirty || _children.IsDirty;
            }
        }

        #endregion

        #region "Object ID value"

        protected override object GetIdValue()
        {
            return _ID;
        }

        #endregion

        protected override void AddBusinessRules()
        {
            //don't need rules for databinding tests
            //ValidationRules.AddRule(Validation.CommonRules.StringRequired, "Data");
        }

        #region "constructors"

        /// <summary>
        /// DO NOT USE in UI - use the factory method instead
        /// </summary>
        /// <remaks>
        ///this constructor is public only to support web forms databinding 
        ///</remaks>
        public ParentEntity()
        {
            //if we need authorization rules:
            //this.AuthorizationRules.AllowWrite("Data", "Admin");
            //this.AuthorizationRules.AllowRead("Data", "Admin");
        }

        #endregion

        #region "Factory Methods"

        public static ParentEntity NewParentEntity()
        {
            return Csla.DataPortal.Create<ParentEntity>();
        }

        public static ParentEntity GetParentEntity(int ID)
        {
            return Csla.DataPortal.Fetch<ParentEntity>(new Criteria(ID));
        }

        public static void DeleteParentEntity(int ID)
        {
            Csla.DataPortal.Delete(new Criteria(ID));
        }

        #endregion

        public override ParentEntity Save()
        {
            return base.Save();
        }

        #region "Criteria"

        [Serializable()]
        private class Criteria
        {
            public int _id;

            public Criteria(int id)
            {
                this._id = id;
            }
        }

        #endregion

        #region "Data Access"

        [RunLocal()]
        protected override void DataPortal_Create()
        {
            Csla.ApplicationContext.GlobalContext.Clear();
            Csla.ApplicationContext.GlobalContext.Add("ParentEntity", "Created");
            ValidationRules.CheckRules();
            Console.WriteLine("DataPortal_Create");
        }

        protected override void DataPortal_Fetch(object criteria)
        {
            Console.WriteLine("DataPortal_Fetch");
            Csla.ApplicationContext.GlobalContext.Clear();
            Csla.ApplicationContext.GlobalContext.Add("ParentEntity", "Fetched");
            ValidationRules.CheckRules();
        }

        protected override void DataPortal_Insert()
        { 
            Csla.ApplicationContext.GlobalContext.Clear();
            Csla.ApplicationContext.GlobalContext.Add("ParentEntity", "Inserted");
            Console.WriteLine("DataPortal_Insert");
        }

        protected override void  DataPortal_Update()
        {
            Console.WriteLine("DataPortal_Update");
            Csla.ApplicationContext.GlobalContext.Clear();
            Csla.ApplicationContext.GlobalContext.Add("ParentEntity", "Updated");
        }

        protected override void DataPortal_DeleteSelf()
        {
            Console.WriteLine("DataPortal_DeleteSelf");
            Csla.ApplicationContext.GlobalContext.Clear();
            Csla.ApplicationContext.GlobalContext.Add("ParentEntity", "Deleted Self");
        }

        protected override void DataPortal_Delete(object criteria)
        {
            Console.WriteLine("DataPortal_Delete");
            Csla.ApplicationContext.GlobalContext.Clear();
            Csla.ApplicationContext.GlobalContext.Add("ParentEntity", "Deleted");
        }

        #endregion
    }
}