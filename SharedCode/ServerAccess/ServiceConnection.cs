using AutoScribeDataModel;
using Microsoft.WindowsAzure.MobileServices;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace AutoScribeClient.ServerAccess {
    /// <summary>
    /// A connection to the app service
    /// </summary>
    public class ServiceConnection : IServiceConnection {
        /// <summary>
        /// The uri of the app service
        /// </summary>
        private const string MOBILE_SERVICE_URI = "http://localhost:59482/"; //https://autoscribe.azurewebsites.net/
        /// <summary>
        /// The language to use in all requests
        /// </summary>
        private const string LANGUAGE = "en-US";

        /// <summary>
        /// Creates a new service connection
        /// </summary>
        public ServiceConnection()
        {
            client = new MobileServiceClient(MOBILE_SERVICE_URI);
        }

        private MobileServiceClient client;

        /// <summary>
        /// Creates a new speaker
        /// </summary>
        /// <param name="name">The name of the speaker</param>
        /// <param name="enrollmentAudio">The audio used to identify the speaker</param>
        /// <returns>The created speaker</returns>
        public async Task<Speaker> CreateSpeakerAsync(string name, Stream enrollmentAudio)
        {
            try {
                HttpResponseMessage response = await HttpMessageHelper.RequestApiAsync(client, HttpMethod.Post, "create-speaker", new Dictionary<string, object>()
                {
                    { "name", name }
                }, new StreamContent(enrollmentAudio));
                Speaker speaker = await HttpMessageHelper.DeserializeResponse<Speaker>(response);

                return speaker;
            } catch (MobileServiceInvalidOperationException ex) {
                throw new ServerAccessException(ex.Response.StatusCode);
            }
        }
        /// <summary>
        /// Deletes the speaker with the given id
        /// </summary>
        /// <param name="id">The id of the speaker to delete</param>
        public async Task DeleteSpeakerAsync(string id)
        {
            try {
                await HttpMessageHelper.RequestApiAsync(client, HttpMethod.Delete, "delete-speaker", new Dictionary<string, object>()
                {
                    { "id", id },
                    { "language", LANGUAGE }
                });
            } catch (MobileServiceInvalidOperationException ex) {
                if (ex.Response.StatusCode == HttpStatusCode.NotFound)
                    throw new ServerRessourceNotFoundException(nameof(DeleteSpeakerAsync), id);
                else
                    throw new ServerAccessException(ex.Response.StatusCode);
            }
        }
        /// <summary>
        /// Gets all speakers
        /// </summary>
        /// <returns>All speakers</returns>
        public async Task<List<Speaker>> GetAllSpeakersAsync()
        {
            try {
                HttpResponseMessage response = await HttpMessageHelper.RequestApiAsync(client, HttpMethod.Get, "getall-speaker", new Dictionary<string, object>());
                List<Speaker> speakers = await HttpMessageHelper.DeserializeResponse<List<Speaker>>(response);

                return speakers;
            } catch (MobileServiceInvalidOperationException ex) {
                throw new ServerAccessException(ex.Response.StatusCode);
            }
        }
        /// <summary>
        /// Gets the speaker with the given id
        /// </summary>
        /// <param name="id">The id of the speaker to get</param>
        /// <returns>The speaker with the given id</returns>
        public async Task<Speaker> GetSpeakerAsync(string id)
        {
            try {
                HttpResponseMessage response = await HttpMessageHelper.RequestApiAsync(client, HttpMethod.Get, "get-speaker", new Dictionary<string, object>()
                {
                    { "id", id }
                });
                Speaker speaker = await HttpMessageHelper.DeserializeResponse<Speaker>(response);

                return speaker;
            } catch (MobileServiceInvalidOperationException ex) {
                if (ex.Response.StatusCode == HttpStatusCode.NotFound)
                    throw new ServerRessourceNotFoundException(nameof(GetSpeakerAsync), id);
                else
                    throw new ServerAccessException(ex.Response.StatusCode);
            }
        }
        /// <summary>
        /// Updates the speaker
        /// </summary>
        /// <param name="speaker">The speaker to update</param>
        /// <param name="enrollmentAudio">The new audio used to identify the speaker (can be null indicating that the audio should not be updated)</param>
        /// <returns>The updated speaker</returns>
        public async Task<Speaker> UpdateSpeakerAsync(Speaker speaker, Stream enrollmentAudio = null)
        {
            try {
                HttpResponseMessage response = await HttpMessageHelper.RequestApiAsync(client, HttpMethod.Put, "update-speaker", new Dictionary<string, object>()
                {
                    { "speaker", speaker },
                    { "language", LANGUAGE }
                }, new StreamContent(enrollmentAudio));
                Speaker speakerResponse = await HttpMessageHelper.DeserializeResponse<Speaker>(response);

                return speakerResponse;
            } catch (MobileServiceInvalidOperationException ex) {
                if (ex.Response.StatusCode == HttpStatusCode.NotFound)
                    throw new ServerRessourceNotFoundException(nameof(UpdateSpeakerAsync), speaker.Id);
                else
                    throw new ServerAccessException(ex.Response.StatusCode);
            }
        }

        /// <summary>
        /// Creates a new protocol
        /// </summary>
        /// <param name="audio">The audio of the protocol</param>
        /// <param name="protocol">The protocol information</param>
        /// <returns>The created protocol (without the sections, which are started to be created after the server responded with this protocol)</returns>
        public async Task<Protocol> CreateProtocolAsync(Stream audio, Protocol protocol)
        {
            try {
                HttpResponseMessage response = await HttpMessageHelper.RequestApiAsync(client, HttpMethod.Post, "create-protocol", new Dictionary<string, object>()
                {
                    { "protocol", protocol },
                    { "language", LANGUAGE }
                }, new StreamContent(audio));
                Protocol protocolResponse = await HttpMessageHelper.DeserializeResponse<Protocol>(response);

                return protocolResponse;
            } catch (MobileServiceInvalidOperationException ex) {
                throw new ServerAccessException(ex.Response.StatusCode);
            }
        }
        /// <summary>
        /// Deletes the protocol with the given id
        /// </summary>
        /// <param name="id">The id of the protocol to delete</param>
        public async Task DeleteProtocolAsync(string id)
        {
            try {
                await HttpMessageHelper.RequestApiAsync(client, HttpMethod.Delete, "delete-protocol", new Dictionary<string, object>()
                {
                    { "id", id }
                });
            } catch (MobileServiceInvalidOperationException ex) {
                if (ex.Response.StatusCode == HttpStatusCode.NotFound)
                    throw new ServerRessourceNotFoundException(nameof(DeleteProtocolAsync), id);
                else
                    throw new ServerAccessException(ex.Response.StatusCode);
            }
        }
        /// <summary>
        /// Gets all protocols
        /// </summary>
        /// <returns>All protocols</returns>
        public async Task<List<Protocol>> GetAllProtocolsAsync()
        {
            try {
                HttpResponseMessage response = await HttpMessageHelper.RequestApiAsync(client, HttpMethod.Get, "getall-protocol", new Dictionary<string, object>());
                List<Protocol> protocols = await HttpMessageHelper.DeserializeResponse<List<Protocol>>(response);

                return protocols;
            } catch (MobileServiceInvalidOperationException ex) {
                throw new ServerAccessException(ex.Response.StatusCode);
            }
        }
        /// <summary>
        /// Gets the protocol with the given id
        /// </summary>
        /// <param name="id">The id of the protocol to get</param>
        /// <returns>The protocol with the given id</returns>
        public async Task<Protocol> GetProtocolAsync(string id)
        {
            try {
                HttpResponseMessage response = await HttpMessageHelper.RequestApiAsync(client, HttpMethod.Get, "get-protocol", new Dictionary<string, object>()
                {
                    { "id", id }
                });
                Protocol protocol = await HttpMessageHelper.DeserializeResponse<Protocol>(response);

                return protocol;
            } catch (MobileServiceInvalidOperationException ex) {
                if (ex.Response.StatusCode == HttpStatusCode.NotFound)
                    throw new ServerRessourceNotFoundException(nameof(GetProtocolAsync), id);
                else
                    throw new ServerAccessException(ex.Response.StatusCode);
            }
        }
        /// <summary>
        /// Gets the protocol sections of the protocol with the given id
        /// </summary>
        /// <param name="id">The id of the protocol</param>
        /// <returns>The protocol sections</returns>
        public async Task<List<ProtocolSection>> GetProtocolSectionsAsync(string id)
        {
            try {
                HttpResponseMessage response = await HttpMessageHelper.RequestApiAsync(client, HttpMethod.Get, "get-protocol-sections", new Dictionary<string, object>()
                {
                    { "id", id }
                });
                List<ProtocolSection> protocolSections = await HttpMessageHelper.DeserializeResponse<List<ProtocolSection>>(response);

                return protocolSections;
            } catch (MobileServiceInvalidOperationException ex) {
                if (ex.Response.StatusCode == HttpStatusCode.NotFound)
                    throw new ServerRessourceNotFoundException(nameof(GetProtocolSectionsAsync), id);
                else
                    throw new ServerAccessException(ex.Response.StatusCode);
            }
        }
        /// <summary>
        /// Gets the protocol status of the protocol with the given id
        /// </summary>
        /// <param name="id">The id of the protocol</param>
        /// <returns>The protocol status</returns>
        public async Task<ProtocolStatus> GetProtocolStatusAsync(string id)
        {
            try {
                HttpResponseMessage response = await HttpMessageHelper.RequestApiAsync(client, HttpMethod.Get, "get-protocol-status", new Dictionary<string, object>()
                {
                    { "id", id }
                });
                ProtocolStatus protocolStatus = await HttpMessageHelper.DeserializeResponse<ProtocolStatus>(response);

                return protocolStatus;
            } catch (MobileServiceInvalidOperationException ex) {
                if (ex.Response.StatusCode == HttpStatusCode.NotFound)
                    throw new ServerRessourceNotFoundException(nameof(GetProtocolStatusAsync), id);
                else
                    throw new ServerAccessException(ex.Response.StatusCode);
            }
        }
        /// <summary>
        /// Updates the protocol
        /// </summary>
        /// <param name="protocol">The protocol to update</param>
        public async Task UpdateProtocolAsync(Protocol protocol)
        {
            try {
                await HttpMessageHelper.RequestApiAsync(client, HttpMethod.Put, "update-protocol", new Dictionary<string, object>(),
                    new StringContent(HttpMessageHelper.Serialize(protocol)));
            } catch (MobileServiceInvalidOperationException ex) {
                if (ex.Response.StatusCode == HttpStatusCode.NotFound)
                    throw new ServerRessourceNotFoundException(nameof(UpdateProtocolAsync), protocol.Id);
                else
                    throw new ServerAccessException(ex.Response.StatusCode);
            }
        }

        /// <summary>
        /// Gets the audio with the given id
        /// </summary>
        /// <param name="id">The id of the audio to get</param>
        /// <returns>The audio with the given id</returns>
        public async Task<Stream> GetAudioAsync(string id)
        {
            try {
                HttpResponseMessage response = await HttpMessageHelper.RequestApiAsync(client, HttpMethod.Get, "get-audio", new Dictionary<string, object>()
                {
                    { "id", id }
                });
                Stream audio = await HttpMessageHelper.CreateContentStream(response);

                audio.Seek(0, SeekOrigin.Begin);
                return audio;
            } catch (MobileServiceInvalidOperationException ex) {
                if (ex.Response.StatusCode == HttpStatusCode.NotFound)
                    throw new ServerRessourceNotFoundException(nameof(GetAudioAsync), id);
                else
                    throw new ServerAccessException(ex.Response.StatusCode);
            }
        }
    }
}
