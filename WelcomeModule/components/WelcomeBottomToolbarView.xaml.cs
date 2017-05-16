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
using WelcomeModule.interfaces;

namespace WelcomeModule.components
{
    /// <summary>
    /// Interaction logic for WelcomeBottomToolbarView.xaml
    /// </summary>
    public partial class WelcomeBottomToolbarView : UserControl, IWelcomeToolbarView
    {
        public IViewModel ViewModel
        {
            get => DataContext as IWelcomeViewModel;
            set => DataContext = value;
        }

        public WelcomeBottomToolbarView(IWelcomeViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
        }
    }
}
