using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Rolodex.Silverlight.Services;

namespace Rolodex.Silverlight.Events
{
    public class ServiceEventArgs
    {
        public ServiceEventArgs(RolodexService service, object parameter)
        {
            Service = service;
            Parameter = parameter;
        }

        public ServiceEventArgs(RolodexService service, string regionName, object parameter)
        {
            Service = service;
            Parameter = parameter;
            RegionName = regionName;
        }

        public ServiceEventArgs(RolodexService service)
        {
            Service = service;
        }

        public RolodexService Service { get; private set; }
        public object Parameter { get; private set; }
        public string RegionName { get; private set; }
    }
}
