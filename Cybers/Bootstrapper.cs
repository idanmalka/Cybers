using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Cybers.Infrustructure.controls;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Unity;

namespace Cybers
{
    public class Bootstrapper: UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            Container.RegisterType<Shell>();
            Container.RegisterType<ServicesModule.ServicesModule>();
            Container.RegisterType<WelcomeModule.WelcomeModule>();
            Container.RegisterType<ConfigurationModule.ConfigurationModule>();
            Container.RegisterType<AlgorithmModule.AlgorithmModule>();
            Container.RegisterType<ResultsModule.ResultsModule>();
            return Container.Resolve<Shell>();
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();
            Application.Current.MainWindow = Shell as Window;
            Application.Current.MainWindow?.Show();
        }

        protected override void ConfigureModuleCatalog()
        {
            base.ConfigureModuleCatalog();
            AddModuleToCatalog(typeof(ServicesModule.ServicesModule), InitializationMode.WhenAvailable);
            AddModuleToCatalog(typeof(WelcomeModule.WelcomeModule), InitializationMode.WhenAvailable);
            AddModuleToCatalog(typeof(ConfigurationModule.ConfigurationModule), InitializationMode.WhenAvailable);
            AddModuleToCatalog(typeof(AlgorithmModule.AlgorithmModule), InitializationMode.WhenAvailable);
            AddModuleToCatalog(typeof(ResultsModule.ResultsModule),InitializationMode.WhenAvailable);
        }

        private void AddModuleToCatalog(Type type, InitializationMode mode)
        {
            ModuleCatalog.AddModule(new ModuleInfo()
            {
                ModuleName = type.Name,
                ModuleType = type.AssemblyQualifiedName,
                InitializationMode = mode
            });
        }
    }
}
