using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace AutoScribeDataModel
{
    /// <summary>
    /// Protocol class that encapsulates all information belonging to one instance.
    /// </summary>
    public class Protocol
    {
        /// <summary>
        /// Database Id
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Name of the protocol
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Date the protocol was recorded
        /// </summary>
        [JsonProperty(PropertyName = "recorded")]
        public DateTime RecordedDate { get; set; }

        /// <summary>
        /// Date the protocol was last edited
        /// </summary>
        [JsonProperty(PropertyName = "edited")]
        public DateTime EditedDate { get; set; }

        /// <summary>
        /// Id of the transcribed audio
        /// </summary>
        [JsonProperty(PropertyName = "audioid")]
        public string AudioId { get; set; }

        /// <summary>
        /// List of all speakers within the conversation
        /// </summary>
        [JsonProperty(PropertyName = "speakers")]
        public List<Speaker> Speakers { get; }

        /// <summary>
        /// Collection of keywords found in the protocol
        /// </summary>
        [JsonProperty(PropertyName = "keywords")]
        public List<string> Keywords { get; set; }

        /// <summary>
        /// The status of the sections
        /// </summary>
        [JsonProperty(PropertyName = "status")]
        public ProtocolStatus Status { get; set; }
        
        /// <summary>
        /// List of all sections the protocol consists of
        /// </summary>
        [JsonProperty(PropertyName = "sections")]
        public List<ProtocolSection> Sections { get; set; }

        /// <summary>
        /// Create empty lists on creation
        /// </summary>
        public Protocol()
        {
            Speakers = new List<Speaker>();
            Keywords = null;
            Sections = null;

            Status = ProtocolStatus.Empty;
        }
    }
}
