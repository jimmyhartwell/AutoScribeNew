using AutoScribeClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace UWP.Helpers
{
    public class StringExtensions
    {
        public static string TrimProtocolName(string name) {
            if (name.Count() > 18) {
                name = String.Concat(name.Remove(15), "...");
            }
            return name;
        }

        public static string ListSpeakerNames(List<SpeakerViewModel> speakers) {
            if (speakers != null) {
                StringBuilder builder = new StringBuilder("");
                foreach (SpeakerViewModel speaker in speakers) {
                    builder.Append(speaker.Name);
                    builder.Append("\n");
                }
                builder.Remove(builder.Length - 1, 1);
                return builder.ToString();
            } else {
                throw new ArgumentNullException("Speaker list must be set.");
            }
        }

        public static List<Border> ListKeywords(List<string> keywords)
        {
            if (keywords != null && keywords.Count > 0) {
                List<Border> borders = new List<Border>();
                foreach (string keyword in keywords) {
                    TextBlock text = new TextBlock() {
                        Text = keyword,
                        Foreground = new SolidColorBrush(Colors.Black)
                    };
                    Border border = new Border() {
                        Background = new SolidColorBrush(Colors.DarkOrange),
                        CornerRadius = new Windows.UI.Xaml.CornerRadius(5),
                        Padding = new Windows.UI.Xaml.Thickness(7, 1, 7, 3),
                        Margin = new Windows.UI.Xaml.Thickness(2)
                    };
                    border.Child = text;
                    borders.Add(border);
                }
                return borders;
            } else {
                return new List<Border>();
            }
        }
        
    }
}
