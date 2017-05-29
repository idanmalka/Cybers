using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlgorithmModule.interfaces;
using Cybers.Infrustructure.interfaces;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace AlgorithmModule.components
{
    public class AlgorithmViewModel : BindableBase, IAlgorithmViewModel, INavigationAware
    {
        private AlgorithmStep _algStep = AlgorithmStep.Init;

        public enum AlgorithmStep
        {
            Init, Clustering, Distributing, Finish, End
        }

        public DelegateCommand TestCommand { get; }

        public AlgorithmStep AlgStep
        {
            get => _algStep;
            set => SetProperty(ref _algStep, value);
        }

        public AlgorithmViewModel()
        {
            TestCommand = new DelegateCommand(Test);
        }

        private void Test()
        {
            AlgStep = AlgorithmStep.Clustering;
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
    }
}
