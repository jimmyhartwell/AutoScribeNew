using AutoScribeClient.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Windows.Input;
using UWP.AudioControllers;
using UWP.Helpers;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWP.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SpeakerPage : BasePage
    {
        private string _header;
        private bool _editable;
        private bool _recorded;
        private bool _isRecording;
        public AudioController AudioController { get; set; }
        public SpeakerPage()
        {
            this.InitializeComponent();
            Edit_Command = new RelayCommand(() => ToggleEditMode());
            Cancel_Command = new RelayCommand(() => Cancel());
            Save_Command = new RelayCommand(() => Save());
            Delete_Command = new RelayCommand(() => Delete());
            StartRecording_Command = new RelayCommand(async () => await StartRecording());
            StopRecording_Command = new RelayCommand(() => StopRecording());
            SaveAudio_Command = new RelayCommand(() => SaveAudio());
            CancelAudio_Command = new RelayCommand(() => CancelAudio());
            AudioController = new AudioController();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            ViewModel = e.Parameter as SpeakerViewModel;
            Frame.DataContext = this;
            GetEnrollmentAudioSource();
            Recorded = true;
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ReleaseAudioStream();
            base.OnNavigatedFrom(e);
        }

        private SpeakerViewModel _viewModel;
        public SpeakerViewModel ViewModel {
            get => _viewModel;
            set {
                if (_viewModel != value) {
                    _viewModel = value;
                    Header = _viewModel.Name;
                }
            }
        }
        public bool Editable {
            get => _editable;
            set {
                _editable = value;
                OnPropertyChanged();
            }
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

        public override string Header {
            get => _header;
            set {
                _header = value;
                OnPropertyChanged();
            }
        }

        private async void GetEnrollmentAudioSource() {
            await ViewModel.GetAudio();
            try {
                EnrollmentAudioPlayer.MediaPlayer.Source = MediaSource.CreateFromStream(ViewModel.EnrollmentAudio.Audio.AsRandomAccessStream(), ".wav");
            } catch (Exception e) {
                string err = e.ToString();
            }
            
        }

        public void ToggleEditMode() => Editable = true;

        private async void Save() {
            Editable = false;
            await ViewModel.Save();
            GetEnrollmentAudioSource();
            ProtocolListViewModel.GetProtocolListViewModel().AddViewModels();
        }

        private async void Cancel() {
            Editable = false;
            ReleaseAudioStream();
            await ViewModel.Refresh();
            GetEnrollmentAudioSource();
        }
        private void SaveAudio() {
            EditGrid.IsPaneOpen = false;
            ReleaseAudioStream();
        }
        private async void CancelAudio() {
            EditGrid.IsPaneOpen = false;
            ReleaseAudioStream();
            await ViewModel.GetAudio();
        }
        private async void Delete() {
            await SpeakerListViewModel.GetSpeakerListViewModel().DeleteSpeaker(ViewModel);
            Frame.GoBack();
        }
        private void NewRecording(object sender, RoutedEventArgs e) {
            EditGrid.IsPaneOpen = true;
        }
        private async Task StartRecording() {
            IsRecording = true;
            await AudioController.StartRecording();
        }
        private void StopRecording() {
            IsRecording = false;
            ViewModel.EnrollmentAudio.Audio = AudioController.StopRecording();
            ReleaseAudioStream();
        }
        private void ReleaseAudioStream() => ViewModel.EnrollmentAudio.Audio?.Dispose();
    }
}
