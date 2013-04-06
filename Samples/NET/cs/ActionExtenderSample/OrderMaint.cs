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
	public partial class OrderMaint : Form
	{
		public OrderMaint()
		{
			InitializeComponent();
		}

		public OrderMaint(Guid orderId)
		{
			InitializeComponent();

			_Order = Order.GetOrderWithDetail(orderId);
			BindUI();
		}

		Order _Order = null;

		void BindUI()
		{
			cslaActionExtender1.ResetActionBehaviors(_Order);
		}

		private void cslaActionExtender1_SetForNew(object sender, CslaActionEventArgs e)
		{
			_Order = Order.NewOrder();
			BindUI();
		}
	}
}
