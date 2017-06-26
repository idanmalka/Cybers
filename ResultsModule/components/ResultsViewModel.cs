using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Cybers.Infrustructure;
using Cybers.Infrustructure.interfaces;
using Cybers.Infrustructure.models;
using MaterialDesignThemes.Wpf;
using Microsoft.Practices.ServiceLocation;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using ResultsModule.interfaces;
using ResultsModule.models;

namespace ResultsModule.components
{
    public class ResultsViewModel : BindableBase, IResultsViewModel, INavigationAware
    {

        public enum ExportOptionsEnum
        {
            Cyber_Detection,
            Pajek
        }

        #region Private Fields

        private readonly IEventAggregator _eventAggregator;
        private readonly IRegionManager _regionManager;
        private readonly IIOService _ioService;

        private ObservableCollection<UserSuspicion> _usersSuspicionLevel;
        private Partition _partition;
        private ObservableCollection<ChartData> _distributionData;
        private string _selectedExportOption;
        private string _exportOptionDescription;
        private SnackbarMessageQueue _messageQueue;
        private AlgorithmResultsEventArgs _exportResultArgs;
        private string _selectedClusterId;
        private string _selectedAttribute;

        #endregion

        #region Properties

        public DelegateCommand GoToWelcomeScreenCommand { get; }
        public DelegateCommand ExportCommand { get; set; }

        public Dictionary<KeyValuePair<string, string>, Dictionary<long, long>> AttributeRarityMeasurement { get; set; }

        public ObservableCollection<string> ClusterIds { get; set; }
        public ObservableCollection<string> AttributeNames { get; set; }

        public bool KeepAlive { get; set; } = true;

        public SnackbarMessageQueue MessageQueue
        {
            get => _messageQueue;
            set => SetProperty(ref _messageQueue, value);
        }

        public ObservableCollection<ChartData> DistributionData
        {
            get => _distributionData;
            set => SetProperty(ref _distributionData, value);
        }

        public ObservableCollection<UserSuspicion> UsersSuspicionLevel
        {
            get => _usersSuspicionLevel;
            set => SetProperty(ref _usersSuspicionLevel, value);
        }

        public Partition Partition
        {
            get => _partition;
            set => SetProperty(ref _partition, value);
        }


        public string ExportOptionDescription
        {
            get => _exportOptionDescription;
            set => SetProperty(ref _exportOptionDescription, value);
        }

        public List<string> ExportOptions { get; set; }

        public string SelectedExportOption
        {
            get => _selectedExportOption;
            set
            {
                switch (value)
                {
                    case "Cyber_Detection":
                        ExportOptionDescription = "Export results for further analysis in Cybers Detection Application";
                        break;
                    case "Pajek":
                        ExportOptionDescription = "Export results for visualization in pajek";
                        break;
                }
                _selectedExportOption = value;
            }
        }

        public string SelectedClusterId
        {
            get => _selectedClusterId;
            set
            {
                _selectedClusterId = value;
                OnChartDisplayChanged(value);
            }
        }

        public string SelectedAttribute
        {
            get => _selectedAttribute;
            set
            {
                _selectedAttribute = value;
                OnChartDisplayChanged(value);
            }
        }

        #endregion

        #region Methods

        public ResultsViewModel(IEventAggregator eventAggregator, IRegionManager regionManager, IIOService ioService)
        {
            GoToWelcomeScreenCommand = new DelegateCommand(GoToWelcomeScreen);
            ExportCommand = new DelegateCommand(OnExportToFile);
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            _ioService = ioService;

            ExportOptions = new List<string>();
            MessageQueue = new SnackbarMessageQueue();

            foreach (var option in Enum.GetValues(typeof(ExportOptionsEnum)))
            {
                ExportOptions.Add(option.ToString());
            }

            ClusterIds = new ObservableCollection<string>();
            AttributeNames = new ObservableCollection<string>();

            UsersSuspicionLevel = CreateData();
            DistributionData = CreateChartData();
            _eventAggregator.GetEvent<AlgorithmResultsEvent>().Subscribe(arg =>
            {
                _exportResultArgs = arg;
                UsersSuspicionLevel = new ObservableCollection<UserSuspicion>(arg.UsersSuspicionLevel.Select(kvp => new UserSuspicion(kvp.Key, kvp.Value)).ToList());
                Partition = arg.Partition;
                AttributeRarityMeasurement = arg.AttributesRarityMeasurement;

                foreach (var cluster in Partition.Clusters)
                    ClusterIds.Add(cluster.Id.ToString());

                foreach (var attribute in arg.DistributionAttributes)
                    AttributeNames.Add(attribute);
            });
        }

        private void OnExportToFile()
        {
            var res = false;
            switch (SelectedExportOption)
            {
                case "Cyber_Detection":
                    res = Task<bool>.Factory.StartNew(() => _ioService.ExportResultsToFile(_exportResultArgs)).Result;
                    break;
                case "Pajek":
                    res = Task<bool>.Factory.StartNew(() => _ioService.ExportResultsToPajekFile(Partition)).Result;
                    break;
            }

            if (res)
                MessageQueue.Enqueue("Exported successfully");

        }

        private void GoToWelcomeScreen()
        {
            _eventAggregator.GetEvent<KeepAliveEvent>().Publish();

            var uri = new Uri("WelcomeView", UriKind.Relative);
            _regionManager.RequestNavigate(RegionNames.MainContentRegion, uri);
            KeepAlive = false;
        }

        private static ObservableCollection<ChartData> CreateChartData()
        {

            return new ObservableCollection<ChartData>
            {
                new ChartData { Attribute = "2006", UsersPerAttribute = 55 },

            };
        }

        private static ObservableCollection<UserSuspicion> CreateData()
        {
            return new ObservableCollection<UserSuspicion>
            {
                new UserSuspicion
                {
                    Key = "A",
                    Level = 20
                },
                new UserSuspicion
                {
                    Key = "B",
                    Level = 40
                },
                new UserSuspicion
                {
                    Key = "C",
                    Level = 60
                },
                new UserSuspicion
                {
                    Key = "B",
                    Level = 40
                },
                new UserSuspicion
                {
                    Key = "C",
                    Level = 60
                },
                new UserSuspicion
                {
                    Key = "B",
                    Level = 40
                },
                new UserSuspicion
                {
                    Key = "C",
                    Level = 60
                },
                new UserSuspicion
                {
                    Key = "B",
                    Level = 40
                },
                new UserSuspicion
                {
                    Key = "C",
                    Level = 60
                },
                new UserSuspicion
                {
                    Key = "B",
                    Level = 40
                },
                new UserSuspicion
                {
                    Key = "C",
                    Level = 60
                },
                new UserSuspicion
                {
                    Key = "B",
                    Level = 40
                },
                new UserSuspicion
                {
                    Key = "C",
                    Level = 60
                },
                new UserSuspicion
                {
                    Key = "B",
                    Level = 40
                },
                new UserSuspicion
                {
                    Key = "C",
                    Level = 60
                },
                new UserSuspicion
                {
                    Key = "B",
                    Level = 40
                },
                new UserSuspicion
                {
                    Key = "C",
                    Level = 60
                },
                new UserSuspicion
                {
                    Key = "B",
                    Level = 40
                },
                new UserSuspicion
                {
                    Key = "C",
                    Level = 60
                },
                new UserSuspicion
                {
                    Key = "B",
                    Level = 40
                },
                new UserSuspicion
                {
                    Key = "C",
                    Level = 60
                }

            };
        }

        private void OnChartDisplayChanged(string argValue, [CallerMemberName]string argName = null)
        {
            if (argName == nameof(SelectedClusterId))
            {

                return;
            }

            if (argName == nameof(SelectedAttribute))
            {
                return;
            }
           
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return KeepAlive;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
        }

        #endregion
    }
}
