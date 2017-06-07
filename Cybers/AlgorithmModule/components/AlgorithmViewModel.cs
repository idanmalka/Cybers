﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlgorithmModule.interfaces;
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

        #endregion

        #region Properties

        public DelegateCommand TestCommand { get; }

        public AlgorithmStep AlgStep
        {
            get => _algStep;
            set => SetProperty(ref _algStep, value);
        }

        public List<UserAttribute> ClusteringAttributes { get; set; }
        public List<UserAttribute> DistributingAttributes { get; set; }
        public string GraphFilePath { get; set; }
        public int DistributingThreshold { get; set; }

        #endregion

        #region Methods

        public AlgorithmViewModel(IEventAggregator eventAggregator)
        {
            TestCommand = new DelegateCommand(Test);
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<AlgorithmAttributesEvent>().Subscribe(obj =>
            {
                ClusteringAttributes = obj.ClustringAttributes;
                DistributingAttributes = obj.DistributingAttributes;
                GraphFilePath = obj.GraphFilePath;
                DistributingThreshold = obj.Threshold;
            });

            //get users from file here
            //
            //
            //


            //Task.Run(() => StartAlgorithm());
        }

        private void StartAlgorithm()
        {
            var clusteringAttributes = ClusteringAttributes.Select(a => a.Value);
            var distributingAttributes = DistributingAttributes.Select(a => a.Value);

            var algorithm = new CybersDetection(_users, clusteringAttributes, distributingAttributes, DistributingThreshold);

            algorithm.InitializationFinished += OnAlgorithmInitializationFinished;
            algorithm.ClusteringFinished += OnClusteringFinished;
            algorithm.DistributingFinished += OnDistributingFinished;

            algorithm.Execute();

            _results = algorithm.LatestRunResults;
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
