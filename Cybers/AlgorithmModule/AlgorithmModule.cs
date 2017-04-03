using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlgorithmModule.components;
using AlgorithmModule.interfaces;
using Cybers.Infrustructure;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;

namespace AlgorithmModule
{
    public class AlgorithmModule: IModule
    {
        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;

        public AlgorithmModule(IUnityContainer container, IRegionManager regionManager)
        {
            _container = container;
            _regionManager = regionManager;
        }

        public void Initialize()
        {
            _container.RegisterType<IAlgorithmToolbarView,IAlgorithmToolbarView>();
            _container.RegisterType<IAlgorithmViewModel,AlgorithmViewModel>();
            _container.RegisterType<IAlgorithmView,AlgorithmLoadingView>();

            _regionManager.RegisterViewWithRegion(RegionNames.MainContentRegion, typeof(AlgorithmLoadingView));
            _regionManager.RegisterViewWithRegion(RegionNames.BottomToolbarReegion, typeof(AlgorithmBottomToolbarView));
        }
    }
}
