using System;
using Newtonsoft.Json;

namespace AutoScribeDataModel
{
    /// <summary>
    /// One section of a protocol
    /// </summary>
    public class ProtocolSection
    {
        /// <summary>
        /// Text of the section
        /// </summary>
        [JsonProperty(PropertyName = "text")]
        public string Text { get; set; }

        /// <summary>
        /// Id of the corresponding audio segment
        /// </summary>
        [JsonProperty(PropertyName = "audioid")]
        public string AudioId { get; set; }

        /// <summary>
        /// Id of the speaker of the section
        /// </summary>
        [JsonProperty(PropertyName = "speakerid")]
        public string SpeakerId { get; set; }

        public ProtocolSection()
        {
        }
    }
}
