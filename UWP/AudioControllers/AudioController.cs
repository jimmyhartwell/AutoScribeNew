using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Playback;
using AutoScribeClient.Business;
using System.IO;
using Windows.Media.Core;
using Windows.Storage;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using Windows.Media.Devices;
using System.Timers;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace UWP.AudioControllers
{
    public class AudioController : IAudioController, INotifyPropertyChanged
    {
        public AudioController() {
            Timer = new DispatcherTimer() {
                Interval = new TimeSpan(0, 0, 1)
            };
            Timer.Tick += (s, e) => AudioLength = AudioLength.Add(new TimeSpan(0, 0, 1));
            AudioLength = new TimeSpan(0, 0, 0);
        }
        public MediaPlayer mediaPlayer { get; private set; }
        private TimeSpan audioLength;
        private string audioLengthString;
        private DispatcherTimer Timer;
        private bool isRecording;
        /// <summary>
        /// Raised event when mediaPlayer.MediaEnded event is raised.
        /// </summary>
        public event EventHandler MediaEnded;
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Continue the audio playback.
        /// </summary>
        public void ContinueAudio()
        {
            mediaPlayer.Play();
        }

        /// <summary>
        /// Pause the audio playback.
        /// </summary>
        public void PauseAudio()
        {
            mediaPlayer.Pause();
        }

        /// <summary>
        /// Play the specified audio
        /// </summary>
        /// <param name="audio">The audio</param>
        public void PlayAudio(Stream audio)
        {
            if (mediaPlayer != null) {
                StopAudio();
            }
            mediaPlayer = new MediaPlayer();
            /// <see cref="MediaEnded"/> event handler raises when media playing is ended.
            mediaPlayer.MediaEnded += (sender, args) => MediaEnded?.Invoke(sender, System.EventArgs.Empty);
            try {
                mediaPlayer.Source = MediaSource.CreateFromStream(audio.AsRandomAccessStream(), ".wav");
            } catch {
                throw new ArgumentException("The audio file is not in the correct format.");
            }
            mediaPlayer.Play();
        }

        /// <summary>
        /// Stop the audio playback.
        /// </summary>
        public void StopAudio()
        {
            mediaPlayer.Dispose();
            mediaPlayer = null;
        }

        private StorageFile file;
        private string defaultStoragePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AutoScribe");
        public MediaCapture mediaCapture { get; private set; }
        public TimeSpan AudioLength {
            get => audioLength;
            set {
                audioLength = value;
                AudioLengthString = audioLength.ToString();
            }
        }

        public string AudioLengthString {
            get => audioLengthString;
            set {
                audioLengthString = value;
                OnPropertyChanged();
            }
        }

        public bool IsRecording {
            get => isRecording;
            set {
                isRecording = value;
                OnPropertyChanged();
            }
        }

        private const int BITS_PER_SAMPLE = 16;
        private const int CHANNEL_COUNT = 1;
        private const int SAMPLE_RATE = 16000;

        /// <summary>
        /// Initialize the MediaCapture object.
        /// </summary>
        private async Task InitMediaCapture()
        {
            mediaCapture = new MediaCapture();
            await mediaCapture.InitializeAsync(new MediaCaptureInitializationSettings() {
                StreamingCaptureMode = StreamingCaptureMode.Audio
            });
        }

        /// <summary>
        /// Start recording a new audio.
        /// </summary>
        public async Task StartRecording()
        {
            AudioLength = new TimeSpan(0, 0, 0);
            IsRecording = true;
            if (mediaCapture == null) {
                await InitMediaCapture();
            }

            var folder = ApplicationData.Current.LocalFolder;
            file = await folder.CreateFileAsync("recording1.wav", CreationCollisionOption.ReplaceExisting);

            MediaEncodingProfile encodingProfile = MediaEncodingProfile.CreateWav(AudioEncodingQuality.Low);

            await mediaCapture.StartRecordToStorageFileAsync(encodingProfile, file);
            Timer.Start();
            
        }

        /// <summary>
        /// Pause the recording.
        /// </summary>
        public async void PauseRecording()
        {
            await mediaCapture.PauseRecordAsync(MediaCapturePauseBehavior.RetainHardwareResources);
        }

        /// <summary>
        /// Continue the recording.
        /// </summary>
        public async void ContinueRecording()
        {
            await mediaCapture.ResumeRecordAsync();
        }

        /// <summary>
        /// Stop the recording and get the created audio.
        /// </summary>
        /// <returns>The recorded audio</returns>
        public Stream StopRecording()
        {
            Timer.Stop();
            IsRecording = false;
            mediaCapture.StopRecordAsync().GetAwaiter().GetResult();
            return File.OpenRead(file.Path);
        }

        public void RegisterMediaEndedHandler(EventHandler handler)
        {
            //
        }
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
        
            changed?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
