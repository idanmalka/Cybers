using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cybers.Infrustructure;
using Cybers.Infrustructure.interfaces;
using Cybers.Infrustructure.models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using WelcomeModule.interfaces;

namespace WelcomeModule.components
{
    public class WelcomeViewModel : BindableBase, IWelcomeViewModel
    {
        public DelegateCommand<string> NavigateToConfigView { get; }
        public DelegateCommand LoadPreviousResults { get; }

        private readonly IRegionManager _regionManager;
        private readonly IIOService _ioService;
        private readonly IEventAggregator _eventAggregator;

        public WelcomeViewModel(IRegionManager regionManager, IIOService ioService, IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _ioService = ioService;
            _eventAggregator = eventAggregator;
            NavigateToConfigView = new DelegateCommand<string>(Navigate);
            LoadPreviousResults = new DelegateCommand(OnLoadPreviousResults);
        }

        private void OnLoadPreviousResults()
        {
            _ioService?.OpenFileDialog((sender, result) =>
            {
                var path = result.Object;
                var args = _ioService.ImportPrevouseResultsFile(path);

                NavigateToResultsView();
                _eventAggregator.GetEvent<AlgorithmResultsEvent>().Publish(args);

            });



        }

        private void NavigateToResultsView()
        {
            var uri = new Uri("ResultsModule.components.ResultsView", UriKind.Relative);
            _regionManager.RequestNavigate(RegionNames.MainContentRegion, uri);
        }

        private void Navigate(string value)
        {
            var parameters = new NavigationParameters { { "Mode", value } };
            var uri = new Uri(typeof(ConfigurationModule.components.ConfigurationWelcomeView).FullName, UriKind.Relative);
            _regionManager.RequestNavigate(RegionNames.MainContentRegion, uri, parameters);
        }

    }
}
