using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ChildGrandChild.Business;
using Csla;
using Csla.Wpf;

namespace ChildGrandChild
{
    public partial class Page : UserControl
    {
        public Page()
        {
            InitializeComponent();
        }


        private void FetchComplete(object sender, DataPortalResult<ChildList> result)
        {
            //this.DataContext = result.Object;
           // BindDetails();
        }

        private void Fetch()
        {
            //busy.IsRunning = true;
                  ChildList.FetchByName(null,FetchComplete);
        }
    }
}
