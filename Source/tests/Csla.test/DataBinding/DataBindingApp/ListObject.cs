//-----------------------------------------------------------------------
// <copyright file="ListObject.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using Csla.Core;
using Csla;

namespace DataBindingApp
{
    [Serializable()]
    public class ListObject : BusinessListBase<ListObject, ListObject.DataObject>
    {
        [Serializable()]
        public class DataObject : BusinessBase<DataObject>
        {
            private int _ID;
            private string _data;
            private int _number;

            public int Number
            {
                get { return _number; }
                set { _number = value; }
            }

            public string Data
            {
                get { return _data; }
                set { _data = value; }
            }

            public int ID
            {
                get { return _ID; }
                set { _ID = value; }
            }

            protected override object GetIdValue()
            {
                 return _number;
            }

            public DataObject(string data, int number)
            {
                this.MarkAsChild();
                _data = data;
                _number = number;
                this.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(DataObject_PropertyChanged);
            }

            public void DataObject_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
            {
                Console.WriteLine("Property has changed");
            }
        }

        private ListObject()
        { }

        public static ListObject GetList()
        {
            ListObject list = new ListObject();

            for (int i = 0; i < 5; i++)
            {
                list.Add(new DataObject("element" + i, i));
            }

            return list;
        }


    }
}