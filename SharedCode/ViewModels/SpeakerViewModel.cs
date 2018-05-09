using AutoScribeDataModel;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AutoScribeClient.ViewModels
{
    /// <summary>
    /// ViewModel for SpeakerPage
    /// </summary>
    public class SpeakerViewModel : BaseViewModel
    {
        /// <summary>
        /// Data object for this ViewModel sent and retrieved from server.
        /// </summary>
        private Speaker data;

        /// <summary>
        /// This attribute
        /// </summary>
        private AudioViewModel enrollmentAudio;

        /// <summary>
        /// Indicator of whether this speaker can be edited.
        /// </summary>
        private bool editMode;

        /// <summary>
        /// Indicator whether this speaker is newly added.
        /// </summary>
        private string newlyAdded;

        /// <summary>
        /// Temporary name, internal used for editing speaker.
        /// </summary>
        private string tempName;

        /// <summary>
        /// Error to show to user when there is one.
        /// </summary>
        private string error;

        /// <summary>
        /// Indicator when an editing of this speaker is being saved.
        /// </summary>
        private bool isSaving;

        /// <summary>
        /// Command to edit this speaker.
        /// </summary>
        private ICommand editCommand;

        /// <summary>
        /// Command to save this speaker after editing.
        /// </summary>
        private ICommand saveCommand;

        /// <summary>
        /// Command to cancel while editing.
        /// </summary>
        private ICommand cancelCommand;

        /// <summary>
        /// Create a new ViewModel with speaker data retrieved from server.
        /// </summary>
        /// <param name="data">Retrieved data.</param>
        public SpeakerViewModel(Speaker data) {
            Data = data;
            EnrollmentAudio = new AudioViewModel();
            EditMode = false;
            IsSaving = false;
        }

        /// <summary>
        /// Create a new ViewModel without speaker data.
        /// </summary>
        public SpeakerViewModel() {
            Error = "";
            Data = new Speaker();
            EnrollmentAudio = new AudioViewModel();
            EditMode = false;
            NewlyAdded = "New";
        }

        /// <summary>
        /// Name of this speaker, can only be given by user.
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
        /// This attribute
        /// </summary>
        public AudioViewModel EnrollmentAudio { get => enrollmentAudio; set => enrollmentAudio = value; }

        /// <summary>
        /// Indicator of whether this speaker can be edited.
        /// </summary>
        public bool EditMode {
            get => editMode;
            set {
                if (editMode != value) {
                    editMode = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Command to edit this speaker.
        /// </summary>
        public ICommand EditCommand => editCommand;

        /// <summary>
        /// Command to save this speaker after editing.
        /// </summary>
        public ICommand SaveCommand => saveCommand;

        /// <summary>
        /// Command to cancel while editing.
        /// </summary>
        public ICommand CancelCommand => cancelCommand;

        /// <summary>
        /// Data object for this ViewModel.
        /// </summary>
        public Speaker Data { get => data; set => data = value; }

        /// <summary>
        /// Indicator whether this speaker is newly added.
        /// </summary>
        public string NewlyAdded { get => newlyAdded; set => newlyAdded = value; }

        /// <summary>
        /// Error to show to user when there is one.
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
        /// Indicator when the editing of this speaker is being saved.
        /// </summary>
        public bool IsSaving { get => isSaving;
            set {
                if (isSaving != value) {
                    isSaving = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Edit this speaker.
        /// </summary>
        private void Edit() {
            Error = EnrollmentAudio.Error = "";
            EditMode = true;
            tempName = Name;
        }

        /// <summary>
        /// Save this speaker after editing.
        /// </summary>
        public async Task Save() {
            if (!String.IsNullOrWhiteSpace(Name)) {
                try {
                    IsSaving = true;
                    await ProtocolController.GetProtocolController().UpdateSpeakerAsync(Data, EnrollmentAudio.Audio);
                    Data = await ProtocolController.GetProtocolController().GetSpeakerAsync(Data.Id);

                    await ProtocolListViewModel.GetProtocolListViewModel().GetAllProtocols();

                } catch {
                    //await Task.Run(() => {
                    //    Error = "Error while updating speaker, please try again.";
                    //    Thread.Sleep(3000);
                    //    Error = "";
                    //});
                    //IsSaving = false;
                }
            } else {
                Error = "Name cannot be empty";
            }
        }

        /// <summary>
        /// Cancel editing.
        /// </summary>
        private void Cancel() {
            Name = tempName;
            EditMode = false;
        }

        /// <summary>
        /// Load enrollment audio of this speaker from database.
        /// </summary>
        public async Task GetAudio() {
            try {
                EnrollmentAudio.Error = "";
                EnrollmentAudio.Audio = await ProtocolController.GetProtocolController().GetAudioAsync(data.EnrollmentAudioId);               
            } catch {
                EnrollmentAudio.Error = "Enrollment audio of this speaker could not be loaded.";
            }
        }

        public async Task Refresh() {
            Data = await ProtocolController.GetProtocolController().GetSpeakerAsync(Data.Id);
        }
    }
}
