﻿using System;
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
using ConfigurationModule.interfaces;
using Cybers.Infrustructure.interfaces;

namespace ConfigurationModule.components
{
    /// <summary>
    /// Interaction logic for ConfigurationBottomToolbarView.xaml
    /// </summary>
    public partial class ConfigurationBottomToolbarView : UserControl, IConfigurationToolbarView
    {


        public ConfigurationBottomToolbarView()
        {
            InitializeComponent();
        }

        public IViewModel ViewModel { get; set; }
    }
}
