﻿using System;
using Csla.Xaml;
using System.Windows;
using Csla;

namespace SimpleApp
{
  public class CustomerViewModel : ViewModel<Library.CustomerEdit>
  {
    public CustomerViewModel()
    {
      BeginRefresh("BeginNewCustomer", DataPortal.ProxyModes.LocalOnly);
    }

    public CustomerViewModel(int id)
    {
      BeginRefresh("BeginGetCustomer", DataPortal.ProxyModes.LocalOnly);
    }

    protected override void OnError(Exception error)
    {
      MessageBox.Show(error.Message);
    }

    protected override void OnRefreshed()
    {
      base.OnRefreshed();
    }
  }
}
