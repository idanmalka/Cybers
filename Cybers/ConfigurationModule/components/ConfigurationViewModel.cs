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
        public DelegateCommand<string> TextFieldFocusedCommand { get; private set; }

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

        private string _graphFilePath;
        public string GraphFilePath
        {
            get => _graphFilePath;
            set
            {
                _graphFilePath = value;
                SetProperty(ref _graphFilePath, value);
            }
        }

        private string _configFilePath;
        public string ConfigFilePath
        {
            get => _configFilePath;
            set
            {
                _configFilePath = value;
                SetProperty(ref _configFilePath, value);
            }
        }

        private readonly IRegionManager _regionManager;
        private readonly IIOService _ioService;

        public ConfigurationViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
            _ioService = null;
            GoBackCommand = new DelegateCommand(GoBack);
            TextFieldFocusedCommand = new DelegateCommand<string>(OpenFileBrowser);
        }

        private void OpenFileBrowser(string value)
        {
            if (!string.IsNullOrWhiteSpace(value) && _ioService != null)
            {
                var path = _ioService.OpenFileDialog();
                switch (value)
                {
                    case "Graph":
                        GraphFilePath = path;
                        break;
                    case "Config":
                        ConfigFilePath = path;
                        break;
                }
            }
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
