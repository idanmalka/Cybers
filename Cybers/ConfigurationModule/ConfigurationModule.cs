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
    public class ConfigurationModule : ModuleBase
    {
        public ConfigurationModule(IUnityContainer container, IRegionManager regionManager) : base(container, regionManager)
        {
        }

        protected override void InitializeModule()
        {
            RegionManager.RegisterViewWithRegion(RegionNames.BottomToolbarRegion, typeof(ConfigurationBottomToolbarView));
        }

        protected override void RegisterTypes()
        {
            Container.RegisterType<IConfigurationToolbarView, ConfigurationBottomToolbarView>();
            Container.RegisterType<IConfigurationView, ConfigurationWelcomeView>();
            Container.RegisterType<IConfigurationViewModel, ConfigurationViewModel>();
            Container.RegisterTypeForNavigation<ConfigurationWelcomeView>();
        }
    }
}
