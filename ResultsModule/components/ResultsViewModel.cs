using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Cybers.Infrustructure;
using Cybers.Infrustructure.controls.UserDetails;
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
        private int _intervalBySelectedAttribute;

        #endregion

        #region Properties

        public DelegateCommand GoToWelcomeScreenCommand { get; }
        public DelegateCommand ExportCommand { get; set; }
        public DelegateCommand<UserSuspicion> ShowUserDetailsCommand { get; set; }

        public Dictionary<RarityKeyObject, RarityValueObject> AttributeRarityMeasurement { get; set; }

        public ObservableCollection<string> ClusterIds { get; set; }
        public ObservableCollection<string> AttributeNames { get; set; }

        public bool KeepAlive { get; set; } = true;

        public SnackbarMessageQueue MessageQueue
        {
            get => _messageQueue;
            set => SetProperty(ref _messageQueue, value);
        }

        public int IntervalBySelectedAttribute
        {
            get => _intervalBySelectedAttribute;
            set => SetProperty(ref _intervalBySelectedAttribute, value);
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
            ShowUserDetailsCommand = new DelegateCommand<UserSuspicion>(OnShowUserDetails);
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

            _eventAggregator.GetEvent<AlgorithmResultsEvent>().Subscribe(arg =>
            {
                _exportResultArgs = arg;
                UsersSuspicionLevel = new ObservableCollection<UserSuspicion>(arg.UsersSuspicionLevel);
                Partition = arg.Partition;
                AttributeRarityMeasurement = arg.AttributesRarityMeasurement;

                foreach (var cluster in Partition.Clusters)
                    ClusterIds.Add(cluster.Id.ToString());

                foreach (var attribute in arg.DistributionAttributes)
                    AttributeNames.Add(attribute);
            });
        }

        private void OnShowUserDetails(UserSuspicion obj)
        {
            var user = Partition.Clusters.First(c => c.Id == obj.ClusterId).Verticies.First(u => u.Index.ToString() == obj.Key);

            var userDetailView = new UserDetails(new UserDetailsViewModel(user));
            userDetailView.ShowDialog();
        }

        private void OnExportToFile()
        {
            var res = false;
            switch (SelectedExportOption)
            {
                case "Cyber_Detection":
                {
                    var task = Task.Run(() => _ioService.ExportResultsToFile(_exportResultArgs));
                    task.Wait();
                    res = task.Result;
                }
                    break;
                case "Pajek":
                {
                    var task = Task.Run(() => _ioService.ExportResultsToPajekFile(Partition));
                    task.Wait();
                    res = task.Result;
                }
                    break;
            }

            MessageQueue.Enqueue(res ? "Exported successfully" : "Export Failed");
        }

        private void GoToWelcomeScreen()
        {
            _eventAggregator.GetEvent<KeepAliveEvent>().Publish();

            var uri = new Uri("WelcomeView", UriKind.Relative);
            _regionManager.RequestNavigate(RegionNames.MainContentRegion, uri);
            KeepAlive = false;
        }


        private void OnChartDisplayChanged(string argValue, [CallerMemberName]string argName = null)
        {
            if (SelectedAttribute == null || SelectedClusterId == null) return;

            try
            {
                var key = new RarityKeyObject();

                foreach (var innerKey in AttributeRarityMeasurement.Keys)
                    if (innerKey.ClusterId == SelectedClusterId && innerKey.AttributeName == SelectedAttribute)
                        key = innerKey;
                if (key.ClusterId == null || key.AttributeName == null) return;

                var attributeValuesDictionary = AttributeRarityMeasurement[key];

                IntervalBySelectedAttribute = (int) attributeValuesDictionary.UsersPerValue.Values.Average();

                DistributionData = new ObservableCollection<ChartData>();

                attributeValuesDictionary.Keys.Select(value => new ChartData
                {
                    AttributeValue = CoulmnNameFromData(value, key.AttributeName),
                    UsersPerAttributeValue = attributeValuesDictionary[value]
                }).OrderBy(data => data.AttributeValue).ToObservable().SubscribeOn(Scheduler.Default).ObserveOnDispatcher()
                .Subscribe(data =>
                    {
                        DistributionData.Add(data);
                    });

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        private string CoulmnNameFromData(long value, string keyAttributeName)
        {
            switch (keyAttributeName)
            {
                case nameof(User.Gender):
                    return value == 1 ? "Male" : "Female";
                case nameof(User.CreationDate):
                    return value.ToString().Substring(2, 2);
            }
            return value.ToString();
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
