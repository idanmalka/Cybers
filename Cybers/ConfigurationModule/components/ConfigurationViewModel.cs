using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
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

    public class ConfigurationViewModel : BindableBase, IConfigurationViewModel, INavigationAware
    {
        public DelegateCommand<string> TextFieldFocusedCommand { get; }
        public DelegateCommand NextCommand { get; }

        private bool _isNew;
        public bool IsNew
        {
            get => _isNew;
            set
            {
                SetProperty(ref _isNew, value);
                NextCommand.RaiseCanExecuteChanged();
            }
        }

        private string _graphFilePath;
        public string GraphFilePath
        {
            get => _graphFilePath;
            set
            {
                SetProperty(ref _graphFilePath, value);
                NextCommand.RaiseCanExecuteChanged();
            }
        }

        private string _configFilePath;
        public string ConfigFilePath
        {
            get => _configFilePath;
            set
            {
                SetProperty(ref _configFilePath, value);
                NextCommand.RaiseCanExecuteChanged();
            }
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
            NextCommand = new DelegateCommand(Next, NextCanExecute);
            TextFieldFocusedCommand = new DelegateCommand<string>(OpenFileBrowser);
            ItemsClustering = CreateData();
            ItemsDistribution = CreateData();
        }

        private bool NextCanExecute()
        {
            //return (IsNew || ConfigFilePath != null) && GraphFilePath != null;
            return true; //for now
        }

        private void Next()
        {
            var arg = new AlgorithmAttributesEventArgs
            {
                ClustringAttributes = ItemsClustering.Where(a => a.IsSelected).Select(a => a.Value).ToList(),
                DistributingAttributes = ItemsDistribution.Where(a => a.IsSelected).Select(a => a.Value).ToList(),
                GraphFilePath = this.GraphFilePath,
                Threshold = DistributionThreshold
            };

            var uri = new Uri(typeof(AlgorithmModule.components.AlgorithmLoadingView).FullName, UriKind.Relative);
            _regionManager.RequestNavigate(RegionNames.MainContentRegion, uri);

            _eventAggregator.GetEvent<AlgorithmAttributesEvent>().Publish(arg);
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
                            LoadExistingConfiguration(path);
                            break;
                    }
                });
            }
        }

        private void LoadExistingConfiguration(string path)
        {
            try
            {
                var attributes = _ioService.ReadConfigurationFromFile(path);

                foreach (var cAttribute in attributes.ClustringAttributes)
                {
                    var userAttribute = ItemsClustering.FirstOrDefault(a => a.Key == cAttribute);
                    if (userAttribute != null)
                        userAttribute.IsSelected = true;
                }

                foreach (var dAttribute in attributes.DistributingAttributes)
                {
                    var userAttribute = ItemsDistribution.FirstOrDefault(a => a.Key == dAttribute);
                    if (userAttribute != null)
                        userAttribute.IsSelected = true;
                }
            }
            catch(IncorrectConfigurationFileException)
            {
                // ignored for now
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

        public ObservableCollection<UserAttribute> ItemsClustering { get; set; }
        public ObservableCollection<UserAttribute> ItemsDistribution { get; set; }
        public IList<int> DistributionThresholds { get; }

        private static ObservableCollection<UserAttribute> CreateData()
        {
            return new ObservableCollection<UserAttribute>
            {
                new UserAttribute
                {
                    Key = "PostsNumber",
                    Value = "PostsNumber"
                },
                new UserAttribute
                {
                    Key = "CreationDate",
                    Value = "CreationDate"
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
