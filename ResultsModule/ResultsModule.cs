using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cybers.Infrustructure;
using Microsoft.Practices.Unity;
using Prism.Regions;
using ResultsModule.components;
using ResultsModule.interfaces;

namespace ResultsModule
{
    public class ResultsModule: ModuleBase
    {
        public ResultsModule(IUnityContainer container, IRegionManager regionManager) : base(container, regionManager)
        {
        }

        protected override void InitializeModule()
        {

        }

        protected override void RegisterTypes()
        {
            Container.RegisterType<IResultsView, ResultsView>();
            Container.RegisterType<IResultsViewModel, ResultsViewModel>();
            Container.RegisterTypeForNavigation<ResultsViewModel>();
        }
    }
}
