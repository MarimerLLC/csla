//-----------------------------------------------------------------------
// <copyright file="Form1.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: https://cslanet.com
// </copyright>
// <summary>no summary</summary>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NUnit.Framework;

namespace DataBindingApp
{
    [TestFixture]
    public partial class Form1 : Form
    {
        private ListBox listBox1;
        private Button button1;
        private TextBox textBox1;

        public Form1()
        {
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1.Location = new System.Drawing.Point(169, 26);
            this.textBox1.Size = new System.Drawing.Size(100, 20);
            this.textBox1.Text = "new element";
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(12, 12);
            this.listBox1.Size = new System.Drawing.Size(120, 95);
            this.button1.Location = new System.Drawing.Point(180, 83);
            this.button1.Size = new System.Drawing.Size(95, 23);
            this.button1.Text = "Add New Object";
            this.button1.Click += new System.EventHandler(this.button1_Click);
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.textBox1);
            this.Text = "Databinding Tests";
            this.Load += new EventHandler(Form1_Load);
        }

        private ListObject dataSource;
        //private BindingList<MyObject> dataSource;

        public class MyObject
        {
            private string _name;
            private int _age;

            public string Name
            {
                get { return _name; }
                set { _name = value; }
            }

            public int Age
            {
                get { return _age; }
                set { _age = value; }
            }

            public MyObject(string name, int age)
            {
                _name = name;
                _age = age;
            }
        }

        void Form1_Load(object sender, EventArgs e)
        {
            //dataSource = new BindingList<MyObject>();
            //for (int i = 0; i < 5; i++)
            //{
            //    dataSource.Add(new MyObject("element" + i, i));
            //}

            dataSource = ListObject.GetList();
            dataSource.RaiseListChangedEvents = true;
            dataSource.AllowNew = true;
            dataSource.AllowRemove = false;
            dataSource.AllowEdit = false;

            listBox1.DataSource = dataSource;
            listBox1.DisplayMember = "Data";
            dataSource.AddingNew += new AddingNewEventHandler(dataSource_AddingNew);
            dataSource.ListChanged += new ListChangedEventHandler(dataSource_ListChanged);
        }

        //create a new dataobject
        public void dataSource_AddingNew(object sender, AddingNewEventArgs e)
        {
            e.NewObject = new ListObject.DataObject(textBox1.Text, 45);
            //e.NewObject = new MyObject(textBox1.Text, 45);
            MessageBox.Show("AddingNew event raised");
        }

        //add the new object and test ICancelAddNew interface
        private void button1_Click(object sender, EventArgs e)
        {
            ListObject.DataObject newObject = dataSource.AddNew();

            //cancel new if textbox contains spaces
            if (newObject.Data.Contains(" "))
            {
                MessageBox.Show("CancelNew: data property cannot contain spaces");
                MessageBox.Show("CancelNew: index is " + dataSource.IndexOf(newObject).ToString());
                //discards pending object
                dataSource.CancelNew(dataSource.IndexOf(newObject));
                Console.WriteLine(dataSource.IndexOf(newObject));
            }
            else
            {
                //commits pending object
                dataSource.EndNew(dataSource.IndexOf(newObject));
            }

            //MyObject newObject = dataSource.AddNew();

            //if (newObject.Name.Contains(" "))
            //{
            //    MessageBox.Show("CancelNew: name property cannot contain spaces");
            //    //discards pending object
            //    dataSource.CancelNew(dataSource.IndexOf(newObject));
            //}
        }


        void dataSource_ListChanged(object sender, ListChangedEventArgs e)
        {
            MessageBox.Show("ListChanged event raised: " + e.ListChangedType.ToString());
        }

        [Test]
        public void RunFormWithBusinessListBaseObj()
        {
            Main();
        }

        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Form1 newForm1 = new Form1();
            Application.Run(newForm1);
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(292, 266);
            this.Name = "Databinding tests";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
        }
    }
}