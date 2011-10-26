using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace FirstTest
{
    partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            DataItem.GetDataItem("Rocky", (o, a) =>
            {
                if (a.Error != null)
                    System.Diagnostics.Debug.Assert(false);
                this.DataContext = a.Object;
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var obj = (DataItem)this.DataContext;
            Label1.Text = obj.Name;
        }
    }
}
