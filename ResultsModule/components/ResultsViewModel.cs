using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cybers.Infrustructure.models;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using ResultsModule.interfaces;

namespace ResultsModule.components
{
    public class ResultsViewModel : BindableBase, IResultsViewModel, INavigationAware
    {
        #region Private Fields

        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        private Dictionary<string, double> _usersSuspicionLevel;
        private Partition _partition;

        #endregion

        #region Properties

        public Dictionary<string, double> UsersSuspicionLevel
        {
            get => _usersSuspicionLevel;
            set => SetProperty(ref _usersSuspicionLevel, value);
        }

        public Partition Partition
        {
            get => _partition;
            set => SetProperty(ref _partition, value);
        }

        #endregion

        #region Methods

        public ResultsViewModel(IEventAggregator eventAggregator, IRegionManager regionManager)
        {

            _eventAggregator = eventAggregator;
            _regionManager = regionManager;

            _eventAggregator.GetEvent<AlgorithmResultsEvent>().Subscribe(arg =>
            {
                UsersSuspicionLevel = arg.UsersSuspicionLevel;
                Partition = arg.Partition;
            });
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
