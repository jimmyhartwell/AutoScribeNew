using AutoScribeClient;
using AutoScribeClient.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Windows.Input;
using UWP.Helpers;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWP.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProtocolPage : BasePage {
        public ProtocolPage()
        {
            Refresh_Command = new RelayCommand(() => GetTranscription());
            Info_Command = new RelayCommand(() => ToggleInfo());
            Edit_Command = new RelayCommand(() => ToggleEditMode());
            Cancel_Command = new RelayCommand(() => Cancel());
            Save_Command = new RelayCommand(() => Save());
            Delete_Command = new RelayCommand(async () => await Delete());
            Search_Command = new RelayCommand(() => Search());
            this.InitializeComponent();
        }

        private string _header;
        private string _speakers;
        private string _keywords;
        private bool _editable;
        private string _searchTerm;

        /// <summary>
        /// Header of main navigation view, assigned with opened protocol's name.
        /// </summary>
        public override string Header {
            set {
                _header = value;
                OnPropertyChanged();
            }
            get => _header;
        }
        public string Speakers {
            get => _speakers;
            set {
                _speakers = value;
                OnPropertyChanged();
            }
        }
        public string Keywords {
            get => _keywords;
            set {
                _keywords = value;
                OnPropertyChanged();
            }
        }
        public bool Editable {
            get => _editable;
            set {
                _editable = value;
                OnPropertyChanged();
            }
        }
        public string SearchTerm {
            get => _searchTerm;
            set => _searchTerm = value;
        }
        private ProtocolViewModel _viewModel;
        public ProtocolViewModel ViewModel {
            get => _viewModel;
            set {
                if (_viewModel != value) {
                    _viewModel = value;
                    Header = StringExtensions.TrimProtocolName(_viewModel.Name);
                    Speakers = StringExtensions.ListSpeakerNames(ViewModel.Speakers);
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            ViewModel = e.Parameter as ProtocolViewModel;
            Frame.DataContext = this;
            GetTranscription();
            ListKeywords();
            base.OnNavigatedTo(e);
        }

        private async void GetTranscription() {
            if (ViewModel == null) {
                // Handle error
                return;
            }
            await ViewModel.GetSections();
            if (ViewModel.Sections != null) {
                TranscriptionContainer.Blocks.Clear();
                foreach (SectionViewModel section in ViewModel.Sections) {
                    TranscriptionContainer.Blocks.Add(ParagraphFromSection(section));
                }
            }
        }

        // TO DO: Write separate function for searching in RichTextBlock.
        private async void Search() {
            TranscriptionContainer.TextHighlighters.Clear();
            TextHighlighter highlighter = new TextHighlighter();
            highlighter.Background = new SolidColorBrush(Colors.DarkOrange);
            TranscriptionContainer.SelectAll();
            string selected = TranscriptionContainer.SelectedText;
            selected = selected.Replace("\n", "");
            selected = selected.Replace("\r", "");
            if (String.IsNullOrWhiteSpace(SearchTerm))
                return;
            if (!selected.Contains(SearchTerm)) {
                ContentDialog dialog = new ContentDialog();
                dialog.Title = "Search Result";
                dialog.Content = "No result found.";
                dialog.CloseButtonText = "OK";
                await dialog.ShowAsync();
            }
            List<int> indices = new List<int>();
            int i = 0;
            while (i < selected.Length) {
                int j = selected.IndexOf(SearchTerm, i, StringComparison.InvariantCulture);
                if (j >= 0) {
                    highlighter.Ranges.Add(new TextRange() { StartIndex = j, Length = SearchTerm.Length });
                    i = j + SearchTerm.Length;
                } else {
                    break;
                }
            }
            TranscriptionContainer.TextHighlighters.Add(highlighter);
        }

        private Paragraph ParagraphFromSection(SectionViewModel section) {
            Paragraph para = new Paragraph {
                Margin = new Thickness(0, 0, 0, 10)
            };

            // Speaker
            Run speaker = new Run() {
                FontWeight = FontWeights.SemiBold
            };
            if (section.Speaker == null) {
                speaker.Text = "Unknown";
            } else {
                speaker.Text = section.Speaker.Name;
            }
            para.Inlines.Add(speaker);
            para.Inlines.Add(new LineBreak());

            // Transcript
            Run transcript = new Run() {
                Text = section.Text
            };
            para.Inlines.Add(transcript);
            return para;
        }

        private void ToggleInfo() => ProtocolView.IsPaneOpen = !ProtocolView.IsPaneOpen;
        
        private void ListKeywords()
        {
            ViewModel.GetKeywords();
            List<Border> borders = StringExtensions.ListKeywords(ViewModel.Keywords);
            borders.ForEach(border => KeywordContainer.Children.Add(border));
        }

        public void ToggleEditMode() => Editable = true;
        
        private void Cancel() {
            Editable = false;
            GetTranscription();
        }

        private async void Save() {
            await ViewModel.Save();
            Editable = false;
            GetTranscription();
        }

        private async Task Delete() {
            ContentDialog deleteDialog = new ContentDialog {
                Title = "Delete Protocol",
                Content = ViewModel.Name + " will be deleted.",
                PrimaryButtonText = "Delete",
                SecondaryButtonText = "Cancel"
            };
            if (await deleteDialog.ShowAsync() == ContentDialogResult.Primary) {
                ProtocolListViewModel.GetProtocolListViewModel().DeleteProtocol(ViewModel);
                Frame.GoBack();
            } else {
                deleteDialog.Hide();
            }
        }

        private async void LoadSectionAudio(object sender, RoutedEventArgs e)
        {
            SectionViewModel section = ((MediaPlayerElement)sender).DataContext as SectionViewModel;
            await section.GetAudio();
            ((MediaPlayerElement)sender).MediaPlayer.Source = MediaSource.CreateFromStream(section.Audio.Audio.AsRandomAccessStream(), ".wav");
        }

        private List<Border> SplitRowBorders(List<Border> borders, double rowWidth) {
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

        private async void MediaPlayerElement_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (((MediaPlayerElement)sender).MediaPlayer.Source == null) {
                SectionViewModel section = ((MediaPlayerElement)sender).DataContext as SectionViewModel;
                await section.GetAudio();
                ((MediaPlayerElement)sender).MediaPlayer.Source = MediaSource.CreateFromStream(section.Audio.Audio.AsRandomAccessStream(), ".wav");
            }            
        }
    }
}
