using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileUI
{
  public class OrderVm : INotifyPropertyChanged
  {
    internal async Task LoadData()
    {
      Model = await BusinessLibrary.Order.GetOrderAsync(441);
    }

    private BusinessLibrary.Order _model;
    public BusinessLibrary.Order Model
    {
      get { return _model; }
      private set
      {
        _model = value;
        OnPropertyChanged("Model");
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string propertyName)
    {
      if (PropertyChanged != null)
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }

    internal async Task SaveData()
    {
      await Model.SaveAsync();
    }
  }
}
