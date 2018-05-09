using System.Collections.Generic;
using AutoScribeDataModel;
using AutoScribeClient.ServerAccess;
using System.Threading.Tasks;
using System.IO;

namespace AutoScribeClient {
    /// <summary>
    /// Controller class that handles all data related functions.
    /// Currently only delegates them to the ServiceConnection.
    /// Implements the singleton pattern.
    /// </summary>
    public class ProtocolController {
        public IServiceConnection ServiceConnection { get; set; }
        private static ProtocolController instance;

        /// <summary>
        /// Sets up the ServiceConnection.
        /// </summary>
        public ProtocolController()
        {
        }

        /// <summary>
        /// Returns the single instance of the controller
        /// </summary>
        /// <returns>The ProtocolController instance</returns>
        public static ProtocolController GetProtocolController()
        {
            if (instance == null) {
                instance = new ProtocolController {
                    ServiceConnection = new ServiceConnection()
                };
            }
            return instance;
        }

        //-------------------------------------
        // Speaker operations

        /// <summary>
        /// Creates a new speaker
        /// </summary>
        /// <param name="name">The name of the speaker</param>
        /// <param name="enrollmentAudio">The audio used to identify the speaker</param>
        /// <returns>The created speaker</returns>
        public async Task<Speaker> CreateSpeakerAsync(string name, Stream enrollmentAudio)
        {
            var speaker = await ServiceConnection.CreateSpeakerAsync(name, enrollmentAudio);
            if (speaker != null) {
                enrollmentAudio.Close();
                File.Delete(((FileStream)enrollmentAudio).Name);
            }
            return speaker;
        }

        /// <summary>
        /// Deletes the speaker with the given id
        /// </summary>
        /// <param name="id">The id of the speaker to delete</param>
        public async Task DeleteSpeakerAsync(string id)
        {
            await ServiceConnection.DeleteSpeakerAsync(id);
        }

        /// <summary>
        /// Gets the speaker with the given id
        /// </summary>
        /// <param name="id">The id of the speaker to get</param>
        /// <returns>The speaker with the given id</returns>
        public async Task<Speaker> GetSpeakerAsync(string id)
        {
            return await ServiceConnection.GetSpeakerAsync(id);
        }

        /// <summary>
        /// Gets all speakers
        /// </summary>
        /// <returns>All speakers</returns>
        public async Task<List<Speaker>> GetAllSpeakersAsync()
        {
            return await ServiceConnection.GetAllSpeakersAsync();
        }

        /// <summary>
        /// Updates the speaker
        /// </summary>
        /// <param name="speaker">The speaker to update</param>
        /// <param name="enrollmentAudio">The new audio used to identify the speaker (can be null indicating that the audio should not be updated)</param>
        /// <returns>The updated speaker</returns>
        public async Task UpdateSpeakerAsync(Speaker speaker, Stream enrollmentAudio)
        {
            await ServiceConnection.UpdateSpeakerAsync(speaker, enrollmentAudio);
        }

        //-------------------------------------
        // Protocol operations

        /// <summary>
        /// Creates a new protocol
        /// </summary>
        /// <param name="audio">The audio of the protocol</param>
        /// <param name="protocol">The protocol information</param>
        /// <returns>The created protocol (without the sections, which are started to be created after the server responded with this protocol)</returns>
        public async Task<Protocol> CreateProtocolAsync(Stream audio, Protocol protocol)
        {
            var returnedProtocol = await ServiceConnection.CreateProtocolAsync(audio, protocol);
            //Delete Audio-File if protocol creation was successful
            if (returnedProtocol != null) {
                audio.Close();
                File.Delete(((FileStream)audio).Name);
            }
            return returnedProtocol;
        }

        /// <summary>
        /// Deletes the protocol with the given id
        /// </summary>
        /// <param name="id">The id of the protocol to delete</param>
        public async Task DeleteProtocolAsync(string id)
        {
            await ServiceConnection.DeleteProtocolAsync(id);
        }

        /// <summary>
        /// Gets all protocols
        /// </summary>
        /// <returns>All protocols</returns>
        public async Task<List<Protocol>> GetAllProtocolsAsync()
        {
            return await ServiceConnection.GetAllProtocolsAsync();
        }

        /// <summary>
        /// Gets the protocol with the given id
        /// </summary>
        /// <param name="id">The id of the protocol to get</param>
        /// <returns>The protocol with the given id</returns>
        public async Task<Protocol> GetProtocolAsync(string id)
        {
            return await ServiceConnection.GetProtocolAsync(id);
        }

        /// <summary>
        /// Gets the protocol sections of the protocol with the given id
        /// </summary>
        /// <param name="id">The id of the protocol</param>
        /// <returns>The protocol sections</returns>
        public async Task<List<ProtocolSection>> GetProtocolSectionsAsync(string id)
        {
            return await ServiceConnection.GetProtocolSectionsAsync(id);
        }

        /// <summary>
        /// Gets the protocol status of the protocol with the given id
        /// </summary>
        /// <param name="id">The id of the protocol</param>
        /// <returns>The protocol status</returns>
        public async Task<ProtocolStatus> GetProtocolStatusAsync(string id)
        {
            return await ServiceConnection.GetProtocolStatusAsync(id);
        }

        /// <summary>
        /// Updates the protocol
        /// </summary>
        /// <param name="protocol">The protocol to update</param>
        public async Task UpdateProtocolAsync(Protocol protocol)
        {
            await ServiceConnection.UpdateProtocolAsync(protocol);
        }

        //--------------------------------------
        // Audio operation

        /// <summary>
        /// Gets the audio with the given id
        /// </summary>
        /// <param name="audioId">The id of the audio to get</param>
        /// <returns>The audio with the given id</returns>
        public async Task<Stream> GetAudioAsync(string audioId)
        {
            return await ServiceConnection.GetAudioAsync(audioId);
        }
    }
}
