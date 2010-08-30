using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CslaStore.Business;
using Csla.Windows;

namespace Sample
{
	public partial class OrderMaint2 : Form
	{
		public OrderMaint2()
		{
			InitializeComponent();
		}

		public OrderMaint2(Guid orderId)
		{
			InitializeComponent();

			_Order = Order.GetOrderWithDetail(orderId);
			BindUI();
		}

		Order _Order = null;

		void BindUI()
		{
			_BindingTree = BindingSourceHelper.InitializeBindingSourceTree(this.components, orderBindingSource);
			_BindingTree.Bind(_Order);
		}

		BindingSourceNode _BindingTree = null;

		private void toolSave_Click(object sender, EventArgs e)
		{
			if (Save())
				MessageBox.Show("Order saved.");
		}

		private void toolSaveNew_Click(object sender, EventArgs e)
		{
			if (Save())
			{
				_Order = Order.NewOrder();
				BindUI();
			}
		}

		private void toolSaveClose_Click(object sender, EventArgs e)
		{
			if (Save())
			{
				this.Close();
			}
		}

		private void toolCancel_Click(object sender, EventArgs e)
		{
			_BindingTree.Cancel(_Order);
		}

		private void toolClose_Click(object sender, EventArgs e)
		{
			_BindingTree.Close();
		}

		bool Save()
		{
			bool ret = false;

			_BindingTree.Apply();

			try
			{
				_Order = _Order.Save();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			BindUI();

			ret = true;

			return ret;
		}
	}
}
