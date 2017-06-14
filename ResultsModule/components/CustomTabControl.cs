using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ResultsModule.components
{
    public class CustomTabControl : TabControl
    {
        protected override void OnRenderSizeChanged(System.Windows.SizeChangedInfo sizeInfo)
        {
            foreach (TabItem item in Items)
            {
                var newW = ActualWidth / Items.Count - 1;
                if (newW < 0) newW = 0;

                item.Width = newW*0.998;
            }
        }
    }
}
