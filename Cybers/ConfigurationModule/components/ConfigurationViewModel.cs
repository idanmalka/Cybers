using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigurationModule.interfaces;
using Cybers.Infrustructure.interfaces;

namespace ConfigurationModule.components
{
    
    public class ConfigurationViewModel: IConfigurationViewModel
    {
        public string MainContentTitle { get; set; } = "Configuration Main Content Title";
        public string BottomToolbarTitle { get; set; } = "Configuration Bottom Toolbar Title";
    }
}
