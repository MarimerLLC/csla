using System;
using System.Collections.Generic;
using System.Text;

namespace Csla.Test.DataPortal
{
    [Serializable()]
    public class StronglyTypedDP : BusinessBase<StronglyTypedDP>
    {
        private string _data = "";
        private int _ID;

        protected override object GetIdValue()
        {
            return _data;
        }

        public string Data
        {
            get { return _data; }
            set
            {
                _data = value;
                PropertyHasChanged();
            }    
        }

        public int ID
        {
            get { return _ID; }
            set 
            {
                _ID = value;
                PropertyHasChanged();
            }
        }

        //criteria class needs to be protected since DataPortal_xyz methods are 
        //protected
        [Serializable()]
        protected class Criteria
        {
            public string _data;
            public int _ID;

            public Criteria()
            {
                _data = "new default data";
                _ID = 56;
            }

            public Criteria(int id)
            {
                this._ID = id;
                this._data = "fetched existing data";
            }
        }

        public static StronglyTypedDP NewStronglyTypedDP()
        {
            return Csla.DataPortal.Create<StronglyTypedDP>(new StronglyTypedDP.Criteria());
        }

        public static StronglyTypedDP GetStronglyTypedDP(int id)
        {
            return Csla.DataPortal.Fetch<StronglyTypedDP>(new StronglyTypedDP.Criteria(id));
        }

        public static void DeleteStronglyTypedDP(int id)
        {
          Csla.DataPortal.Delete<StronglyTypedDP>(new Criteria(id));
        }

        private StronglyTypedDP()
        {
            //prevent direct creation
        }

        protected void DataPortal_Create(StronglyTypedDP.Criteria criteria)
        {
            _data = criteria._data;
            _ID = criteria._ID;
            Csla.ApplicationContext.GlobalContext.Add("StronglyTypedDP", "Created");

            //System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();

            ////change 16 to whatever you want (chops off the nunit calls that get made every time)
            //for (int i = 0; i < st.FrameCount - 16; i++)
            //{
            //    string className = st.GetFrame(i).GetMethod().ReflectedType.ToString();
            //    string methodName = st.GetFrame(i).GetMethod().Name;
            //    string parameters = "";
            //    System.Reflection.ParameterInfo[] paramArray = st.GetFrame(i).GetMethod().GetParameters();

            //    foreach (System.Reflection.ParameterInfo pInfo in paramArray)
            //    {
            //        parameters += pInfo.Name;

            //        if (pInfo.Name != paramArray[paramArray.Length - 1].Name)
            //            parameters += ", ";
            //    }

            //    Console.WriteLine(className + "." + methodName + "(" + parameters + ")");
            //    Console.WriteLine();
            //}
        }

        protected void DataPortal_Fetch(StronglyTypedDP.Criteria criteria)
        {
            _data = criteria._data;
            _ID = criteria._ID;
            MarkOld();
            Csla.ApplicationContext.GlobalContext.Add("StronglyTypedDP", "Fetched");
        }

        protected override void DataPortal_Insert()
        {
            Csla.ApplicationContext.GlobalContext.Add("StronglyTypedDP", "Inserted");
        }

        protected override void DataPortal_Update()
        {
            Csla.ApplicationContext.GlobalContext.Add("StronglyTypedDP", "Updated");
        }

        protected override void DataPortal_DeleteSelf()
        {
            Csla.ApplicationContext.GlobalContext.Add("StronglyTypedDP", "Deleted self");
        }

        protected void DataPortal_Delete(StronglyTypedDP.Criteria criteria)
        {
            Csla.ApplicationContext.GlobalContext.Add("StronglyTypedDP_Criteria", criteria._ID);
        }
    }
}
