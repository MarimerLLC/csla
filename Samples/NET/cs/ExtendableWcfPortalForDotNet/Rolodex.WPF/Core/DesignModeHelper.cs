using System;
using System.Windows;
using System.ComponentModel;

namespace Rolodex.Silverlight.Core
{
    public static class DesignModeHelper
    {
        public static bool IsInDesignMode
        {
            get
            {
#if SILVERLIGHT
                if (Application.Current != null && Application.Current.RootVisual != null)
                {
                    return DesignerProperties.GetIsInDesignMode(Application.Current.RootVisual);
                }
                else
                    return false;
#else
                return DesignerProperties.GetIsInDesignMode(Application.Current.MainWindow);
#endif
            }
        }
    }
}
