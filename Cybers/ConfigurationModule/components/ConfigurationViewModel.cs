using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using ConfigurationModule.interfaces;
using Cybers.Infrustructure;
using Cybers.Infrustructure.interfaces;
using Cybers.Infrustructure.models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;

namespace ConfigurationModule.components
{
    
    public class ConfigurationViewModel: BindableBase, IConfigurationViewModel, INavigationAware
    {
        public DelegateCommand<string> TextFieldFocusedCommand { get; }
        public DelegateCommand NextCommand { get; }

        private bool _isNew;
        public bool IsNew
        {
            get => _isNew;
            set => SetProperty(ref _isNew, value);
        }

        private string _graphFilePath;
        public string GraphFilePath
        {
            get => _graphFilePath;
            set => SetProperty(ref _graphFilePath, value);
        }

        private string _configFilePath;
        public string ConfigFilePath
        {
            get => _configFilePath;
            set => SetProperty(ref _configFilePath, value);
        }

        private readonly IRegionManager _regionManager;
        private readonly IIOService _ioService;
        private int _distributionThreshold;
        private readonly IEventAggregator _eventAggregator;

        public ConfigurationViewModel(IRegionManager regionManager, IIOService ioService, IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _ioService = ioService;
            _eventAggregator = eventAggregator;
            DistributionThresholds = new List<int> { 5, 10, 15, 20, 25 };
            GoBackCommand = new DelegateCommand(GoBack);
            NextCommand = new DelegateCommand(Next);
            TextFieldFocusedCommand = new DelegateCommand<string>(OpenFileBrowser);
            ItemsClustering = CreateData();
            ItemsDistribution = CreateData();
        }

        private void Next()
        {
            var uri = new Uri(typeof(AlgorithmModule.components.AlgorithmLoadingView).FullName, UriKind.Relative);
            _regionManager.RequestNavigate(RegionNames.MainContentRegion, uri);

            _eventAggregator.GetEvent<AlgorithmAttributesEvent>().Publish(new AlgorithmAttributesEventArgs
            {
                ClustringAttributes = ItemsClustering.Where(a => a.IsSelected).ToList(),
                DistributingAttributes = ItemsDistribution.Where(a => a.IsSelected).ToList(),
                GraphFilePath = this.GraphFilePath,
                Threshold = DistributionThreshold
            });
        }

        private void OpenFileBrowser(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                _ioService?.OpenFileDialog((sender, result) =>
                {
                    var path = result.Object;
                    switch (value)
                    {
                        case "Graph":
                            GraphFilePath = path;
                            break;
                        case "Config":
                            ConfigFilePath = path;
                            break;
                    }
                });
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
                IsNew = mode.Equals("NEW");
            }
        }

        public ObservableCollection<UserAttribute> ItemsClustering { get; }
        public ObservableCollection<UserAttribute> ItemsDistribution { get; }
        public IList<int> DistributionThresholds { get; }

        private static ObservableCollection<UserAttribute> CreateData()
        {
            return new ObservableCollection<UserAttribute>
            {
                new UserAttribute
                {
                    Key = "Name",
                    Value = "Test1"
                },
                new UserAttribute
                {
                    Key = "Country",
                    Value = "Test2"
                },
                new UserAttribute
                {
                    Key = "Age",
                    Value = "Test3"
                },
                new UserAttribute
                {
                    Key = "Name",
                    Value = "Test1"
                },
                new UserAttribute
                {
                    Key = "Country",
                    Value = "Test2"
                },
                new UserAttribute
                {
                    Key = "Age",
                    Value = "Test3"
                },
                new UserAttribute
                {
                    Key = "Name",
                    Value = "Test1"
                },
                new UserAttribute
                {
                    Key = "Country",
                    Value = "Test2"
                },
                new UserAttribute
                {
                    Key = "Age",
                    Value = "Test3"
                },
                new UserAttribute
                {
                    Key = "Name",
                    Value = "Test1"
                },
                new UserAttribute
                {
                    Key = "Country",
                    Value = "Test2"
                },
                new UserAttribute
                {
                    Key = "Age",
                    Value = "Test3"
                }

            };
        }

        public int DistributionThreshold
        {
            get => _distributionThreshold;
            set => SetProperty(ref _distributionThreshold, value);
        }
    }
}
