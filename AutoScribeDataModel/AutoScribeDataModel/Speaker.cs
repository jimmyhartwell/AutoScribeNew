using Newtonsoft.Json;
using System;

namespace AutoScribeDataModel
{
    /// <summary>
    /// Class that contains all information about a speaker
    /// </summary>
    public class Speaker
    {
        /// <summary>
        /// Database Id assigned by the database
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Id assigned by the Azure Speaker Recognition API
        /// </summary>
        [JsonProperty(PropertyName = "apiid")]
        public string ApiId { get; set; }

        /// <summary>
        /// Name of the speaker
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Id of the audio used for enrollment
        /// </summary>
        [JsonProperty(PropertyName = "enrollmentaudioid")]
        public string EnrollmentAudioId { get; set; }

        public Speaker()
        {
        }
    }
}
