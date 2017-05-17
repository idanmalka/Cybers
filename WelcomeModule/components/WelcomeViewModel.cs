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
        public DelegateCommand<string> NavigateToConfigView { get; }
        private readonly IRegionManager _regionManager;

        public WelcomeViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            NavigateToConfigView = new DelegateCommand<string>(Navigate);
        }

        private void Navigate(string value)
        {
            var uriQuery = new NavigationParameters {{"Mode", value}};
            var uri = new Uri(typeof(ConfigurationModule.components.ConfigurationWelcomeView).FullName + uriQuery, UriKind.Relative);
            _regionManager.RequestNavigate(RegionNames.MainContentRegion, uri);
        }

    }
}
