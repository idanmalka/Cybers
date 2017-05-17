using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Cybers.Infrustructure;
using Cybers.Infrustructure.interfaces;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;

namespace ServicesModule
{
    public class ServicesModule: IModule
    {
        private readonly IUnityContainer _container;

        public ServicesModule(IUnityContainer container)
        {
            _container = container;
        }

        public void Initialize()
        {
            _container.RegisterType<IIOService, IOService>(new ContainerControlledLifetimeManager());
        }
    }
}
