using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;

namespace FirstTest
{
    partial class App
    {
        public App()
        {
            InitializeComponent();

            Csla.DataPortal.ProxyTypeName = "Local";
        }

        protected override void OnLaunched(LaunchActivatedEventArgs args)
        {
            Window.Current.Content = new MainPage();
            Window.Current.Activate();
        }
    }
}
