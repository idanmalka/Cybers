﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cybers.Infrustructure.models;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using ResultsModule.interfaces;
using ResultsModule.models;

namespace ResultsModule.components
{
    public class ResultsViewModel : BindableBase, IResultsViewModel, INavigationAware
    {
        #region Private Fields

        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;
        private ObservableCollection<UserSuspicion> _usersSuspicionLevel;
        private Partition _partition;

        #endregion

        #region Properties

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

        #endregion

        #region Methods

        public ResultsViewModel(IEventAggregator eventAggregator, IRegionManager regionManager)
        {
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;

            UsersSuspicionLevel = CreateData();

            //_eventAggregator.GetEvent<AlgorithmResultsEvent>().Subscribe(arg =>
            //{
            //    UsersSuspicionLevel = new ObservableCollection<UserSuspicion>(arg.UsersSuspicionLevel.Select(kvp => new UserSuspicion(kvp.Key, kvp.Value)).ToList());
            //    Partition = arg.Partition;
            //});
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
