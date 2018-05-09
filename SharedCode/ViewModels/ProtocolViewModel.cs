using AutoScribeDataModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AutoScribeClient.ViewModels {
    /// <summary>
    /// ViewModel for ProtocolPage and NewProtocolPage.
    /// </summary>
    public class ProtocolViewModel : BaseViewModel
    {
        /// <summary>
        /// Data object of this protocol, which is sent to and receive from server.
        /// </summary>
        private Protocol data;

        /// <summary>
        /// All speakers participating in this protocol.
        /// </summary>
        private List<SpeakerViewModel> speakers;

        /// <summary>
        /// All sections of this protocol's transcript.
        /// </summary>
        private ObservableCollection<SectionViewModel> sections;

        /// <summary>
        /// This attribute contains information about the audio file of this protocol.
        /// </summary>
        private AudioViewModel audio;

        /// <summary>
        /// Indicator of whether this protocol is being process in server and the sections cannot be loaded.
        /// </summary>
        private bool inProcess;

        /// <summary>
        /// Indicator of whether this protocol is searchable to the user.
        /// </summary>
        private bool searchEnabled;

        /// <summary>
        /// Error to show to the user when there is one.
        /// </summary>
        private string error;

        /// <summary>
        /// Keywords of this protocol.
        /// </summary>
        private List<string> keywords;

        /// <summary>
        /// Indicator of whether this protocol is being reloaded.
        /// </summary>
        private bool isReloading;

        /// <summary>
        /// Command to save after user has edited this protocol.
        /// </summary>
        private ICommand saveProtocolCommand;

        /// <summary>
        /// Command to reload this protocol from database.
        /// </summary>
        private ICommand reloadProtocolCommand;

        /// <summary>
        /// Create a ViewModel using the data retrieved from server.
        /// </summary>
        /// <param name="data">Retrieved data.</param>
        public ProtocolViewModel(Protocol data) {
            Error = "";
            Data = data;
            Speakers = new List<SpeakerViewModel>();
            foreach (Speaker speaker in data.Speakers) {
                Speakers.Add(new SpeakerViewModel(speaker));
            }
            Audio = new AudioViewModel();
            SearchEnabled = false;
            //saveProtocolCommand = new Command(Save);
            //reloadProtocolCommand = new Command(ReloadAsync);
        }

        /// <summary>
        /// Create a ViewModel without data.
        /// </summary>
        public ProtocolViewModel() {
            Error = "";
            Data = new Protocol();
            CreatedTime = LastEditedTime = DateTime.Now;
            Speakers = new List<SpeakerViewModel>();
            Audio = new AudioViewModel();
            InProcess = true;
            SearchEnabled = false;
            //saveProtocolCommand = new Command(Save);
            //reloadProtocolCommand = new Command(ReloadAsync);
        }

        /// <summary>
        /// Name of this protocol, given by user.
        /// </summary>
        public string Name {
            get => Data.Name;
            set {
                if (Data.Name != value) {
                    Data.Name = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Time this protocol was created.
        /// </summary>
        public DateTime CreatedTime {
            get => Data.RecordedDate;
            set {
                if (Data.RecordedDate != value)
                {
                    Data.RecordedDate = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Last time this protocol was edited.
        /// </summary>
        public DateTime LastEditedTime {
            get => Data.EditedDate;
            set {
                if (Data.EditedDate != value)
                {
                    Data.EditedDate = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Command to save after user has edited this protocol.
        /// </summary>
        public ICommand SaveProtocolCommand => saveProtocolCommand;

        /// <summary>
        /// All speakers participating in this protocol.
        /// </summary>
        public List<SpeakerViewModel> Speakers {
            get => speakers;
            set {
                if (speakers != value)
                {
                    speakers = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// This attribute contains information about the audio file of this protocol.
        /// </summary>
        public AudioViewModel Audio { get => audio; set => audio = value; }

        /// <summary>
        /// All sections of this protocol's transcript.
        /// </summary>
        public ObservableCollection<SectionViewModel> Sections {
            get => sections;
            set {
                if (sections != value)
                {
                    sections = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Id of this protocol, given only by API.
        /// </summary>
        public string Id => Data.Id;

        /// <summary>
        /// Data object of this protocol, which is sent to and received from server.
        /// </summary>
        public Protocol Data { get => data; set => data = value; }

        /// <summary>
        /// Indicator of whether this protocol is being processed in server and the sections cannot be loaded.
        /// </summary>
        public bool InProcess { get => inProcess;
            set {
                if (inProcess != value) {
                    inProcess = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Indicator of whether this protocol is searchable to the user.
        /// </summary>
        public bool SearchEnabled { get => searchEnabled;
            set {
                if (searchEnabled != value) {
                    searchEnabled = value;
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
        /// Keywords of this protocol.
        /// </summary>
        public List<string> Keywords {
            get => keywords;
            set {
                if (keywords != value) {
                    keywords = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Command to reload this protocol from database.
        /// </summary>
        public ICommand ReloadProtocolCommand => reloadProtocolCommand;

        /// <summary>
        /// Indicator of whether this protocol is being reloaded.
        /// </summary>
        public bool IsReloading { get => isReloading;
            set {
                if (isReloading != value) {
                    isReloading = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Save this protocol after editing any section.
        /// </summary>
        public async Task Save() {
            try {
                Error = "";
                IsReloading = true;
                await ProtocolController.GetProtocolController().UpdateProtocolAsync(Data);
            } catch {
                Error = "Error occurs while updating this protocol, please try again";
            } finally {
                IsReloading = false;
            }
        }

        /// <summary>
        /// Load all sections and corresponding speakers.
        /// </summary>
        public async Task GetSections()
        {
            try {
                Error = "";
                IsReloading = true;
                if (Data.Status == ProtocolStatus.Empty || Data.Status == ProtocolStatus.InProgress) {
                    InProcess = true;
                    return;
                }
                if (Data.Status == ProtocolStatus.Error) {
                    Error = "This protocol could not be transcribed.";
                    return;
                }
                if (Sections == null) {
                    Sections = new ObservableCollection<SectionViewModel>();
                }
                Sections.Clear();
                Data.Sections = await ProtocolController.GetProtocolController().GetProtocolSectionsAsync(Data.Id);
                if (Data.Sections != null && Data.Sections.Count != 0) {
                    InProcess = false;
                    foreach (ProtocolSection section in Data.Sections) {
                        SectionViewModel vm = new SectionViewModel(section);
                        GetSpeakerFromProtocol(vm);
                        Sections.Add(vm);
                    }
                } else {
                    InProcess = true;
                }
            } catch {
                InProcess = false;
                Error = "Sections of this protocol could not be loaded, please try again.";
            } finally {
                IsReloading = false;
            }
        }

        private void GetSpeakerFromProtocol(SectionViewModel section) {
            if (section.SpeakerId == null) {
                return;
            }
            foreach (SpeakerViewModel speaker in Speakers) {
                if (speaker.Data.Id == section.SpeakerId) {
                    section.Speaker = speaker;
                    break;
                }
            }
        }

        public void GetKeywords()
        {
            Keywords = new List<string>();
            if (Data.Keywords != null && Data.Keywords.Count != 0) {
                Keywords.AddRange(Data.Keywords);
            }
        }
    }
}
