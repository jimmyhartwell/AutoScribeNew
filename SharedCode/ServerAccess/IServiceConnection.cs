using AutoScribeDataModel;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AutoScribeClient.ServerAccess {
    public interface IServiceConnection {
        Task DeleteSpeakerAsync(string id);
        Task<Speaker> CreateSpeakerAsync(string name, Stream enrollmentAudio);
        Task<Speaker> GetSpeakerAsync(string id);
        Task<List<Speaker>> GetAllSpeakersAsync();
        Task<Speaker> UpdateSpeakerAsync(Speaker speaker, Stream enrollmentAudio);
        Task<Protocol> CreateProtocolAsync(Stream audio, Protocol protocol);
        Task DeleteProtocolAsync(string id);
        Task<List<Protocol>> GetAllProtocolsAsync();
        Task<Protocol> GetProtocolAsync(string id);
        Task<List<ProtocolSection>> GetProtocolSectionsAsync(string id);
        Task<ProtocolStatus> GetProtocolStatusAsync(string id);
        Task UpdateProtocolAsync(Protocol protocol);
        Task<Stream> GetAudioAsync(string id);
    }
}
