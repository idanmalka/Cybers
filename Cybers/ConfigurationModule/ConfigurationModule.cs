using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigurationModule.components;
using ConfigurationModule.interfaces;
using Cybers.Infrustructure;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;

namespace ConfigurationModule
{
    public class ConfigurationModule: IModule
    {
        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;

        public ConfigurationModule(IUnityContainer container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _container.RegisterType<IConfigurationToolbarView,ConfigurationBottomToolbarView>();
            _container.RegisterType<IConfigurationView, ConfigurationWelcomeView>();
            _container.RegisterType<IConfigurationViewModel, ConfigurationViewModel>();

            _regionManager.RegisterViewWithRegion(RegionNames.BottomToolbarReegion, typeof(ConfigurationBottomToolbarView));
            _regionManager.RegisterViewWithRegion(RegionNames.MainContentRegion, typeof(ConfigurationWelcomeView));

        }
    }
}
