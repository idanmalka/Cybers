using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

        #endregion

        #region Properties

        public DelegateCommand GoToWelcomeScreenCommand { get; }
        public DelegateCommand ExportCommand { get; set; }

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


            UsersSuspicionLevel = CreateData();
            DistributionData = CreateChartData();
            _eventAggregator.GetEvent<AlgorithmResultsEvent>().Subscribe(arg =>
            {
                UsersSuspicionLevel = new ObservableCollection<UserSuspicion>(arg.UsersSuspicionLevel.Select(kvp => new UserSuspicion(kvp.Key, kvp.Value)).ToList());
                Partition = arg.Partition;
            });
        }

        private void OnExportToFile()
        {
            var res = false;
            switch (SelectedExportOption)
            {
                case "Cyber_Detection":
                    res = Task<bool>.Factory.StartNew(() => _ioService.ExportResultsToFile(UsersSuspicionLevel.ToList(), Partition)).Result;
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
        }

        private static ObservableCollection<ChartData> CreateChartData()
        {
            return new ObservableCollection<ChartData>
            {
                new ChartData { Year = 2006, Value = 50.5 },
                new ChartData { Year = 2008, Value = 20.0 },
                new ChartData { Year = 2009, Value = 5.5  },
                new ChartData { Year = 2010, Value = 12.5 },
                new ChartData { Year = 2011, Value = 18.0 },
                new ChartData { Year = 2012, Value = 22.0 },
                new ChartData { Year = 2013, Value = 19.8 }
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


        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
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
