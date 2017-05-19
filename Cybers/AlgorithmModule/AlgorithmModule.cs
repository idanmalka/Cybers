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
    public class AlgorithmModule: ModuleBase
    {
        public AlgorithmModule(IUnityContainer container, IRegionManager regionManager) : base(container, regionManager)
        {
        }

        protected override void InitializeModule()
        {
            //RegionManager.RegisterViewWithRegion(RegionNames.BottomToolbarRegion, typeof(AlgorithmBottomToolbarView));
        }

        protected override void RegisterTypes()
        {
            Container.RegisterType<IAlgorithmToolbarView>();
            Container.RegisterType<IAlgorithmViewModel, AlgorithmViewModel>();
            Container.RegisterType<IAlgorithmView, AlgorithmLoadingView>();
            Container.RegisterTypeForNavigation<AlgorithmLoadingView>();
        }
    }
}
