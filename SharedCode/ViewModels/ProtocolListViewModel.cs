using System.Collections.Generic;
using System.Windows.Input;
using AutoScribeDataModel;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using System;
using System.Diagnostics;

namespace AutoScribeClient.ViewModels
{
    /// <summary>
    /// ViewModel class used for ProtocolListPage and NewProtocolPage.
    /// </summary>
    public class ProtocolListViewModel : BaseViewModel
    {
        private static ProtocolListViewModel instance;

        /// <summary>
        /// Protocol list to show.
        /// </summary>
        private ObservableCollection<ProtocolViewModel> protocols;        

        private bool hasError;

        /// <summary>
        /// Error to show to the user while there is one.
        /// </summary>
        private string error;

        /// <summary>
        /// Indicator of whether the NewProtocolPage can be cancelled.
        /// A NewProtocolPage cannot be cancelled when it shows up as the app starts and can be cancelled when new protocol is being created from ProtocolListPage.
        /// </summary>
        private bool canBeCancelled;

        /// <summary>
        /// Indicator of whether the new protocol could be created.
        /// </summary>
        private bool onSuccess;

        /// <summary>
        /// Indicator of whether protocol list is being reloaded.
        /// </summary>
        private bool isReloading;

        /// <summary>
        /// Create a new object. All protocols are downloaded using ProtocolController class.
        /// </summary>
        private ProtocolListViewModel() {
            Protocols = new ObservableCollection<ProtocolViewModel>();
            ClearError();
            AddViewModels();
            CanBeCancelled = false;
            OnSuccess = false;
            IsReloading = false;
        }

        /// <summary>
        /// Get the single, static instance of ProtocolListViewModel.
        /// </summary>
        /// <returns></returns>
        public static ProtocolListViewModel GetProtocolListViewModel() {
            if (instance == null) {
                instance = new ProtocolListViewModel();
            }
            return instance;
        }

        /// <summary>
        /// Protocol list to show.
        /// </summary>
        public ObservableCollection<ProtocolViewModel> Protocols { get => protocols; set => protocols = value; }

        public override bool HasError {
            get => hasError;
            set {
                hasError = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Error to show to the user while there is one.
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

        /// <summary>
        /// Indicator of whether the NewProtocolPage can be cancelled.
        /// A NewProtocolPage cannot be cancelled when it shows up as the app starts and can be cancelled when new protocol is being created from ProtocolListPage.
        /// </summary>
        public bool CanBeCancelled {
            get => canBeCancelled;
            set {
                if (canBeCancelled != value) {
                    canBeCancelled = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Indicator of whether the new protocol could be created.
        /// </summary>
        public bool OnSuccess {
            get => onSuccess;
            set {
                if (onSuccess != value) {
                    onSuccess = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Indicator of whether the protocol list is being reloaded.
        /// </summary>
        public bool IsReloading {
            get => isReloading;
            set {
                if (isReloading != value) {
                    isReloading = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Create a new protocol to process using input from user.
        /// </summary>
        /// <param name="protocol">Input from user.</param>
        public async Task CreateProtocol(ProtocolViewModel protocol) {
            try {
                ClearError();
                IsReloading = true;
                protocol.Data.Speakers.AddRange(Enumerable.Select(protocol.Speakers, speaker => speaker.Data));
                protocol.Data = await ProtocolController.GetProtocolController().CreateProtocolAsync(protocol.Audio.Audio, protocol.Data);
                Protocols.Add(protocol);
                IsReloading = false;
            } catch (Exception e) {
                SetError("An error has occured while creating this protocol. Please try again.");
            } finally {
                IsReloading = false;
            }
        }

        /// <summary>
        /// Delete a protocol.
        /// </summary>
        /// <param name="protocol">Protocol to delete.</param>
        public async Task DeleteProtocol(ProtocolViewModel protocol) {
            try {
                IsReloading = true;
                ClearError();
                await ProtocolController.GetProtocolController().DeleteProtocolAsync(protocol.Id);
                Protocols.Remove(protocol);
            } catch {
                SetError("An error has occured while deleting this protocol. Please try again.");
            } finally {
                IsReloading = false;
            }
        }

        /// <summary>
        /// Get all protocols from database.
        /// </summary>
        public async Task<List<Protocol>> GetAllProtocols()
        {
            try {
                IsReloading = true;
                ClearError();          
                List<Protocol> pList = await ProtocolController.GetProtocolController().GetAllProtocolsAsync();                
                return pList;
            } catch {
                SetError("An error has occured while loading protocols. Please try again.");
            } finally {
                IsReloading = false;   
            }
            return new List<Protocol>();
        }

        public async void AddViewModels() {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            IsReloading = true;
            Protocols.Clear();
            foreach (Protocol protocol in await GetAllProtocols()) {
                Protocols.Add(new ProtocolViewModel(protocol));
            }
            if (stopwatch.ElapsedMilliseconds < 2000) {
                Thread.Sleep(2000 - (int)stopwatch.ElapsedMilliseconds);
            }
            IsReloading = false;
        }
    }
}
