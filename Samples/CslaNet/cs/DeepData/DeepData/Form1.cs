using System;
using System.Windows.Forms;
using DeepData.Library;

namespace DeepData
{

	public partial class Form1 : Form
	{
		///<summary>
		///</summary>
		public Form1()
		{
			InitializeComponent();
		}

		private void OrderBindingNavigatorSaveItem_Click(object sender, EventArgs e)
		{
			Validate();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			OrderListBindingSource.DataSource = OrderList.GetList();
		}
	}
}
