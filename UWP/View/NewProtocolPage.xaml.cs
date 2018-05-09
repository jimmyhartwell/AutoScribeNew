using AutoScribeClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using UWP.AudioControllers;
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
    public sealed partial class NewProtocolPage : BasePage {

        private ObservableCollection<SpeakerViewModel> Speakers => SpeakerListViewModel.GetSpeakerListViewModel().Speakers;
        private Stream Audio { get; set; }
        private bool _recorded;
        private bool _isRecording;
        private ProtocolViewModel viewModel;

        public AudioController AudioController { get; set; }
        public ProtocolViewModel ViewModel {
            get => viewModel;
            set => viewModel = value;
        }

        public NewProtocolPage()
        {
            this.InitializeComponent();
            this.AudioController = new AudioController();
            Recorded = false;
            Save_Command = new RelayCommand(() => Save());
            Cancel_Command = new RelayCommand(() => Cancel());
            SaveAudio_Command = new RelayCommand(() => SaveAudio());
            CancelAudio_Command = new RelayCommand(() => CancelAudio());
            StartRecording_Command = new RelayCommand(async () => await StartRecording());
            StopRecording_Command = new RelayCommand(() => StopRecording());
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel = e.Parameter as ProtocolViewModel;
            Frame.DataContext = this;
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ReleaseAudioStream();
            base.OnNavigatedFrom(e);
        }

        public bool Recorded {
            get => _recorded;
            set {
                _recorded = value;
                OnPropertyChanged();
            }
        }
        public bool IsRecording {
            get => _isRecording;
            set {
                _isRecording = value;
                OnPropertyChanged();
            }
        }
        private async void Save()
        {
            if (String.IsNullOrEmpty(ViewModel.Name)) {
                ShowWarning("Please give this protocol a name.");
                return;
            }
            if (!Recorded) {
                ShowWarning("Please record an audio for this protocol.");
                return;
            }
            if (!CheckSpeakers()) {
                ShowWarning("Please choose speakers of this protocol.");
                return;
            }
            await ProtocolListViewModel.GetProtocolListViewModel().CreateProtocol(ViewModel);
            Frame.GoBack();            
        }
        private void Cancel() {
            Frame.GoBack();
        }
        private void SaveAudio()
        {
            EditGrid.IsPaneOpen = false;
            ViewModel.Audio.Audio = Audio;
            Recorded = true;
        }
        private void CancelAudio()
        {
            ReleaseAudioStream();
            EditGrid.IsPaneOpen = false;
        }
        private void NewRecording(object sender, RoutedEventArgs e)
        {
            EditGrid.IsPaneOpen = true;
        }
        private async Task StartRecording()
        {
            ReleaseAudioStream();
            IsRecording = true;
            await AudioController.StartRecording();
        }
        private void StopRecording()
        {
            IsRecording = false;
            Audio = AudioController.StopRecording();
        }
        private void ReleaseAudioStream() => ViewModel.Audio.Audio?.Dispose();

        private bool CheckSpeakers() {
            if (!(SpeakerList.SelectedItems.Count > 0)) {
                return false;
            } else {
                ViewModel.Speakers.AddRange(SpeakerList.SelectedItems.Select(speaker => (SpeakerViewModel)speaker));
                return true;
            }
        }

        private async void ShowWarning(string message) {
            ContentDialog dialog = new ContentDialog {
                Title = "Warning",
                Content = message,
                CloseButtonText = "OK"
            };
            await dialog.ShowAsync();
        }
    }
}
