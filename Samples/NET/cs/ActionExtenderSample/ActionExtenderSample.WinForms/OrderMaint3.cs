//-----------------------------------------------------------------------
// <copyright file="OrderMaint3.cs" company="Marimer LLC">
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
  public partial class OrderMaint3 : Form
  {
    private Order _order = null;

    private BindingSourceNode _bindingTree = null;

    private OrderMaint3()
    {
    }

    public OrderMaint3(Guid orderId)
    {
      InitializeComponent();

      _order = Order.GetOrder(orderId);
      BindUI();
    }

    private void BindUI()
    {
      _bindingTree = BindingSourceHelper.InitializeBindingSourceTree(components, orderBindingSource);
      _bindingTree.Bind(_order);
    }

    private void toolSave_Click(object sender, EventArgs e)
    {
      if (Save())
        MessageBox.Show("Order saved.");
    }

    private void toolSaveNew_Click(object sender, EventArgs e)
    {
      if (Save())
      {
        _order = Order.NewOrder();
        BindUI();
      }
    }

    private void toolSaveClose_Click(object sender, EventArgs e)
    {
      if (Save())
      {
        Close();
      }
    }

    private void toolCancel_Click(object sender, EventArgs e)
    {
      _bindingTree.Cancel(_order);
    }

    private void toolClose_Click(object sender, EventArgs e)
    {
      _bindingTree.Close();
      Close();
    }

    private bool Save()
    {
      bool ret = false;

      _bindingTree.Apply();

      try
      {
        _order = _order.Save();
      }
      catch (Exception ex)
      {
        MessageBox.Show(ex.Message);
      }
      BindUI();

      ret = true;

      return ret;
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
