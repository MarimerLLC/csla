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
