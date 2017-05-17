using Cybers.Infrustructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Prism.Regions;
using WelcomeModule.components;
using WelcomeModule.interfaces;
using ConfigurationModule;

namespace WelcomeModule
{
    public class WelcomeModule : ModuleBase
    {
        public WelcomeModule(IUnityContainer container, IRegionManager regionManager) : base(container, regionManager)
        {
        }

        protected override void InitializeModule()
        {
            RegionManager.RegisterViewWithRegion(RegionNames.BottomToolbarReegion, typeof(WelcomeBottomToolbarView));
            RegionManager.RequestNavigate(RegionNames.MainContentRegion, typeof(WelcomeView).FullName);
        }

        protected override void RegisterTypes()
        {
            Container.RegisterType<IWelcomeToolbarView, WelcomeBottomToolbarView>();
            Container.RegisterType<IWelcomeView, WelcomeView>();
            Container.RegisterType<IWelcomeViewModel, WelcomeViewModel>();
            Container.RegisterTypeForNavigation<WelcomeView>();
        }
    }
}
