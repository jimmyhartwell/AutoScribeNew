using AutoScribeDataModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AutoScribeClient.ViewModels
{
    /// <summary>
    /// ViewModel used for SpeakerListPage and NewSpeakerPage.
    /// </summary>
    public class SpeakerListViewModel : BaseViewModel
    {
        private static SpeakerListViewModel instance;

        /// <summary>
        /// All available speakers.
        /// </summary>
        private ObservableCollection<SpeakerViewModel> speakers;
        
        /// <summary>
        /// Error to show to user when there is one.
        /// </summary>
        private string error;

        private bool hasError;

        private bool isReloading;

        /// <summary>
        /// Create a new ViewModel using speaker objects retrieved from server.
        /// </summary>
        private SpeakerListViewModel() {
            ClearError();
            Speakers = new ObservableCollection<SpeakerViewModel>();
            AddViewModels();
        }

        public static SpeakerListViewModel GetSpeakerListViewModel() {
            if (instance == null) {
                instance = new SpeakerListViewModel();
            }
            return instance;
        }

        /// <summary>
        /// All available speakers.
        /// </summary>
        public ObservableCollection<SpeakerViewModel> Speakers {
            get => speakers;
            set {
                if (speakers != value) {
                    speakers = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Error to show to user when there is one.
        /// </summary>
        public override string Error {
            get => error;
            set {
                if (error != value) {
                    error = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool IsReloading { get => isReloading;
            set {
                if (isReloading != value) {
                    isReloading = value;
                    OnPropertyChanged();
                }
            }
        }

        public override bool HasError {
            get => hasError;
            set {
                hasError = value;
                OnPropertyChanged();
            }
        }


        /// <summary>
        /// Create a new speaker and send data to server.
        /// </summary>
        /// <param name="speaker">Input from user.</param>
        public async Task CreateSpeaker(SpeakerViewModel speaker) {
            try {
                ClearError();
                speaker.Data = await ProtocolController.GetProtocolController().CreateSpeakerAsync(speaker.Name, speaker.EnrollmentAudio.Audio);
                Speakers.Add(speaker);
            } catch {
                SetError("An error has occured while creating this speaker. Please try again.");
                speaker.EnrollmentAudio.Audio.Dispose();
            }
        }

        /// <summary>
        /// Delete a speaker.
        /// </summary>
        /// <param name="speaker">Input from user.</param>
        public async Task DeleteSpeaker(SpeakerViewModel speaker) {
            try {
                IsReloading = true;
                ClearError();
                await ProtocolController.GetProtocolController().DeleteSpeakerAsync(speaker.Data.Id);
                Speakers.Remove(speaker);
            } catch {
                SetError("An error has occured while deleting this speaker. Please try again.");
            } finally {
                IsReloading = false;
            } 
        }

        /// <summary>
        /// Get all speakers from database.
        /// </summary>
        private async Task<List<Speaker>> GetAllSpeakers() {
            try {
                IsReloading = true;
                ClearError();
                Speakers.Clear();
                List<Speaker> speakerList = await ProtocolController.GetProtocolController().GetAllSpeakersAsync();
                return speakerList;
            } catch {
                SetError("An error has occured while loading speakers. Please try again.");
            } finally {
                IsReloading = false;
            }
            return new List<Speaker>();
        }

        public async void AddViewModels()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            IsReloading = true;
            Speakers.Clear();
            foreach (Speaker speaker in await GetAllSpeakers()) {
                Speakers.Add(new SpeakerViewModel(speaker));
            }
            if (stopwatch.ElapsedMilliseconds < 2000) {
                Thread.Sleep(2000 - (int)stopwatch.ElapsedMilliseconds);
            }
            IsReloading = false;
        }
    }
}
