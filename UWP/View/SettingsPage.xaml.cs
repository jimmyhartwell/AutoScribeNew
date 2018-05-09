using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UWP.Helpers;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWP.View {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page {
        public SettingsPage()
        {
            this.InitializeComponent();
            ListKeywords();
        }

        private void ListKeywords()
        {
            List<string> keywords = new List<string>() {
                "Lionel Messi", "Barcelona", "New York", "America", "California", "Youtube", "AudoScribe", "Tokyo", "Deutschland",
            };
            List<Border> borders = StringExtensions.ListKeywords(keywords);
            borders.ForEach(border => KeywordContainer.Children.Add(border));
        }
        private List<Border> SplitRowBorders(List<Border> borders, double rowWidth)
        {
            double widthSum = 0;
            List<Border> rowBorders = new List<Border>();
            foreach (Border border in borders) {
                widthSum += border.ActualWidth;
                if (widthSum > rowWidth) {
                    break;
                } else {
                    rowBorders.Add(border);
                }
            }
            rowBorders.ForEach(border => borders.Remove(border));
            return rowBorders;
        }
    }
}
