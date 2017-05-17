using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cybers.Infrustructure;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using WelcomeModule.interfaces;

namespace WelcomeModule.components
{
    public class WelcomeViewModel : BindableBase, IWelcomeViewModel
    {
        private readonly IRegionManager _regionManager;
        public DelegateCommand<string> NavigateToConfigView { get; private set; }

        public WelcomeViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            NavigateToConfigView = new DelegateCommand<string>(Navigate);
        }

        private void Navigate(string value)
        {
            Console.WriteLine(value);
            _regionManager.RequestNavigate(RegionNames.MainContentRegion, typeof(ConfigurationModule.components.ConfigurationWelcomeView).FullName);
        }
    }
}
