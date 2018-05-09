using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UWP.View;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace UWP.Resources
{
    public class NavigationViewHeaderTemplateSelector : DataTemplateSelector
    {
        public DataTemplate ProtocolListPageNavigationViewHeader { get; set; }
        public DataTemplate ProtocolPageNavigationViewHeader { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container) {
            var parent = container;

            while (parent != null && !(parent is Page)) {
                parent = VisualTreeHelper.GetParent(parent);
            }

            if (parent is ProtocolListPage) {
                return ProtocolListPageNavigationViewHeader;
            }
            if (parent is ProtocolPage) {
                return ProtocolPageNavigationViewHeader;
            }

            //throw new ArgumentException("DataTemplate for given page is not provided.");
            return ProtocolListPageNavigationViewHeader;
        }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            return base.SelectTemplateCore(item);
        }
    }
}
