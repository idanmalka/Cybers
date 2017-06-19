using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlgorithmModule.interfaces;
using Cybers.Infrustructure;
using Cybers.Infrustructure.interfaces;
using Cybers.Infrustructure.models;
using CybersDetectionAlgorithm;
using Prism.Commands;
using Prism.Events;
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
        private CybersDetectionResults _results;
        private IRegionManager _regionManager;

        #endregion

        #region Properties

        public DelegateCommand GoBackCommand { get; }
        public DelegateCommand TestCommand { get; }
        public DelegateCommand NextCommand { get; }
        public AlgorithmStep AlgStep
        {
            get => _algStep;
            set => SetProperty(ref _algStep, value);
        }

        public List<string> ClusteringAttributes { get; set; }
        public List<string> DistributingAttributes { get; set; }
        public string GraphFilePath { get; set; }
        public int DistributingThreshold { get; set; }

        #endregion

        #region Methods

        public AlgorithmViewModel(IEventAggregator eventAggregator, IRegionManager regionManager,IIOService ioService)
        {
            GoBackCommand = new DelegateCommand(GoBack);
            TestCommand = new DelegateCommand(Test);
            NextCommand = new DelegateCommand(OnNextCommandPressed);
            _regionManager = regionManager;
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<AlgorithmAttributesEvent>().Subscribe(obj =>
            {
                try
                {
                    ClusteringAttributes = obj.ClustringAttributes;
                    DistributingAttributes = obj.DistributingAttributes;
                    GraphFilePath = obj.GraphFilePath;
                    DistributingThreshold = obj.Threshold;

                    //_users = ioService.ReadUsersFromPath(obj.GraphFilePath);
                    //Task.Run(() => StartAlgorithm());
                }
                catch (IncorrectUsersFileException e)
                {
                    //Ignored for now
                }
            },true);
        }

        private void StartAlgorithm()
        {
            var algorithm = new CybersDetection(_users, ClusteringAttributes, DistributingAttributes, DistributingThreshold);

            algorithm.InitializationFinished += OnAlgorithmInitializationFinished;
            algorithm.ClusteringFinished += OnClusteringFinished;
            algorithm.DistributingFinished += OnDistributingFinished;

            algorithm.Execute();

            _results = algorithm.LatestRunResults;
        }

        private void GoBack()
        {
            _regionManager.Regions[RegionNames.MainContentRegion].NavigationService.Journal.GoBack();
        }

        private void OnDistributingFinished(object sender, EventArgs e)
        {
            AlgStep = AlgorithmStep.End;
        }

        private void OnClusteringFinished(object sender, EventArgs e)
        {
            AlgStep = AlgorithmStep.Distributing;
        }

        private void OnAlgorithmInitializationFinished(object sender, EventArgs e)
        {
            AlgStep = AlgorithmStep.Clustering;
        }

        private void Test()
        {
            switch (AlgStep)
            {
                case AlgorithmStep.Init:
                    AlgStep = AlgorithmStep.Clustering;
                    break;
                case AlgorithmStep.Clustering:
                    AlgStep = AlgorithmStep.Distributing;
                    break;
                case AlgorithmStep.Distributing:
                    AlgStep = AlgorithmStep.Finish;
                    break;
                case AlgorithmStep.Finish:
                    AlgStep = AlgorithmStep.End;
                    break;
                case AlgorithmStep.End:
                    AlgStep = AlgorithmStep.Init;
                    break;
            }
        }

        private void OnNextCommandPressed()
        {
            var uri = new Uri(typeof(ResultsModule.components.ResultsView).FullName, UriKind.Relative);
            _regionManager.RequestNavigate(RegionNames.MainContentRegion, uri);

            _eventAggregator.GetEvent<AlgorithmResultsEvent>().Publish(new AlgorithmResultsEventArgs
            {
                //UsersSuspicionLevel = _results.UsersSuspicionLevel,
                //Partition = _results.Partition
            });
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {

        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {

        }

        #endregion
    }
}
