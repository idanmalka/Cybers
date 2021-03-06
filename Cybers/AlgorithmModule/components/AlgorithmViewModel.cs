﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AlgorithmModule.interfaces;
using Cybers.Infrustructure;
using Cybers.Infrustructure.controls;
using Cybers.Infrustructure.interfaces;
using Cybers.Infrustructure.models;
using CybersDetectionAlgorithm;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using Prism.Regions;

namespace AlgorithmModule.components
{
    public class AlgorithmViewModel : BindableBase, IAlgorithmViewModel, INavigationAware
    {
        public enum AlgorithmStep
        {
            Init, Clustering, Distributing, Finish, End
        }

        #region Private Fields
        
        private AlgorithmStep _algStep = AlgorithmStep.Init;
        private readonly IEventAggregator _eventAggregator;
        private IEnumerable<User> _users;
        private AlgorithmResultsEventArgs _results;
        private readonly IRegionManager _regionManager;
        private string _initializingStatusText;
        private string _clusteringStatusText;
        private string _distributingStatusText;
        private string _finalizingStatusText;
        private long _numberOfClusters;

        #endregion

        #region Properties

        public DelegateCommand GoBackCommand { get; }
        public DelegateCommand NextCommand { get; }

        public List<string> ClusteringAttributes { get; set; }
        public List<string> DistributingAttributes { get; set; }
        public string GraphFilePath { get; set; }
        public int DistributingThreshold { get; set; }
        public InteractionRequest<AlertDialogNotification> AlertDialogRequest { get; }
        public bool KeepAlive { get; private set; } = true;

        public string InitializingStatusText
        {
            get => _initializingStatusText;
            set => SetProperty(ref _initializingStatusText, value);
        }

        public string ClusteringStatusText
        {
            get => _clusteringStatusText;
            set => SetProperty(ref _clusteringStatusText, value);
        }

        public string DistributingStatusText
        {
            get => _distributingStatusText;
            set => SetProperty(ref _distributingStatusText, value);
        }

        public string FinalizingStatusText
        {
            get => _finalizingStatusText;
            set => SetProperty(ref _finalizingStatusText, value); 
        }

        public AlgorithmStep AlgStep
        {
            get => _algStep;
            set => SetProperty(ref _algStep, value);
        }

        public long NumberOfClusters
        {
            get => _numberOfClusters;
            set => SetProperty(ref _numberOfClusters, value);
        }

        #endregion

        #region Methods

        public AlgorithmViewModel(IEventAggregator eventAggregator, IRegionManager regionManager,IIOService ioService)
        {
            GoBackCommand = new DelegateCommand(GoBack);
            NextCommand = new DelegateCommand(OnNextCommandPressed);
            _regionManager = regionManager;
            AlertDialogRequest = new InteractionRequest<AlertDialogNotification>();
            _eventAggregator = eventAggregator;

            _eventAggregator.GetEvent<KeepAliveEvent>().Subscribe(() =>
            {
                KeepAlive = false;
            });

            _eventAggregator.GetEvent<AlgorithmAttributesEvent>().Subscribe(obj =>
            {
                try
                {
                    ClusteringAttributes = obj.ClustringAttributes;
                    DistributingAttributes = obj.DistributingAttributes;
                    GraphFilePath = obj.GraphFilePath;
                    DistributingThreshold = obj.Threshold;

                    _users = ioService.ReadUsersFromPath(obj.GraphFilePath);
                    Task.Run(() => StartAlgorithm());
                }
                catch (IncorrectUsersFileException)
                {
                    //Ignored for now
                }
            },true);
        }

        private void StartAlgorithm()
        {
            var algorithm = new CybersDetection(_users, ClusteringAttributes, DistributingAttributes, DistributingThreshold);

            algorithm.InitializationStarted += (s,e) => InitializingStatusText = "Started: " + DateTime.Now;
            algorithm.ClusteringStarted += (s, e) => ClusteringStatusText = "Started: " + DateTime.Now;
            algorithm.DistributingStarted += (s, e) => DistributingStatusText = "Started: " + DateTime.Now;

            algorithm.InitializationFinished += OnAlgorithmInitializationFinished;
            algorithm.ClusteringFinished += OnClusteringFinished;
            algorithm.DistributingFinished += OnDistributingFinished;

            algorithm.RunDataUpdate += (s, e) =>
            {
                var clustersUsersCount = ((AlgorithmRunDataUpdateEventArgs) e).ClustersUsersCount;
                var numberOfClusters = clustersUsersCount.Count(d => d.Value != 0);
                Application.Current.Dispatcher.Invoke(() => NumberOfClusters = numberOfClusters);
            };

            algorithm.Execute();

            _results = algorithm.LatestRunResults;
        }


        private void GoBack()
        {
            RaiseConfirmation();
        }

        private void OnAlgorithmInitializationFinished(object sender, EventArgs e)
        {
            AlgStep = AlgorithmStep.Clustering;
            var str = InitializingStatusText;
            str += "\n Ended: " + DateTime.Now;
            InitializingStatusText = str;
        }

        private void OnClusteringFinished(object sender, EventArgs e)
        {
            AlgStep = AlgorithmStep.Distributing;
            var str = ClusteringStatusText;
            str += "\n Ended: " + DateTime.Now;
            ClusteringStatusText = str;
        }

        private void OnDistributingFinished(object sender, EventArgs e)
        {
            AlgStep = AlgorithmStep.End;
            var str = DistributingStatusText;
            str += "\n Ended: " + DateTime.Now;
            DistributingStatusText = str;
        }

        private void OnNextCommandPressed()
        {
            _eventAggregator.GetEvent<KeepAliveEvent>().Publish();

            var uri = new Uri(typeof(ResultsModule.components.ResultsView).FullName, UriKind.Relative);
            _regionManager.RequestNavigate(RegionNames.MainContentRegion, uri);

            _eventAggregator.GetEvent<AlgorithmResultsEvent>().Publish(_results);
        }

        private void RaiseConfirmation()
        {
            AlertDialogRequest.Raise(new AlertDialogNotification { Content = "Are you sure?", Title = "Stop Algorithm" },
                returned =>
                {
                    if (returned != null && returned.Confirmed)
                    {
                        _regionManager.Regions[RegionNames.MainContentRegion].NavigationService.Journal.GoBack();
                        KeepAlive = false;
                    }
                });
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {

        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return KeepAlive;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }

        #endregion
    }
}
