namespace DeepData
{

	///<summary>
	///</summary>
	partial class Form1
	{
		/// <summary>
		/// Required designer variable
		/// </summary>
		private System.ComponentModel.IContainer components;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{

			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		//Required by the Windows Form Designer

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the content of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources =
				new System.ComponentModel.ComponentResourceManager(typeof(Form1));
			components = new System.ComponentModel.Container();
			OrderListDataGridView = new System.Windows.Forms.DataGridView();
			DataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			DataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			OrderListBindingSource = new System.Windows.Forms.BindingSource(components);
			LineItemsBindingSource = new System.Windows.Forms.BindingSource(components);
			LineItemsDataGridView = new System.Windows.Forms.DataGridView();
			DataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			DataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			DataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			DetailsBindingSource = new System.Windows.Forms.BindingSource(components);
			DetailsDataGridView = new System.Windows.Forms.DataGridView();
			DataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			DataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			DataGridViewTextBoxColumn8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			DataGridViewTextBoxColumn9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			((System.ComponentModel.ISupportInitialize)(OrderListDataGridView)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(OrderListBindingSource)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(LineItemsBindingSource)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(LineItemsDataGridView)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(DetailsBindingSource)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(DetailsDataGridView)).BeginInit();
			SuspendLayout();
			//
			//OrderListDataGridView
			//
			OrderListDataGridView.AutoGenerateColumns = false;
			OrderListDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[]
		                                            	{
		                                            		DataGridViewTextBoxColumn1, 
															DataGridViewTextBoxColumn2
		                                            	});
			OrderListDataGridView.DataSource = OrderListBindingSource;
			OrderListDataGridView.Location = new System.Drawing.Point(12, 12);
			OrderListDataGridView.Name = "OrderListDataGridView";
			OrderListDataGridView.Size = new System.Drawing.Size(245, 220);
			OrderListDataGridView.TabIndex = 1;
			//
			//DataGridViewTextBoxColumn1
			//
			DataGridViewTextBoxColumn1.DataPropertyName = "Customer";
			DataGridViewTextBoxColumn1.HeaderText = "Customer";
			DataGridViewTextBoxColumn1.Name = "DataGridViewTextBoxColumn1";
			DataGridViewTextBoxColumn1.ReadOnly = true;
			//
			//DataGridViewTextBoxColumn2
			//
			DataGridViewTextBoxColumn2.DataPropertyName = "Id";
			DataGridViewTextBoxColumn2.HeaderText = "Id";
			DataGridViewTextBoxColumn2.Name = "DataGridViewTextBoxColumn2";
			DataGridViewTextBoxColumn2.ReadOnly = true;
			//
			//OrderListBindingSource
			//
			OrderListBindingSource.DataSource = typeof(DeepData.Library.OrderInfo);
			//
			//LineItemsBindingSource
			//
			LineItemsBindingSource.DataMember = "LineItems";
			LineItemsBindingSource.DataSource = OrderListBindingSource;
			//
			//LineItemsDataGridView
			//
			LineItemsDataGridView.AutoGenerateColumns = false;
			LineItemsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[]
		                                            	{
		                                            		DataGridViewTextBoxColumn3, 
															DataGridViewTextBoxColumn4, 
															DataGridViewTextBoxColumn5
		                                            	});
			LineItemsDataGridView.DataSource = LineItemsBindingSource;
			LineItemsDataGridView.Location = new System.Drawing.Point(276, 12);
			LineItemsDataGridView.Name = "LineItemsDataGridView";
			LineItemsDataGridView.Size = new System.Drawing.Size(343, 220);
			LineItemsDataGridView.TabIndex = 1;
			//
			//DataGridViewTextBoxColumn3
			//
			DataGridViewTextBoxColumn3.DataPropertyName = "Product";
			DataGridViewTextBoxColumn3.HeaderText = "Product";
			DataGridViewTextBoxColumn3.Name = "DataGridViewTextBoxColumn3";
			DataGridViewTextBoxColumn3.ReadOnly = false;
			//
			//DataGridViewTextBoxColumn4
			//
			DataGridViewTextBoxColumn4.DataPropertyName = "OrderId";
			DataGridViewTextBoxColumn4.HeaderText = "OrderId";
			DataGridViewTextBoxColumn4.Name = "DataGridViewTextBoxColumn4";
			DataGridViewTextBoxColumn4.ReadOnly = true;
			//
			//DataGridViewTextBoxColumn5
			//
			DataGridViewTextBoxColumn5.DataPropertyName = "Id";
			DataGridViewTextBoxColumn5.HeaderText = "Id";
			DataGridViewTextBoxColumn5.Name = "DataGridViewTextBoxColumn5";
			DataGridViewTextBoxColumn5.ReadOnly = true;
			//
			//DetailsBindingSource
			//
			DetailsBindingSource.DataMember = "Details";
			DetailsBindingSource.DataSource = LineItemsBindingSource;
			//
			//DetailsDataGridView
			//
			DetailsDataGridView.AutoGenerateColumns = false;
			DetailsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[]
		                                          	{
		                                          		DataGridViewTextBoxColumn6, 
														DataGridViewTextBoxColumn7, 
														DataGridViewTextBoxColumn8, 
														DataGridViewTextBoxColumn9
		                                          	});
			DetailsDataGridView.DataSource = DetailsBindingSource;
			DetailsDataGridView.Location = new System.Drawing.Point(175, 249);
			DetailsDataGridView.Name = "DetailsDataGridView";
			DetailsDataGridView.Size = new System.Drawing.Size(444, 237);
			DetailsDataGridView.TabIndex = 2;
			//
			//DataGridViewTextBoxColumn6
			//
			DataGridViewTextBoxColumn6.DataPropertyName = "Detail";
			DataGridViewTextBoxColumn6.HeaderText = "Detail";
			DataGridViewTextBoxColumn6.Name = "DataGridViewTextBoxColumn6";
			DataGridViewTextBoxColumn6.ReadOnly = true;
			//
			//DataGridViewTextBoxColumn7
			//
			DataGridViewTextBoxColumn7.DataPropertyName = "OrderId";
			DataGridViewTextBoxColumn7.HeaderText = "OrderId";
			DataGridViewTextBoxColumn7.Name = "DataGridViewTextBoxColumn7";
			DataGridViewTextBoxColumn7.ReadOnly = true;
			//
			//DataGridViewTextBoxColumn8
			//
			DataGridViewTextBoxColumn8.DataPropertyName = "Id";
			DataGridViewTextBoxColumn8.HeaderText = "Id";
			DataGridViewTextBoxColumn8.Name = "DataGridViewTextBoxColumn8";
			DataGridViewTextBoxColumn8.ReadOnly = true;
			//
			//DataGridViewTextBoxColumn9
			//
			DataGridViewTextBoxColumn9.DataPropertyName = "LineId";
			DataGridViewTextBoxColumn9.HeaderText = "LineId";
			DataGridViewTextBoxColumn9.Name = "DataGridViewTextBoxColumn9";
			DataGridViewTextBoxColumn9.ReadOnly = true;
			//
			//Form1
			//
			AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			ClientSize = new System.Drawing.Size(639, 626);
			Controls.Add(DetailsDataGridView);
			Controls.Add(LineItemsDataGridView);
			Controls.Add(OrderListDataGridView);
			Name = "Form1";
			Text = "Form1";
			Load += new System.EventHandler(Form1_Load);
			((System.ComponentModel.ISupportInitialize)(OrderListDataGridView)).EndInit();
			((System.ComponentModel.ISupportInitialize)(OrderListBindingSource)).EndInit();
			((System.ComponentModel.ISupportInitialize)(LineItemsBindingSource)).EndInit();
			((System.ComponentModel.ISupportInitialize)(LineItemsDataGridView)).EndInit();
			((System.ComponentModel.ISupportInitialize)(DetailsBindingSource)).EndInit();
			((System.ComponentModel.ISupportInitialize)(DetailsDataGridView)).EndInit();
			ResumeLayout(false);
			PerformLayout();

		}
		#endregion
		internal System.Windows.Forms.BindingSource OrderListBindingSource;
		internal System.Windows.Forms.DataGridView OrderListDataGridView;
		internal System.Windows.Forms.DataGridViewTextBoxColumn DataGridViewTextBoxColumn1;
		internal System.Windows.Forms.DataGridViewTextBoxColumn DataGridViewTextBoxColumn2;
		internal System.Windows.Forms.BindingSource LineItemsBindingSource;
		internal System.Windows.Forms.DataGridView LineItemsDataGridView;
		internal System.Windows.Forms.DataGridViewTextBoxColumn DataGridViewTextBoxColumn3;
		internal System.Windows.Forms.DataGridViewTextBoxColumn DataGridViewTextBoxColumn4;
		internal System.Windows.Forms.DataGridViewTextBoxColumn DataGridViewTextBoxColumn5;
		internal System.Windows.Forms.BindingSource DetailsBindingSource;
		internal System.Windows.Forms.DataGridView DetailsDataGridView;
		internal System.Windows.Forms.DataGridViewTextBoxColumn DataGridViewTextBoxColumn6;
		internal System.Windows.Forms.DataGridViewTextBoxColumn DataGridViewTextBoxColumn7;
		internal System.Windows.Forms.DataGridViewTextBoxColumn DataGridViewTextBoxColumn8;
		internal System.Windows.Forms.DataGridViewTextBoxColumn DataGridViewTextBoxColumn9;

	}
}