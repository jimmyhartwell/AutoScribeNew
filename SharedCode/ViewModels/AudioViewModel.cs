using System;
using System.IO;
using System.Windows.Input;
using System.Timers;
using System.Threading.Tasks;
using AutoScribeClient.Business;

namespace AutoScribeClient.ViewModels
{
    /// <summary>
    /// ViewModel for RecordingPage and where audio elements show up.
    /// </summary>
    public class AudioViewModel : BaseViewModel
    {
        /// <summary>
        /// Recorded/loaded audio stream.
        /// </summary>
        private Stream audio;

        /// <summary>
        /// Temporary audio stream, used when recording new audio while an audio stream already exists.
        /// </summary>
        private Stream tempAudio;

        /// <summary>
        /// Indicator of whether the audio is being recorded.
        /// </summary>
        private bool isRecording;

        /// <summary>
        /// Indicator of whether the recording process is being paused.
        /// </summary>
        private bool isPausing;

        /// <summary>
        /// Indicator of whether the audio is being played.
        /// </summary>
        private bool isPlaying;

        /// <summary>
        /// Length of the currently recorded audio.
        /// </summary>
        private TimeSpan audioLength;

        /// <summary>
        /// String form of audio length.
        /// </summary>
        private string audioLengthString;

        /// <summary>
        /// Timer used to count the length of this audio while recording.
        /// </summary>
        private Timer Timer;

        /// <summary>
        /// Text to display on the pause-button while recording, is "Pause Recording" by default, otherwise "Continue Recording".
        /// </summary>
        private string pauseButtonText;

        /// <summary>
        /// Text to display on the play-button while playing, is "Play" by default, otherwise "Pause".
        /// </summary>
        private string playButtonText;

        /// <summary>
        /// Indicator of whether this audio is recorded.
        /// </summary>
        private bool audioRecorded;

        /// <summary>
        /// Error to show to the user when there is one.
        /// </summary>
        private string error;

        /// <summary>
        /// Audio controller for recording and playing processes.
        /// </summary>
        private IAudioController audioController;

        /// <summary>
        /// Command to play this audio.
        /// </summary>
        public ICommand PlayAudioCommand { get; }

        /// <summary>
        /// Command to pause playing this audio.
        /// </summary>
        public ICommand PauseAudioCommand { get; }

        /// <summary>
        /// Command to resume playing this audio.
        /// </summary>
        public ICommand ContinueAudioCommand { get; }

        /// <summary>
        /// Command to stop playing this audio.
        /// </summary>
        public ICommand StopAudioCommand { get; }

        /// <summary>
        /// Command to start recording.
        /// </summary>
        public ICommand StartRecordingCommand { get; }

        /// <summary>
        /// Command to pause or continue recording this audio.
        /// </summary>
        public ICommand PauseRecordingCommand { get; }

        /// <summary>
        /// Command to stop recording this audio.
        /// </summary>
        public ICommand StopRecordingCommand { get; }

        /// <summary>
        /// Create a new ViewModel to record and control audio.
        /// </summary>
        public AudioViewModel() {
            //Error = "";
            //AudioRecorded = false;
            //IsRecording = IsPausing = false;
            //IsPlaying = false;
            //AudioLength = new TimeSpan(0, 0, 0);
            //StartRecordingCommand = new Command(StartRecording);
            //StopRecordingCommand = new Command(StopRecording);
            //PauseRecordingCommand = new Command(PauseRecording);
            //PlayAudioCommand = new Command(PlayAudio);
            //PauseAudioCommand = new Command(PauseAudio);
            //ContinueAudioCommand = new Command(ContinueAudio);
            //StopAudioCommand = new Command(StopAudio);
            //PauseButtonText = "Pause";
            //PlayButtonText = "Play";
            //audioController = new AudioController();
            //Timer = new Timer(1000);
            //Timer.Elapsed += UpdateAudioLength;
            //audioController.RegisterMediaEndedHandler(new EventHandler((sender, e) => IsPlaying = false));
        }

        /// <summary>
        /// Indicator of whether the audio is being recorded.
        /// </summary>
        public bool IsRecording {
            get => isRecording;
            set {
                if (isRecording != value) {
                    isRecording = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Length of the currently recorded audio.
        /// </summary>
        public TimeSpan AudioLength {
            get => audioLength;
            set {
                if (audioLength != value) {
                    audioLength = value;
                    AudioLengthString = value.ToString();
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// String form of audio length.
        /// </summary>
        public string AudioLengthString {
            get => AudioLength.ToString();
            set {
                if (audioLengthString != value) {
                    audioLengthString = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Text to display on the pause-button while recording, is "Pause Recording" by default, otherwise "Continue Recording".
        /// </summary>
        public string PauseButtonText {
            get => pauseButtonText;
            set {
                if (pauseButtonText != value) {
                    pauseButtonText = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Recorded/loaded audio stream.
        /// </summary>
        public Stream Audio {
            get => audio;
            set {
                if (value != null) {
                    audio = value;
                    AudioRecorded = true;
                } else {
                    AudioRecorded = false;
                }
            }
        }

        /// <summary>
        /// Indicator of whether the audio is being played.
        /// </summary>
        public bool IsPausing { get => isPausing;
            set {
                if (isPausing != value) {
                    isPausing = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Error to show to the user when there is one.
        /// </summary>
        public string Error { get => error;
            set {
                if (error != value) {
                    error = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Indicator of whether the audio is being played.
        /// </summary>
        public bool IsPlaying { get => isPlaying;
            set {
                if (isPlaying != value) {
                    isPlaying = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Text to display on the play-button while playing, is "Play" by default, otherwise "Pause".
        /// </summary>
        public string PlayButtonText {
            get => playButtonText;
            set {
                if (playButtonText != value) {
                    playButtonText = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Indicator of whether this audio is recorded.
        /// </summary>
        public bool AudioRecorded {
            get => audioRecorded;
            set {
                if (audioRecorded != value) {
                    audioRecorded = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Temporary audio stream, used when recording new audio while an audio stream already exists.
        /// </summary>
        public Stream TempAudio { get => tempAudio; set => tempAudio = value; }

        /// <summary>
        /// Play this audio.
        /// </summary>
        private void PlayAudio()
        {
            try {
                Error = "";
                audioController.PlayAudio(Audio);
                IsPlaying = true;
            } catch {
                Error = "Error occurs while playing audio, please try again.";
            }
        }

        /// <summary>
        /// Pause if this audio is being played.
        /// </summary>
        private void PauseAudio()
        {
            try {
                Error = "";
                audioController.PauseAudio();
                IsPausing = false;
            } catch {
                Error = "Error occurs while pausing audio, plese try again.";
            }
        }

        /// <summary>
        /// Continue playing if this audio is being paused.
        /// </summary>
        private void ContinueAudio()
        {
            try {
                Error = "";
                audioController.ContinueAudio();
                IsPausing = false;
            } catch {
                Error = "Error occurs while continue playing audio, please try again.";
            }
        }

        /// <summary>
        /// Stop playing if this audio is being played.
        /// </summary>
        private void StopAudio()
        {
            try {
                Error = "";
                audioController.StopAudio();
                IsPlaying = false;
                IsPausing = false;
            } catch {
                Error = "Error occurs while continue playing audio, please try again.";
            }
        }

        /// <summary>
        /// Start recording audio.
        /// </summary>
        private async void StartRecording()
        {
            try {
                Error = "";
                Audio?.Dispose();
                await audioController.StartRecording();
                IsRecording = true;
                AudioLength = new TimeSpan(0, 0, 0);
                await Task.Run(() => {
                    Timer.Start();
                });
            } catch {
                Error = "Error occurs while recording audio, please try again.";
            }
        }

        /// <summary>
        /// Pause recording this audio when recording, continue recording when pausing.
        /// </summary>
        private void PauseRecording()
        {
            if (!IsPausing) {
                try {
                    Error = "";
                    audioController.PauseRecording();
                    IsPausing = true;
                    PauseButtonText = "Continue";
                    Task.Run(() => {
                        Timer.Stop();
                    });
                } catch {
                    Error = "Error occurs while pausing recording, please try again.";
                }
            } else {
                try {
                    Error = "";
                    audioController.ContinueRecording();
                    IsPausing = false;
                    PauseButtonText = "Pause";
                    Task.Run(() => {
                        Timer.Start();
                    });
                } catch {
                    Error = "Error occurs while continuing recording, please try again.";
                }
            }
        }

        /// <summary>
        /// Stop recording if this audio is being recorded.
        /// </summary>
        private void StopRecording()
        {
            try {
                Error = "";
                Audio = audioController.StopRecording();
                IsRecording = IsPausing = false;
                PauseButtonText = "Pause recording";
                Task.Run(() => {
                    Timer.Stop();
                });
            } catch {
                Error = "Error occurs while stopping recording, please try again.";
            }
        }

        /// <summary>
        /// Update audio length every second when recording.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event data.</param>
        private void UpdateAudioLength(object sender, ElapsedEventArgs e) {
            AudioLength = AudioLength.Add(new TimeSpan(0, 0, 1));
        }
    }
}