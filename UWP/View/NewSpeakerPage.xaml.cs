using AutoScribeClient.ViewModels;
using System;
using System.Collections.Generic;
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
    public sealed partial class NewSpeakerPage : BasePage {

        public NewSpeakerPage()
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

        private Stream audio;
        
        private bool _recorded;
        private bool _isRecording;
        private SpeakerViewModel viewModel;

        public AudioController AudioController { get; set; }
        private Stream Audio {
            get => audio;
            set => audio = value;
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel = e.Parameter as SpeakerViewModel;
            Frame.DataContext = this;
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ReleaseAudioStream();
            base.OnNavigatedFrom(e);
        }
        
        public SpeakerViewModel ViewModel {
            get => viewModel;
            set => viewModel = value;
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
                ShowWarning("Please give this speaker a name.");
                return;
            }
            if (!Recorded) {
                ShowWarning("Please record an enrollment audio for this speaker.");
                return;
            }
            await SpeakerListViewModel.GetSpeakerListViewModel().CreateSpeaker(ViewModel);
            Frame.GoBack();            
        }
        private void Cancel()
        {
            Frame.GoBack();
        }
        private void SaveAudio()
        {
            EditGrid.IsPaneOpen = false;
            ViewModel.EnrollmentAudio.Audio = Audio;
            Recorded = true;
        }
        private void CancelAudio()
        {
            EditGrid.IsPaneOpen = false;
            ReleaseAudioStream();
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
        private void ReleaseAudioStream() => ViewModel.EnrollmentAudio.Audio?.Dispose();
        private async void ShowWarning(string message)
        {
            ContentDialog dialog = new ContentDialog {
                Title = "Warning",
                Content = message,
                CloseButtonText = "OK"
            };
            await dialog.ShowAsync();
        }
    }
}
