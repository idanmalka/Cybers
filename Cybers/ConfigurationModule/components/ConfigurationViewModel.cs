using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using ConfigurationModule.interfaces;
using Cybers.Infrustructure;
using Cybers.Infrustructure.interfaces;
using Cybers.Infrustructure.models;
using MaterialDesignThemes.Wpf;
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
        public DelegateCommand SaveConfigurationCommand { get; set; }

        private SnackbarMessageQueue _messageQueue;
        public SnackbarMessageQueue MessageQueue
        {
            get => _messageQueue;
            set => SetProperty(ref _messageQueue, value);
        }

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

        public string ConfigToolTip
        {
            get => _configToolTip;
            set => SetProperty(ref _configToolTip, value);
        }

        private readonly IRegionManager _regionManager;
        private readonly IIOService _ioService;
        private int _distributionThreshold;
        private readonly IEventAggregator _eventAggregator;
        private string _configToolTip;

        public ConfigurationViewModel(IRegionManager regionManager, IIOService ioService, IEventAggregator eventAggregator)
        {
            _regionManager = regionManager;
            _ioService = ioService;
            _eventAggregator = eventAggregator;
            DistributionThresholds = new List<int> { 5, 10, 15, 20, 25 };

            GoBackCommand = new DelegateCommand(GoBack);
            NextCommand = new DelegateCommand(Next, NextCanExecute);
            SaveConfigurationCommand = new DelegateCommand(OnSaveConfiguration,SaveConfigurationCanExecute);

            TextFieldFocusedCommand = new DelegateCommand<string>(OpenFileBrowser);
            ItemsClustering = new ObservableCollection<UserAttribute>();
            ItemsDistribution = new ObservableCollection<UserAttribute>();

            ConfigToolTip = "Please load graph file first";
            
            MessageQueue = new SnackbarMessageQueue();

            _eventAggregator.GetEvent<KeepAliveEvent>().Subscribe(() =>
            {
                KeepAlive = false;
            });
        }

        private bool SaveConfigurationCanExecute()
        {
            return ItemsClustering.Any(item => item.IsSelected) || ItemsDistribution.Any(item => item.IsSelected);
        }

        private void OnSaveConfiguration()
        {
            var task = Task<bool>.Factory.StartNew(() =>
            {
                return _ioService.SaveConfigurationToJson(
                    ItemsClustering.Where(a => a.IsSelected).Select(a => a.Key).ToList(),
                    ItemsDistribution.Where(a => a.IsSelected).Select(a => a.Key).ToList());
            });

            var result = task.Result;
            if (result)
            {
                MessageQueue.Enqueue("Configuration saved successfully");
            }
        }

        private bool NextCanExecute()
        {
            return (IsNew || ConfigFilePath != null) && GraphFilePath != null;
            //return true; //for now
        }

        private void Next()
        {
            var arg = new AlgorithmAttributesEventArgs
            {
                ClustringAttributes = ItemsClustering.Where(a => a.IsSelected).Select(a => a.Key).ToList(),
                DistributingAttributes = ItemsDistribution.Where(a => a.IsSelected).Select(a => a.Key).ToList(),
                GraphFilePath = this.GraphFilePath,
                Threshold = DistributionThreshold
            };

            var uri = new Uri(typeof(AlgorithmModule.components.AlgorithmLoadingView).FullName, UriKind.Relative);
            _regionManager.RequestNavigate(RegionNames.MainContentRegion, uri);

            _eventAggregator.GetEvent<AlgorithmAttributesEvent>().Publish(arg);
        }

        private void OpenFileBrowser(string value)
        {
            if (value == "Config" && GraphFilePath == null)
                return;

            if (!string.IsNullOrWhiteSpace(value))
            {
                _ioService?.OpenFileDialog((sender, result) =>
                {
                    var path = result.Object;
                    switch (value)
                    {
                        case "Graph":
                            GraphFilePath = path;
                            LoadAttributes(path).Subscribe(attribute =>
                            {
                                var newAtt = new UserAttribute
                                {
                                    Key = attribute
                                };
                                newAtt.PropertyChanged += (s, e) => { SaveConfigurationCommand.RaiseCanExecuteChanged(); };
                                ItemsClustering.Add(newAtt);

                                newAtt = new UserAttribute
                                {
                                    Key = attribute
                                };
                                newAtt.PropertyChanged += (s, e) => { SaveConfigurationCommand.RaiseCanExecuteChanged(); };
                                ItemsDistribution.Add(newAtt);
                            }, exception => //TODO : show AlertDialog 
                                Console.WriteLine(exception.Message), () => ConfigToolTip = "Algorithm Properties File Path");
                            break;
                        case "Config":
                            ConfigFilePath = path;
                            LoadExistingConfiguration(path);
                            break;
                    }
                });
            }
        }

        private IObservable<string> LoadAttributes(string path)
        {
            return Observable.Create<string>(observer =>
            {
                try
                {
                    var user = _ioService.ReadUsersFromPath(path).First();
                    foreach (var userAttribute in user.Attributes)
                    {
                        observer.OnNext(userAttribute.Key);
                    }
                    observer.OnCompleted();
                }
                catch (Exception e)
                {
                    observer.OnError(e);
                }
                return Disposable.Empty;
            }).SubscribeOn(Scheduler.Default).ObserveOnDispatcher();
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
            KeepAlive = false;
            _regionManager.Regions[RegionNames.MainContentRegion].NavigationService.Journal.GoBack();
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
            var mode = navigationContext.Parameters["Mode"] as string;
            if (!string.IsNullOrWhiteSpace(mode))
            {
                IsNew = mode.Equals("NEW");
            }
        }

        public ObservableCollection<UserAttribute> ItemsClustering { get; set; }
        public ObservableCollection<UserAttribute> ItemsDistribution { get; set; }
        public IList<int> DistributionThresholds { get; }

        public int DistributionThreshold
        {
            get => _distributionThreshold;
            set => SetProperty(ref _distributionThreshold, value);
        }

        public bool KeepAlive { get; private set; } = true;
    }
}
