using AutoScribeDataModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AutoScribeClient.ViewModels
{
    /// <summary>
    /// This ViewModel is used for displaying and editing sections in a protocol.
    /// </summary>
    public class SectionViewModel : BaseViewModel
    {
        private ProtocolSection Data;        
        private SpeakerViewModel speaker;
        private AudioViewModel audio;
        private string error;

        /// <summary>
        /// Create a new ViewModel using data object retrieved from server.
        /// </summary>
        /// <param name="data">Retrieved data.</param>
        public SectionViewModel(ProtocolSection data) {
            Error = "";
            Data = data;
            Audio = new AudioViewModel();
        }

        /// <summary>
        /// Transcript of this section.
        /// </summary>
        public string Text { get => Data.Text;
            set {
                if (Data.Text != value) {
                    Data.Text = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Speaker who has spoken this section of the protocol.
        /// </summary>
        public SpeakerViewModel Speaker { get => speaker;
            set {
                if (speaker != value) {
                    speaker = value;
                    Data.SpeakerId = speaker.Data.Id;
                    OnPropertyChanged();
                }
            }
        }

        public string SpeakerId => Data.SpeakerId;
        /// <summary>
        /// This ViewModel contains information about the audio file of this section.
        /// </summary>
        public string AudioId => Data.AudioId;

        /// <summary>
        /// This ViewModel contains information about the audio file of this section.
        /// </summary>
        public AudioViewModel Audio { get => audio; set => audio = value; }        

        /// <summary>
        /// Error to show to the user when there is one.
        /// </summary>
        public string Error {
            get => error;
            set {
                if (error != value) {
                    error = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Load audio of this section.
        /// </summary>
        /// <returns></returns>
        public async Task GetAudio() {
            try {
                Audio.Audio = await ProtocolController.GetProtocolController().GetAudioAsync(Data.AudioId);
            } catch {
                Error = "Audio of this section could not be loaded.";
            }
        }
    }
}
