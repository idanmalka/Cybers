using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Cybers.Infrustructure.interfaces;
using ResultsModule.interfaces;

namespace ResultsModule.components
{
    /// <summary>
    /// Interaction logic for ResultsView.xaml
    /// </summary>
    public partial class ResultsView : UserControl, IResultsView
    {
        public ResultsView(IResultsViewModel viewModel)
        {
            ViewModel = viewModel;

            InitializeComponent();
            BarChart.DataContext = viewModel;
        }

        public IViewModel ViewModel
        {
            get => DataContext as IResultsViewModel;
            set => DataContext = value;
        }
    }
}
