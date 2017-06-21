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
    public class NullToBooleanConverter : FrameworkElement, IValueConverter
    {
     


            public bool IsNullTrue
            {
                get => (bool)GetValue(IsNullTrueProperty);
                set => SetValue(IsNullTrueProperty, value);
            }

            // Using a DependencyProperty as the backing store for IsNullTrue. 
            public static readonly DependencyProperty IsNullTrueProperty =
                DependencyProperty.Register("IsNullTrue", typeof(bool), typeof(NullToBooleanConverter), new PropertyMetadata(false));



            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (IsNullTrue)
                    return value == null;
                return value != null;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
    
}
