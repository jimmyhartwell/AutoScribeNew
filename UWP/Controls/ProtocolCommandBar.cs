using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWP.Controls {
    
    public class ProtocolCommandBar : CommandBar {

        private AutoSuggestBox _autoSuggestBox;
        private AppBarSeparator _firstSeparator;
        private AppBarButton _reloadButton;
        private AppBarSeparator _secondSeparator;
        private AppBarButton _editButton;
        private AppBarButton _saveButton;
        private AppBarSeparator _thirdSeparator;
        private AppBarButton _deleteButton;

        public ProtocolCommandBar() {
            
        }


    }
}
