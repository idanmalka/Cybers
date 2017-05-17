using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigurationModule.interfaces;
using Cybers.Infrustructure;
using Cybers.Infrustructure.interfaces;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace ConfigurationModule.components
{
    
    public class ConfigurationViewModel: BindableBase, IConfigurationViewModel, INavigationAware
    {
        public string MainContentTitle { get; set; } = "Configuration Main Content Title";
        public string BottomToolbarTitle { get; set; } = "Configuration Bottom Toolbar Title";

        private string _mode;
        public string Mode
        {
            get => _mode;
            set
            {
                _mode = value;
                SetProperty(ref _mode, value);
            }
        }

        private readonly IRegionManager _regionManager;

        public ConfigurationViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            GoBackCommand = new DelegateCommand(GoBack);
        }

        public DelegateCommand GoBackCommand { get; }

        private void GoBack()
        {
            _regionManager.Regions[RegionNames.MainContentRegion].NavigationService.Journal.GoBack();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            var mode = navigationContext.Parameters["Mode"] as string;
            if (!string.IsNullOrWhiteSpace(mode))
            {
                Mode = mode;
            }
        }
    }
}
