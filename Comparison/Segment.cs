using InvertedTomato.LightWeightSerialization;
using Newtonsoft.Json;

namespace Comparison {
    public class Segment {
        /// <summary>
        /// Determines how to compute mapping to verse ID.
        /// </summary>
        /// <remarks>
        /// 0 = continuation of last verse in sequence
        /// 1 = entire verse on it's own
        /// N = number of entire verses
        /// </remarks>
        [LightWeightProperty(0)]
        [JsonProperty("m")]
        public byte Mode { get; set; }
        
        [LightWeightProperty(1)]
        [JsonProperty("c")]
        public string Content { get; set; }
    }
}
