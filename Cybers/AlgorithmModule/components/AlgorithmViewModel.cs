using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlgorithmModule.interfaces;
using Cybers.Infrustructure.interfaces;

namespace AlgorithmModule.components
{
    public class AlgorithmViewModel: IAlgorithmViewModel
    {
        public string MainContentTitle { get; set; } = "Algorithm Main Content Title";
        public string BottomToolbarTittle { get; set; } = "Algorithm Bottom Toolbar Title";
    }
}
