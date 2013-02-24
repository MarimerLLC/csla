//-----------------------------------------------------------------------
// <copyright file="OrderMaint.cs" company="Marimer LLC">
//     Copyright (c) Marimer LLC. All rights reserved.
//     Website: http://www.lhotka.net/cslanet/
// </copyright>
// <summary></summary>
//-----------------------------------------------------------------------
using System;
using System.Windows.Forms;
using ActionExtenderSample.Business;
using Csla.Windows;

namespace ActionExtenderSample
{
  public partial class OrderMaint : Form
  {
    private Order _order = null;

    private OrderMaint()
    {
    }

    public OrderMaint(Guid orderId)
    {
      InitializeComponent();

      _order = Order.GetOrder(orderId);
      BindUI();
    }

    private void BindUI()
    {
      cslaActionExtender1.ResetActionBehaviors(_order);
    }

    private void cslaActionExtender1_SetForNew(object sender, CslaActionEventArgs e)
    {
      _order = Order.NewOrder();
      BindUI();
    }

    private void orderDetailListDataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
    {
      e.Cancel = true;
    }

    private void orderDateTextBox_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
      var textBox = sender as TextBox;
      if (textBox != null && textBox.Name == "orderDateTextBox")
        orderBindingSource.BindingComplete += orderBindingSource_BindingComplete;
    }

    private void orderBindingSource_BindingComplete(object sender, BindingCompleteEventArgs e)
    {
      orderBindingSource.BindingComplete -= orderBindingSource_BindingComplete;
      if (e.BindingCompleteState != BindingCompleteState.Success)
      {
        var textBox = e.Binding.BindableComponent as TextBox;
        if (textBox != null && textBox.Name == "orderDateTextBox")
        {
          textBox.Text = ((sender as BindingSource).Current as Order).OrderDate;
        }
      }
    }
  }
}
