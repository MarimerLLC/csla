using System;

namespace Rolodex.Silverlight.Services
{
    public class ServiceDefinition
    {
        private ServiceDefinition() { }

        public ServiceDefinition(Type viewInterfaceType, Type viewType, Type viewModelInterfaceType, Type viewModelType)
        {
            ViewInterfaceType = viewInterfaceType;
            ViewType = viewType;
            ViewModelInterfaceType = viewModelInterfaceType;
            ViewModelType = viewModelType;
        }

        public Type ViewInterfaceType { get; private set; }
        public Type ViewType { get; private set; }
        public Type ViewModelInterfaceType { get; private set; }
        public Type ViewModelType { get; private set; }
    }
}
