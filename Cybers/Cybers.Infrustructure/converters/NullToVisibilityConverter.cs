using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Cybers.Infrustructure.converters
{
    public class NullToVisibilityConverter : FrameworkElement, IValueConverter
    {


        public bool IsNullVisible
        {
            get => (bool)GetValue(IsNullVisibleProperty);
            set => SetValue(IsNullVisibleProperty, value);
        }

        // Using a DependencyProperty as the backing store for IsNullVisible.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsNullVisibleProperty =
            DependencyProperty.Register("IsNullVisible", typeof(bool), typeof(NullToVisibilityConverter), new PropertyMetadata(false));



        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (IsNullVisible)
                return value == null ? Visibility.Visible : Visibility.Collapsed;
            return value == null ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
