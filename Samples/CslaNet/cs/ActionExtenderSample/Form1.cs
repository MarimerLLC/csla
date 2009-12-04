using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Sample
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			string id = "aea60714-d38b-4c08-9c5c-22fe6e0e7e64";
			Guid orderId = new Guid(id);

			using (OrderMaint frm = new OrderMaint(orderId))
				frm.ShowDialog();
		}

		private void button2_Click(object sender, EventArgs e)
		{
			string id = "aea60714-d38b-4c08-9c5c-22fe6e0e7e64";
			Guid orderId = new Guid(id);

			using (OrderMaint2 frm = new OrderMaint2(orderId))
				frm.ShowDialog();
		}
	}
}
