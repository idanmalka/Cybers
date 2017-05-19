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
using AlgorithmModule.interfaces;
using Cybers.Infrustructure.interfaces;

namespace AlgorithmModule.components
{
    /// <summary>
    /// Interaction logic for AlgorithmLoadingView.xaml
    /// </summary>
    public partial class AlgorithmLoadingView : UserControl, IAlgorithmView
    {
        public IViewModel ViewModel
        {
            get => DataContext as IAlgorithmViewModel;
            set => DataContext = value;
        }

        public AlgorithmLoadingView(IAlgorithmViewModel viewModel)
        {
            ViewModel = viewModel;

            InitializeComponent();
        }

    }
}
