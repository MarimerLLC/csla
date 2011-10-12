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

namespace Rolodex.Silverlight.Controls
{
    public partial class Toolbar : UserControl
    {
        public Toolbar()
        {
            InitializeComponent();
        }

        public bool IsNewButtonShown
        {
            get { return NewButton.Visibility == Visibility.Visible; }
            set
            {
                if (value)
                {
                    NewButton.Visibility = Visibility.Visible;
                    ;
                }
                else
                {
                    NewButton.Visibility = Visibility.Collapsed;
                }
            }
        }
    }
}
